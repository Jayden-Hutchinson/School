defmodule Chat.Proxy do
  use GenServer
  require Logger

  def log(message) do
    send(self(), {:log, message})
  end

  def start_link(socket) do
    GenServer.start_link(__MODULE__, socket)
  end

  def handle("/NCK" <> input, state) do
    case Chat.parse_nickname(input) do
      {:nickname, nickname} ->
        Chat.Server.set_nickname(state.nickname, nickname, self())
        :gen_tcp.send(state.socket, "Nickname set to #{nickname}\n")
        new_state = %{state | nickname: nickname}
        {:noreply, new_state}

      {:error, reason} ->
        :gen_tcp.send(state.socket, "#{reason}\n")
    end
  end

  def handle("/MSG" <> rest, state) do
    {input, message} =
      rest |> String.trim() |> String.split(" ", parts: 2, trim: true)

    if not is_registered(state.nickname) do
      :gen_tcp.send(state.socket, "\e[32mMust be registered to use command.\e[0m\n")
    else
      recipiants = input |> String.split(",", trim: true) |> get_group_recipiants(state)

      Enum.each(recipiants, fn recipiant ->
        Chat.Server.send_message(recipiant, state.nickname, message)
      end)

      :gen_tcp.send(state.socket, "Message sent to #{recipiants}")
    end
  end

  def handle("/GRP" <> rest, state) do
    {group_name, input} =
      rest |> String.trim() |> String.split(" ", parts: 2, trim: true)

    nicknames = input |> String.split(",", trim: true)

    case Chat.parse_group_name(group_name) do
      {:group_name, group_name} ->
        :ets.insert(state.table, {group_name, nicknames})
        :gen_tcp.send(state.socket, "Group #{group_name} created")

      {:error, reason} ->
        :gen_tcp.send(state.socket, "#{reason}\n")
    end
  end

  def handle("/LST" <> _rest, state) do
    nicknames = Chat.Server.get_current_users()
    :gen_tcp.send(state.socket, nicknames)
  end

  def handle(_, _state) do
    log("Invalid Command")
  end

  @impl true
  def init(socket) do
    table = :ets.new(:groups, [:set, :protected])
    nickname = "unregistered"
    state = %{nickname: nickname, socket: socket, table: table}

    log("Started - Controlling #{inspect(socket)}")

    {:ok, state}
  end

  @impl true
  def handle_info({:tcp, socket, command}, state) do
    handle(command, state)
    :inet.setopts(socket, active: :once)
    {:noreply, state}
  end

  @impl true
  def handle_info({:tcp_closed, socket}, state) do
    log("#{inspect(socket)} Closed")
    :gen_tcp.close(socket)
    {:stop, :normal, state}
  end

  @impl true
  def handle_info({:group_msg, group, msg}, state) do
    case :ets.lookup(state.table, group) do
      [{_group, nicknames}] ->
        Enum.each(nicknames, fn to ->
          Chat.Server.send_message(to, state.nickname, msg)
        end)

      [] ->
        log("#{group} is not a group")
    end

    {:noreply, state}
  end

  @impl true
  def handle_info({:error, reason}, state) do
    :gen_tcp.send(state.socket, "Error: #{reason}\n")
    {:noreply, state}
  end

  @impl true
  def handle_info({:grp, group, nicknames}, state) do
    :ets.insert(state.table, {group, nicknames})
    log("Current Groups: #{inspect(:ets.tab2list(state.table))}")

    {:noreply, state}
  end

  @impl true
  def handle_info({:lst}, state) do
    Chat.Server.get_nicknames(self())
    {:noreply, state}
  end

  @impl true
  def handle_info({:message, from, message}, state) do
    :gen_tcp.send(state.socket, "#{from}: #{message}")

    {:noreply, state}
  end

  @impl true
  def handle_info({:current_users}, state) do
    nicknames = Chat.Server.get_current_users()
    :gen_tcp.send(state.socket, "Users #{inspect(nicknames)}\n")
    {:noreply, state}
  end

  @impl true
  def handle_info({:log, message}, state) do
    Logger.info("[#{inspect(self())}] [Proxy] [#{state.nickname}] #{message}")
    {:noreply, state}
  end

  defp is_registered(nickname) do
    nickname == "unregistered"
  end

  defp is_group(nickname) do
    String.starts_with?(nickname, "#")
  end

  defp get_group_recipiants(recipiants, state) do
    Enum.reduce(recipiants, [], fn recipiant, acc ->
      cond do
        is_group(recipiant) ->
          case :ets.lookup(state.table, recipiant) do
            [{_key, nickname_list}] ->
              acc ++ nickname_list

            [] ->
              acc
          end

        true ->
          acc ++ [recipiant]
      end
    end)
    |> Enum.uniq()
    |> Enum.reject(&(&1 == state.nickname))
  end
end
