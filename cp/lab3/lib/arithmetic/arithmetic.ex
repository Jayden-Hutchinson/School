defmodule Arithmetic.Server do
  use GenServer

  def start(pool_size) do
    GenServer.start(__MODULE__, pool_size)
  end

  def init(pool_size) do
    workers =
      for _ <- 1..pool_size do
        {:ok, pid} = Arithmetic.Worker.start()
        pid
      end

    {:ok, workers}
  end
end

defmodule Arithmetic.Worker do
  use GenServer

  def start() do
    GenServer.start(__MODULE__, nil)
  end

  def square(pid, x) do
    GenServer.call(pid, {:square, x})
  end

  def sqrt(pid, x) do
    GenServer.call(pid, {:sqrt, x}, 4000)
  end

  # allows checking when compiling
  @impl true
  def init(arg) do
    {:ok, arg}
  end

  @impl true
  def handle_call({:square, x}, _from, state) do
    {:reply, {self(), x * x}, state}
  end

  @impl true
  def handle_call({:sqrt, x}, _from, state) do
    {:reply, if(x < 0, do: :error, else: {self(), :math.sqrt(x)}), state}
  end
end
