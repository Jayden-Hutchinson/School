defmodule Chat.Proxy do
  use GenServer
  require Logger

  def log_prefix() do
    "[#{inspect(self())}] [Proxy]"
  end

  def start_link(socket) do
    Logger.info("#{log_prefix()} Started")
    GenServer.start_link(__MODULE__, socket)
  end

  def handle("/NCK" <> rest) do
    args = rest |> String.trim() |> String.split()

    Logger.info("#{log_prefix()} [/NCK] Input: #{inspect(args)}")

    case args do
      [nick | _] -> send(self(), {:nck, nick})
      [] -> Logger.info("invalid input")
    end
  end

  def handle("/MSG" <> rest) do
    Logger.info("#{log_prefix()} #{inspect(rest)}")

    if String.starts_with?(rest, "#") do
      [group, msg] = rest |> String.split(" ", parts: 2, trim: true)

      Logger.info("#{log_prefix()} Group: #{inspect(group)} msg: #{inspect(msg)}")
      send(self(), {:msg_group, group, msg})
    else
      [recipiants, msg] =
        rest |> String.trim() |> String.split(" ", parts: 2, trim: true)

      recipiant_list = recipiants |> String.split(",")

      Logger.info(
        "#{log_prefix()} [/MSG] Recipiants: #{inspect(recipiant_list)} msg: #{inspect(msg)}"
      )

      send(self(), {:msg, recipiant_list, msg})
    end
  end

  def handle("/GRP" <> rest) do
    args = rest |> String.trim() |> String.split(",", trim: true) |> Enum.map(&String.trim/1)
    Logger.info("#{log_prefix()} #{inspect(args)}")

    [first | rest] = args
    [group, nick] = first |> String.trim() |> String.split(" ", trim: true)
    nicks = rest ++ [nick]

    unless String.starts_with?(group, "#") do
      Logger.info("#{log_prefix()} Group name must start with \'#\'")
      :ok
    else
      Logger.info("#{log_prefix()} #{inspect(group)}")
      Logger.info("#{log_prefix()} #{inspect(nicks)}")

      send(self(), {:group, group, nicks})
    end
  end

  def handle(_) do
    Logger.info("#{log_prefix()} Invalid Command")
  end

  @impl true
  def init(socket) do
    Logger.info("#{log_prefix()} Controlling #{inspect(socket)}")
    table = :ets.new(:groups, [:set, :protected])
    nick = "unregistered"
    {:ok, {nick, socket, table}}
  end

  @impl true
  def handle_info({:tcp, socket, input}, {nick, socket, table}) do
    handle(input)
    :inet.setopts(socket, active: :once)
    :gen_tcp.send(socket, input)
    {:noreply, {nick, socket, table}}
  end

  @impl true
  def handle_info({:tcp_closed, socket}, {nick, socket, table}) do
    Logger.info("#{log_prefix()} #{inspect(socket)} Closed")
    :gen_tcp.close(socket)
    {:stop, :normal, {socket, table}}
  end

  @impl true
  def handle_info({:nck, new_nick}, {nick, socket, table}) do
    Chat.Server.set_nickname(new_nick, self())

    {:noreply, {nick, socket, table}}
  end

  @impl true
  def handle_info({:msg, recipiants, msg}, {nick, socket, table}) do
    Logger.info("#{log_prefix()} #{msg}")

    Enum.each(recipiants, fn to ->
      Chat.Server.send_message(to, nick, msg)
    end)

    {:noreply, {nick, socket, table}}
  end

  @impl true
  def handle_info({:msg_group, group, msg}, {nick, socket, table}) do
    case :ets.lookup(table, group) do
      [{_group, nicks}] ->
        Enum.each(nicks, fn to ->
          Chat.Server.send_message(to, nick, msg)
        end)

      [] ->
        Logger.info("#{log_prefix()} #{group} is not a group")
    end

    {:noreply, {nick, socket, table}}
  end

  @impl true
  def handle_info({:grp, group, nicks}, {nick, socket, table}) do
    :ets.insert(table, {group, nicks})
    Logger.info("#{log_prefix()} Current Groups: #{inspect(:ets.tab2list(table))}")
    {:noreply, {nick, socket, table}}
  end

  # @impl true
  # def handle_info({:lst}) do
  # end

  @impl true
  def handle_info({:incoming_msg, from, msg}, {nick, socket, table}) do
    :gen_tcp.send(socket, "#{from}: #{msg}")
    {:noreply, {nick, socket, table}}
  end
end
