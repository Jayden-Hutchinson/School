defmodule Chat.Server do
  use GenServer

  require Logger

  # for logging the ets table in the console
  # Logger.info(inspect(:ets.tab2list(table)))

  @log_prefix "[#{inspect(self())}] [Server]"

  def via() do
    {:global, __MODULE__}
  end

  def start_link(_) do
    GenServer.start_link(__MODULE__, :ok, name: via())
  end

  def set_nickname(nickname, proxy_pid) do
    GenServer.cast(via(), {:set_nickname, {nickname, proxy_pid}})
  end

  def send_message(nickname, message) do
    GenServer.cast(via(), {:send_message, {nickname, message}})
  end

  @impl true
  def init(:ok) do
    table = :ets.new(Chat.Nicknames, [:set, :protected, :named_table])
    Logger.info("#{@log_prefix} Started")
    {:ok, table}
  end

  @impl true
  def handle_cast({:set_nickname, {nickname, proxy_pid}}, table) do
    :ets.insert(table, {nickname, proxy_pid})
    Logger.info("#{@log_prefix} Inserted new nickname: #{nickname}")
    Logger.info("#{@log_prefix} Current Users: #{inspect(:ets.tab2list(table))}")
    {:noreply, table}
  end

  @impl true
  def handle_cast({:send_message, {nickname, message}}, table) do
    case :ets.lookup(table, nickname) do
      [{_nickname, proxy_pid}] -> send(proxy_pid, {:message, message})
    end

    {:noreply, table}
  end
end
