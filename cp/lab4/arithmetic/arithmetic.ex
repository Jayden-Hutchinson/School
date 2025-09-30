defmodule Arithmetic.Server do
  use GenServer

  ## API ##
  def start(pool_size) do
    GenServer.start_link(__MODULE__, pool_size, name: __MODULE__)
  end

  def square(x) do
    worker_pid = GenServer.call(__MODULE__, :get_worker)
    Arithmetic.Worker.square(worker_pid, x)
  end

  def sqrt(x) do
    worker_pid = GenServer.call(__MODULE__, :get_worker)
    Arithmetic.Worker.sqrt(worker_pid, x)
  end

  ## Server ##
  def init(pool_size) do
    # Trap exits so we receive :EXIT when a linked worker dies
    Process.flag(:trap_exit, true)

    workers =
      for _ <- 1..pool_size do
        {:ok, pid} = Arithmetic.Worker.start()
        Process.link(pid)
        pid
      end

    Enum.each(workers, fn pid ->
      IO.puts(inspect(pid))
    end)

    {:ok, {workers, 0}}
  end

  def handle_call(:get_worker, _from, {workers, index}) do
    # Pick a worker in round-robin fashion
    {pid, next_index} = pick_worker(workers, index)
    {:reply, pid, {workers, next_index}}
  end

  # Handle worker crashes
  def handle_info({:EXIT, pid, _reason}, {workers, index}) do
    IO.puts("Worker #{inspect(pid)} crashed. Starting a new one...")

    # Remove dead worker
    workers = Enum.reject(workers, fn w -> w == pid end)

    # Start and link a new worker
    {:ok, new_pid} = Arithmetic.Worker.start()
    Process.link(new_pid)
    workers = workers ++ [new_pid]

    {:noreply, {workers, index}}
  end

  defp pick_worker(workers, index) do
    worker = Enum.at(workers, rem(index, length(workers)))
    next_index = rem(index + 1, length(workers))
    {worker, next_index}
  end
end

defmodule Arithmetic.Worker do
  use GenServer

  def start() do
    GenServer.start(__MODULE__, nil)
  end

  def square(pid, x), do: GenServer.call(pid, {:square, x})
  def sqrt(pid, x), do: GenServer.call(pid, {:sqrt, x})

  @impl true
  def init(arg) do
    {:ok, arg}
  end

  @impl true
  def handle_call({:square, x}, _from, state) do
    result = {self(), x * x}
    IO.puts("Square of #{x}: #{inspect(result)}")
    {:reply, result, state}
  end

  @impl true
  def handle_call({:sqrt, x}, _from, state) do
    result = if x < 0, do: :error, else: {self(), :math.sqrt(x)}
    IO.puts("Square Root of #{x}: #{inspect(result)}")
    {:reply, result, state}
  end
end
