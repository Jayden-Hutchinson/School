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

  def update_nickname(old_nickname, new_nickname, proxy_pid) do
    GenServer.cast(via(), {:update_nickname, {old_nickname, new_nickname, proxy_pid}})
  end

  def send_message(to, from, message) do
    GenServer.cast(via(), {:send_message, {to, from, message}})
  end

  def get_nicknames(proxy_pid) do
    GenServer.cast(via(), {:get_nicknames, proxy_pid})
  end

  @impl true
  def init(:ok) do
    nickname_table = :ets.new(Chat.Nicknames, [:set, :protected, :named_table])
    Logger.info("#{@log_prefix} Started")
    {:ok, nickname_table}
  end

  @impl true
  def handle_cast({:set_nickname, {nickname, proxy_pid}}, nickname_table) do
    if :ets.member(nickname_table, nickname) do
      Logger.info("#{@log_prefix} User #{inspect(nickname)} already exists.")
    else
      :ets.insert(nickname_table, {nickname, proxy_pid})

      Logger.info(
        "#{@log_prefix} New user: #{nickname} #{inspect(:ets.tab2list(nickname_table))}"
      )

      send(proxy_pid, {:update_nickname, nickname})
    end

    Logger.info("#{@log_prefix} #{inspect(:ets.tab2list(nickname_table))}")

    {:noreply, nickname_table}
  end

  @impl true
  def handle_cast({:update_nickname, {old_nickname, new_nickname, proxy_pid}}, nickname_table) do
    if :ets.member(nickname_table, new_nickname) do
      Logger.info("#{@log_prefix} User #{inspect(new_nickname)} already exists.")
    else
      :ets.delete(nickname_table, old_nickname)
      :ets.insert(nickname_table, {new_nickname, proxy_pid})

      Logger.info(
        "#{@log_prefix} User Updated: #{old_nickname} -> #{new_nickname} #{inspect(:ets.tab2list(nickname_table))}"
      )

      send(proxy_pid, {:update_nickname, new_nickname})
    end

    Logger.info("#{@log_prefix} #{inspect(:ets.tab2list(nickname_table))}")

    {:noreply, nickname_table}
  end

  @impl true
  def handle_cast({:send_message, {to, from, message}}, nickname_table) do
    case :ets.lookup(nickname_table, to) do
      [{_nickname, proxy_pid}] -> send(proxy_pid, {:incoming_msg, from, message})
      [] -> Logger.info("#{@log_prefix} #{to} is not a registered user")
    end

    {:noreply, nickname_table}
  end

  @impl true
  def handle_cast({:get_nicknames, proxy_pid}, nickname_table) do
    nicknames = :ets.match(nickname_table, {:"$1", :_}) |> List.flatten()

    send(proxy_pid, {:get_nicknames, nicknames})

    {:noreply, nickname_table}
  end
end
