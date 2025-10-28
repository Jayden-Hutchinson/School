defmodule Counter.Worker do
  def start_link(name) do
    GenServer.start_link(__MODULE__, name, name: via(name))
  end

  def via(name) do
    {:global, name}
    # {:via, :global, name}
  end

  def inc(name, amt \\ 1) do
    GenServer.cast(via(name), {:inc, amt})
  end

  def value(name) do
    GenServer.call(via(name), :value)
  end
end
