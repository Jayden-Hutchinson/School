defmodule Counter.Server do
  use GenServer

  def start(n \\ 0) do
    GenServer.start(__MODULE__, n)
  end

  def inc(pid, amt \\ 1) do
    GenServer.cast(pid, {:inc, amt})
  end

  def value(pid) do
    GenServer.call(pid, :value)
  end

  @impl true
  def init(arg) do
    {:ok, arg}
  end

  @impl true
  def handle_call(:value, _from, state) do
    {:reply, state, state}
  end

  @impl true
  def handle_cast({:inc, amt}, state) do
    {:noreply, state + amt}
  end
end
