defmodule Arithmetic.Server do
  use GenServer

  def start(pool_size) do
    GenServer.start_link(__MODULE__, pool_size, name: __MODULE__)
  end

  ## API ##

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
    workers =
      for _ <- 1..pool_size do
        {:ok, pid} = Arithmetic.Worker.start()
        pid
      end

    {:ok, {workers, 0}}
  end

  def handle_call(:get_worker, _from, {workers, index}) do
    {pid, next_index} = pick_worker(workers, index)
    {:reply, pid, {workers, next_index}}
  end

  defp pick_worker(workers, index) do
    worker = Enum.at(workers, index)
    next_index = rem(index + 1, length(workers))
    {worker, next_index}
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
    GenServer.call(pid, {:sqrt, x})
  end

  # allows checking when compiling
  @impl true
  def init(arg) do
    IO.inspect(self())
    {:ok, arg}
  end

  @impl true
  def handle_call({:square, x}, _from, state) do
    current_time = Calendar.strftime(NaiveDateTime.local_now(), "%I:%M:%S %p")

    result = {self(), x * x}

    IO.puts("#{current_time} Square of #{x}: #{inspect(result)}")

    {:reply, result, state}
  end

  @impl true
  def handle_call({:sqrt, x}, _from, state) do
    current_time = Calendar.strftime(NaiveDateTime.local_now(), "%I:%M:%S %p")

    Process.sleep(4000)

    result =
      if x < 0, do: :error, else: {self(), :math.sqrt(x)}

    IO.puts("#{current_time} Square Root of #{x}: #{inspect(result)}")

    {:reply, result, state}
  end
end
