defmodule Arithmetic.Worker do
  use GenServer

  def start() do
    GenServer.start(__MODULE__, nil)
  end

  def square(pid, x) do
    GenServer.call(pid, {:square, x})
  end

  def sqrt(pid, x) do
    GenServer.call(pid, {:sqrt, x})
  end

  @impl true  # allows checking when compiling
  def init(arg) do
    {:ok, arg}
  end

  @impl true
  def handle_call({:square, x}, _from, state) do
    {:reply, x * x, state}
  end

  @impl true
  def handle_call({:sqrt, x}, _from, state) do
    {:reply, (if x < 0, do: :error, else: :math.sqrt(x)), state}
  end
end



