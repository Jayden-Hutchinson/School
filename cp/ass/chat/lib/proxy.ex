defmodule Chat.Proxy do
  use GenServer
  require Logger

  @log_prefix "[#{inspect(self())}] [Proxy]"

  def start_link(socket) do
    Logger.info("#{@log_prefix} Started")
    GenServer.start_link(__MODULE__, socket)
  end

  def handle("/NCK" <> rest) do
    args = rest |> String.trim() |> String.split()

    Logger.info("#{@log_prefix} [/NCK] Input: #{inspect(args)}")

    case args do
      [nickname | _] -> Chat.Server.set_nickname(nickname, self())
      [] -> Logger.info("invalid input")
    end
  end

  def handle("/MSG" <> rest) do
    args = rest |> String.trim() |> String.split(",", trim: true) |> Enum.map(&String.trim/1)

    Logger.info("#{@log_prefix} [/MSG] #{inspect(args)}")

    {nicknames, [last]} = Enum.split(args, -1)
    [nickname | message_parts] = String.split(last, " ", trim: true)
    all_nicknames = nicknames ++ [nickname]
    message = message_parts |> Enum.join(" ")

    [first | rest] = all_nicknames

    if String.starts_with?(first, "#") do
      send(self(), {:group_msg, first, message})
    else
      Enum.each(all_nicknames, fn nickname ->
        Chat.Server.send_message(nickname, message)
      end)
    end
  end

  def handle("/GRP" <> rest) do
    args = rest |> String.trim() |> String.split(",", trim: true) |> Enum.map(&String.trim/1)
    Logger.info("#{@log_prefix} #{inspect(args)}")

    [first | rest] = args
    [group_name, nickname] = first |> String.trim() |> String.split(" ", trim: true)
    nicknames = rest ++ [nickname]

    unless String.starts_with?(group_name, "#") do
      Logger.info("#{@log_prefix} Group name must start with \'#\'")
      :ok
    else
      Logger.info("#{@log_prefix} #{inspect(group_name)}")
      Logger.info("#{@log_prefix} #{inspect(nicknames)}")

      send(self(), {:group, group_name, nicknames})
    end
  end

  def handle(_) do
    Logger.info("#{@log_prefix} Invalid Command")
  end

  @impl true
  def init(socket) do
    Logger.info("#{@log_prefix} Controlling #{inspect(socket)}")
    group_table = :ets.new(Chat.Groups, [:set, :protected, :named_table])
    {:ok, {socket, group_table}}
  end

  @impl true
  def handle_info({:tcp, socket, input}, {socket, group_table}) do
    handle(input)
    :inet.setopts(socket, active: :once)
    :gen_tcp.send(socket, input)
    {:noreply, {socket, group_table}}
  end

  @impl true
  def handle_info({:tcp_closed, socket}, {socket, group_table}) do
    Logger.info("#{@log_prefix} #{inspect(socket)} Closed")
    :gen_tcp.close(socket)
    {:stop, :normal, {socket, group_table}}
  end

  @impl true
  def handle_info({:message, message}, {socket, group_table}) do
    Logger.info("#{@log_prefix} #{message}")
    {:noreply, {socket, group_table}}
  end

  @impl true
  def handle_info({:group, group_name, nicknames}, {socket, group_table}) do
    :ets.insert(group_table, {group_name, nicknames})
    Logger.info("#{@log_prefix} Current Groups: #{inspect(:ets.tab2list(group_table))}")
    {:noreply, {socket, group_table}}
  end

  def handle_info({:group_msg, group_name, message}, {socket, group_table}) do
    case :ets.lookup(group_table, group_name) do
      [{_group_name, nicknames}] ->
        Enum.each(nicknames, fn nickname ->
          Chat.Server.send_message(nickname, message)
        end)

      [] ->
        Logger.info("#{@log_prefix} #{group_name} is not a group")
    end

    {:noreply, {socket, group_table}}
  end
end
