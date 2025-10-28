defmodule Counter.Worker do
  use GenServer
  @store  Counter.Store

  def start_link(name) do
    GenServer.start_link(__MODULE__, name, name: via(name))
  end

  def via(name) do
    {:via, Registry, {Counter.Registry, {__MODULE__, name}}}
  end

  def inc(name, amt \\ 1) do
    GenServer.cast(via(name), {:inc, amt})
  end

  def value(name) do
    GenServer.call(via(name), :value)
  end

  @impl true
  def init(name) do
    # Process.sleep(5000)
    IO.puts("Counter.Worker starting ... #{name}(#{inspect(self())})")
    name = {__MODULE__, name}
    value = 
      case :ets.lookup(@store, name) do
        [] -> 0
        [{_, x}] -> x
      end
    {:ok, {name, value}}
  end

  @impl true
  def handle_cast({:inc, amt}, {name, value}) when is_integer(amt) do
    {:noreply, {name, value + amt}}
  end

  @impl true
  def handle_call(:value, _from, {_, value} = state) do
    {:reply, value, state}
  end
 
  @impl true
  def terminate(_reason, state) do
    :ets.insert(@store, state)
  end
end
