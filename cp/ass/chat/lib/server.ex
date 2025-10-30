defmodule Chat.Server do
  use GenServer

  require Logger

  # for logging the ets table in the console
  # Logger.info(inspect(:ets.tab2list(nicknames)))

  @log_prefix "[#{inspect(self())}] [Server]"

  def via() do
    {:global, __MODULE__}
  end

  def start_link(_) do
    GenServer.start_link(__MODULE__, :ok, name: via())
  end

  @impl true
  def init(:ok) do
    nicknames = :ets.new(Chat.Nicknames, [:set, :protected, :named_table])
    Logger.info(inspect(:ets.tab2list(nicknames)))
    Logger.info("#{@log_prefix} Started")
    {:ok, nicknames}
  end
end
