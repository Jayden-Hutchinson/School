defmodule Counter.Worker do
  use GenServer

  def start_link(n \\ 0) do
    GenServer.start_link(__MODULE__, n, name: __MODULE__)
  end

  def inc(amt \\ 1) do
    GenServer.cast(__MODULE__, {:inc, amt})
  end

  def value() do
    GenServer.call(__MODULE__, :value)
  end

  @impl true
  def init(n) do
    {:ok, Counter.Store.get() || n}
  end

  @impl true
  def handle_call(:value, _from, state) do
    {:reply, state, state}
  end

  @impl true
  def handle_cast({:inc, amt}, state) do
    {:noreply, state + amt}
  end

  @impl true
  def terminate(_reason, state) do
    Counter.Store.put(state)
  end
end
