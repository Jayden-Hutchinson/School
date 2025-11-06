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

  def set_nickname(old, new, proxy_pid) do
    GenServer.cast(via(), {:set_nickname, {old, new, proxy_pid}})
  end

  def send_message(recipiant, from, message) do
    GenServer.cast(via(), {:send_message, {recipiant, from, message}})
  end

  def get_nicknames(proxy_pid) do
    GenServer.cast(via(), {:get_nicknames, proxy_pid})
  end

  def get_current_users() do
    GenServer.call(via(), :current_users)
  end

  @impl true
  def init(:ok) do
    nickname_table = :ets.new(Chat.Nicknames, [:set, :protected, :named_table])
    Logger.info("#{@log_prefix} Started")
    {:ok, nickname_table}
  end

  @impl true
  def handle_cast({:set_nickname, old, new, proxy_pid}, nickname_table) do
    # If exists in nickname table don't add it
    if :ets.member(nickname_table, new) do
      Logger.info("#{@log_prefix} User #{inspect(new)} already exists.")
    else
      # Add user to nickname table

      if(new == "unregistered") do
        # if not registered add the user
        :ets.insert(nickname_table, {new, proxy_pid})

        Logger.info("#{@log_prefix} New user: #{new} #{inspect(:ets.tab2list(nickname_table))}")
      else
        # if already registered update the nickname

        # delete the old nickname
        :ets.delete(nickname_table, old)

        # insert the new nickname
        :ets.insert(nickname_table, {new, proxy_pid})

        Logger.info(
          "#{@log_prefix} User Updated: #{old} -> #{new} #{inspect(:ets.tab2list(nickname_table))}"
        )
      end
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
  def handle_cast({:send_message, {recipiant, from, message}}, nickname_table) do
    case :ets.lookup(nickname_table, recipiant) do
      [{_nickname, proxy_pid}] -> send(proxy_pid, {:message, from, message})
      [] -> Logger.info("#{@log_prefix} #{recipiant} is not a registered user")
    end

    {:noreply, nickname_table}
  end

  @impl true
  def handle_call(:current_users, _from, nickname_table) do
    nicknames = :ets.match(nickname_table, {:"$1", :_}) |> List.flatten()
    {:reply, nicknames, nickname_table}
  end
end
