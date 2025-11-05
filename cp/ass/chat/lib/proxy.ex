defmodule Chat.Proxy do
  use GenServer
  require Logger

  def log(message) do
    send(self(), {:log, message})
  end

  def start_link(socket) do
    GenServer.start_link(__MODULE__, socket)
  end

  def handle("/NCK" <> rest) do
    args = rest |> String.trim() |> String.split()

    case args do
      [name | _] -> send(self(), {:nck, name})
      [] -> Logger.info("invalid input")
    end
  end

  def handle("/MSG" <> rest) do
    parts =
      rest |> String.trim() |> String.split(" ", parts: 2, trim: true)

    case parts do
      [recipiant_str, msg] ->
        recipiants = recipiant_str |> String.split(",")
        send(self(), {:msg, recipiants, msg})

      [_recipiant_str] ->
        log("Must provide a message")

      _ ->
        log("Invalid Input")
    end
  end

  def handle("/GRP" <> rest) do
    args = rest |> String.trim() |> String.split(",", trim: true) |> Enum.map(&String.trim/1)

    [first | rest] = args
    [group, name] = first |> String.trim() |> String.split(" ", trim: true)
    names = rest ++ [name]

    unless String.starts_with?(group, "#") do
      log("Group name must start with \'#\'")
      :ok
    else
      log("#{inspect(group)}")
      log("#{inspect(names)}")

      send(self(), {:grp, group, names})
    end
  end

  def handle("/LST" <> _rest) do
    send(self(), {:lst})
  end

  def handle(_) do
    log("Invalid Command")
  end

  @impl true
  def init(socket) do
    table = :ets.new(:groups, [:set, :protected])
    name = "unregistered"
    state = {name, socket, table}

    log("Started - Controlling #{inspect(socket)}")

    {:ok, state}
  end

  @impl true
  def handle_info({:tcp, socket, command}, state = {_name, socket, _table}) do
    handle(command)
    :inet.setopts(socket, active: :once)
    {:noreply, state}
  end

  @impl true
  def handle_info({:tcp_closed, socket}, state = {_name, socket, _table}) do
    log("#{inspect(socket)} Closed")
    :gen_tcp.close(socket)
    {:stop, :normal, state}
  end

  @impl true
  def handle_info({:nck, new_name}, state = {name, socket, table}) do
    if name == "unregistered" do
      Chat.Server.set_nickname(new_name, self())
    else
      Chat.Server.update_nickname(name, new_name, self())
    end

    {:noreply, state}
  end

  @impl true
  def handle_info({:update_nickname, new_nickname}, state = {_name, socket, table}) do
    {:noreply, {new_nickname, socket, table}}
  end

  @impl true
  def handle_info({:msg, names, msg}, state = {name, _socket, table}) do
    if name == "unregistered" do
      log("Must be registered to send a message")
    else
      recipiants =
        Enum.reduce(names, [], fn name, acc ->
          cond do
            String.starts_with?(name, "#") ->
              case :ets.lookup(table, name) do
                [{_key, name_list}] ->
                  acc ++ name_list

                [] ->
                  acc
              end

            true ->
              acc ++ [name]
          end
        end)
        |> Enum.uniq()
        |> Enum.reject(&(&1 == name))

      log("Sending Message to #{inspect(recipiants)} message: #{inspect(msg)}")

      if Enum.count(recipiants) == 0 do
        log("No recipiants entered")
      else
        Enum.each(recipiants, fn to ->
          Chat.Server.send_message(to, name, msg)
        end)
      end
    end

    {:noreply, state}
  end

  @impl true
  def handle_info({:group_msg, group, msg}, state = {name, _socket, table}) do
    case :ets.lookup(table, group) do
      [{_group, names}] ->
        Enum.each(names, fn to ->
          Chat.Server.send_message(to, name, msg)
        end)

      [] ->
        log("#{group} is not a group")
    end

    {:noreply, state}
  end

  @impl true
  def handle_info({:grp, group, names}, state = {_name, _socket, table}) do
    :ets.insert(table, {group, names})

    log("Current Groups: #{inspect(:ets.tab2list(table))}")

    {:noreply, state}
  end

  @impl true
  def handle_info({:lst}, state) do
    Chat.Server.get_nicknames(self())
    {:noreply, state}
  end

  @impl true
  def handle_info({:incoming_msg, from, msg}, state = {_name, socket, _table}) do
    log(msg)
    :gen_tcp.send(socket, "#{from}: #{msg}\n")

    {:noreply, state}
  end

  @impl true
  def handle_info({:get_nicknames, names}, state = {_name, socket, _table}) do
    :gen_tcp.send(socket, "Users #{inspect(names)}\n")
    {:noreply, state}
  end

  @impl true
  def handle_info({:log, message}, state = {name, _socket, _table}) do
    Logger.info("[#{inspect(self())}] [Proxy] [#{name}] #{message}")
    {:noreply, state}
  end
end
