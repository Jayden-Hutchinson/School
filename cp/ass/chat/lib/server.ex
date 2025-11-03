defmodule Chat.Server do
  use GenServer

  require Logger

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
    nickname_table = :ets.new(Chat.Nicknames, [:set, :protected, :named_table])
    Logger.info("#{@log_prefix} Started")
    {:ok, nickname_table}
  end

  @impl true
  def handle_cast({:set_nickname, {nickname, proxy_pid}}, nickname_table) do
    :ets.insert(nickname_table, {nickname, proxy_pid})
    Logger.info("#{@log_prefix} Inserted new nickname: #{nickname}")
    Logger.info("#{@log_prefix} Current Users: #{inspect(:ets.tab2list(nickname_table))}")
    {:noreply, nickname_table}
  end

  @impl true
  def handle_cast({:send_message, {nickname, message}}, nickname_table) do
    Logger.info("#{@log_prefix} To #{nickname}: #{message}")

    case :ets.lookup(nickname_table, nickname) do
      [{_nickname, proxy_pid}] -> send(proxy_pid, {:message, message})
      [] -> Logger.info("#{@log_prefix} #{nickname} is not a registered user")
    end

    {:noreply, nickname_table}
  end
end
