defmodule Lec1 do
  def fact(n) do
    if n <= 0 do
      1
    else
      n * fact(n - 1)
    end
  end

  defp fact2(0, acc) do acc end
  defp fact2(n, acc) do fact2(n - 1, n * acc) end

  def fact2(n) do fact2(n, 1) end

  def fact3(n) do
    # same as:
    # if(n <= 0, [do: 1, else: n * fact3(n - 1)])
    if n <= 0, do: 1, else: n * fact3(n - 1)
  end

  def dedup(l) do
    case l do
      [h1, x = [h2|_]] ->
        if h1 == h2, do: dedup(x), else: [h1|dedup(x)]
      _ -> l
    end
  end

  def dedup2(l) do
    case l do
      [h, h | t] -> dedup2([h|t])
      [h | t] -> [h | dedup2(t)]
      _ -> l
    end
  end

  defp print(n) do
    cond do
      rem(n, 15) == 0 -> IO.puts("fizzbuzz")
      rem(n, 5) == 0 -> IO.puts("buzz")
      rem(n, 3) == 0 -> IO.puts("fizz")
      true -> IO.puts(n)
    end
  end

  defp fizzbuzz(i, n) when i > n, do: :ok
  defp fizzbuzz(i, n) do
    print(i)
    fizzbuzz(i + 1, n)
  end

  def fizzbuzz(n), do: fizzbuzz(1, n)

  def fizzbuzz2(n), do: Enum.each(1..n, &print/1)
end
