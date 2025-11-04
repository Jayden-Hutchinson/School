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

    log("/NCK: #{inspect(args)}")

    case args do
      [nickname | _] -> send(self(), {:nck, nickname})
      [] -> Logger.info("invalid input")
    end
  end

  def handle("/MSG" <> rest) do
    log("#{inspect(rest)}")

    if String.starts_with?(rest, "#") do
      [group, msg] = rest |> String.split(" ", parts: 2, trim: true)

      log("Group: #{inspect(group)} message: #{inspect(msg)}")
      send(self(), {:msg_group, group, msg})
    else
      [recipiants, msg] =
        rest |> String.trim() |> String.split(" ", parts: 2, trim: true)

      recipiant_list = recipiants |> String.split(",")

      log("/MSG: Recipiants: #{inspect(recipiant_list)} message: #{inspect(msg)}")

      send(self(), {:msg, recipiant_list, msg})
    end
  end

  def handle("/GRP" <> rest) do
    args = rest |> String.trim() |> String.split(",", trim: true) |> Enum.map(&String.trim/1)
    log("#{inspect(args)}")

    [first | rest] = args
    [group, nickname] = first |> String.trim() |> String.split(" ", trim: true)
    nicknames = rest ++ [nickname]

    unless String.starts_with?(group, "#") do
      log("Group name must start with \'#\'")
      :ok
    else
      log("#{inspect(group)}")
      log("#{inspect(nicknames)}")

      send(self(), {:group, group, nicknames})
    end
  end

  def handle(_) do
    log("Invalid Command")
  end

  @impl true
  def init(socket) do
    table = :ets.new(:groups, [:set, :protected])
    nickname = "unregistered"
    state = {nickname, socket, table}

    log("Started - Controlling #{inspect(socket)}")

    {:ok, state}
  end

  @impl true
  def handle_info({:tcp, socket, input}, state = {_nickname, socket, _table}) do
    handle(input)
    :inet.setopts(socket, active: :once)
    :gen_tcp.send(socket, input)
    {:noreply, state}
  end

  @impl true
  def handle_info({:tcp_closed, socket}, state = {_nickname, socket, _table}) do
    log("#{inspect(socket)} Closed")
    :gen_tcp.close(socket)
    {:stop, :normal, state}
  end

  @impl true
  def handle_info({:nck, new_nickname}, state = {_nickname, socket, table}) do
    Chat.Server.set_nickname(new_nickname, self())

    {:noreply, {new_nickname, socket, table}}
  end

  @impl true
  def handle_info({:msg, recipiants, msg}, state = {nickname, _socket, _table}) do
    log(msg)

    Enum.each(recipiants, fn to ->
      Chat.Server.send_message(to, nickname, msg)
    end)

    {:noreply, state}
  end

  @impl true
  def handle_info({:msg_group, group, msg}, state = {nickname, _socket, table}) do
    case :ets.lookup(table, group) do
      [{_group, nicknames}] ->
        Enum.each(nicknames, fn to ->
          Chat.Server.send_message(to, nickname, msg)
        end)

      [] ->
        log("#{group} is not a group")
    end

    {:noreply, state}
  end

  @impl true
  def handle_info({:grp, group, nicknames}, state = {_nickname, _socket, table}) do
    :ets.insert(table, {group, nicknames})

    log("Current Groups: #{inspect(:ets.tab2list(table))}")

    {:noreply, state}
  end

  # @impl true
  # def handle_info({:lst}) do
  # end

  @impl true
  def handle_info({:incoming_msg, from, msg}, state = {_nickname, socket, _table}) do
    :gen_tcp.send(socket, "#{from}: #{msg}")
    {:noreply, state}
  end

  @impl true
  def handle_info({:log, message}, state = {nickname, _socket, _table}) do
    Logger.info("[#{inspect(self())}] [Proxy] [#{nickname}] #{message}")
    {:noreply, state}
  end
end
