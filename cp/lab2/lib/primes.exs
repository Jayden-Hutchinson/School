# Sieve of Eratosthenes
# 1. create a list of numbers from 2 to n
# 2. Start with the first number in the list (x = 2)
# 3. Mark all multiples of x (except x itself) as non-prime
# 4. Move to the next unmarked number and repeat step 3
# 5. Continue until x^2 > n
# 6. The remaining unmakred numbers are all primes

defmodule Primes do
  def primes(n) do
    values = Enum.to_list(2..n)
    limit = :math.sqrt(n) |> floor()
    primes = sieve(values, limit)
    group = permutations(primes)

    IO.inspect(primes, label: "All Primes")
    IO.inspect(group, label: "Largest Permutation Set")
  end

  def permutations(primes) do
    six_digit_primes = Enum.filter(primes, fn x -> x >= 100_000 end)

    if six_digit_primes == [] do
      {:error, six_digit_primes}
    else
      six_digit_primes
      |> Enum.group_by(fn prime ->
        prime |> Integer.digits() |> Enum.sort()
      end)
      |> Enum.max_by(fn {_key, group} -> length(group) end)
      |> elem(1)
    end
  end

  defp sieve([prime | rest], limit) when prime > limit do
    [prime | rest]
  end

  defp sieve([prime | rest], limit) do
    filtered = Enum.filter(rest, fn x -> rem(x, prime) != 0 end)
    [prime | sieve(filtered, limit)]
  end
end

Primes.primes(200_000)
