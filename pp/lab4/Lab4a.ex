defmodule Lab4a do
  @filename "atomic-weights.txt"

  defmacro define_functions() do
    elements =
      File.read!(@filename)
      |> String.split("\n", trim: true)
      |> Enum.map(fn line ->
        [_, symbol_str, _, weight_str] = String.split(line, "\t", trim: true)
        weight = weight_str |> String.to_float()
        symbol = symbol_str |> String.downcase() |> String.to_atom()
        {symbol, weight}
      end)

    quote do
      (unquote_splicing(
         Enum.map(elements, fn {symbol, weight} ->
           quote do
             def unquote(symbol)() do
               unquote(weight)
             end
           end
         end)
       ))
    end
  end
end

defmodule Lab4a.Tests do
  require Lab4a
  Lab4a.define_functions()
end

IO.puts(Lab4a.Tests.h())
