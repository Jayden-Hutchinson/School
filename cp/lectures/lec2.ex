defmodule ArithmeticServer do
  def start() do
    spawn(&loop/0)
  end

  def square(pid, x) do
    send(pid, {:square, x, self()})

    receive do
      x -> x
    end
  end

  def sqrt(pid, x) do
    send(pid, {:sqrt, x, self()})

    receive do
      {^pid, x} -> x
    end
  end

  defp loop() do
    receive do
      {:square, x, from} ->
        send(from, x * x)

      {:sqrt, x, from} ->
        response =
          if x < 0, do: :error, else: :math.sqrt(x)

        send(from, {self(), response})

      # throw away extraneous messages
      _ ->
        :ok
    end

    loop()
  end
end

defmodule CounterServer do
  def start() do
    spawn(fn -> loop(0) end)
  end

  def inc(pid) do
    send(pid, :inc)
  end

  def value(pid) do
    send(pid, {:value, self()})

    receive do
      x -> x
    end
  end

  defp loop(n) do
    receive do
      :inc ->
        loop(n + 1)

      {:value, from} ->
        send(from, n)
        loop(n)
    end
  end
end

defmodule RegisteredCounterServer do
  def start(n \\ 0) do
    Process.register(spawn(fn -> loop(n) end), __MODULE__)
  end

  def inc() do
    send(__MODULE__, :inc)
  end

  def value() do
    send(__MODULE__, {:value, self()})

    receive do
      x -> x
    end
  end

  defp loop(n) do
    receive do
      :inc ->
        loop(n + 1)

      {:value, from} ->
        send(from, n)
        loop(n)
    end
  end
end

defmodule M do
  def f(a \\ 1, b, c \\ 2, d)

  def f(0, b, c, d), do: {b, c, d}
  def f(a, b, c, d), do: {a, b, c, d}
end
