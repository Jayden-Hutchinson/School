defmodule Lab4b do
  @filename "atomic-weights.txt"

  def atomic_weight(target) do
    File.read!(@filename)
    |> String.split("\n", trim: true)
    |> Enum.find_value(fn line ->
      case String.split(line, "\t", trim: true) do
        [_, symbol, _, weight_str] when symbol == target ->
          String.to_float(weight_str)

        _ ->
          nil
      end
    end)
  end
end

h = Lab4b.atomic_weight("H")
he = Lab4b.atomic_weight("He")
ne = Lab4b.atomic_weight("Ne")
IO.puts(h)
IO.puts(he)
IO.puts(ne)
