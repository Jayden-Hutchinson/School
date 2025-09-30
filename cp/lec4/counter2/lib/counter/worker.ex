defmodule Counter.Worker do
  use GenServer

  def start_link(name) do
    GenServer.start_link(__MODULE__, nil, name: name)
  end

  def inc(name, amt \\ 1) do
    GenServer.cast(name, {:inc, amt})
  end

  def value(name) do
    GenServer.call(name, :value)
  end

  @impl true
  def init(_) do
    {:ok, 0}
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
