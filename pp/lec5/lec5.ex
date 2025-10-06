defmodule Lec5 do
  def if_fn(condition, do_block, else_block) do
    cond do
      condition -> do_block
      true -> else_block
    end
  end

  defmacro inspect(expr) do
    IO.inspect(expr)
  end

  defmacro if_macro(condition, do_block, else_block) do
    quote do
      cond do
        unquote(condition) -> unquote(do_block)
        true -> unquote(else_block)
      end
    end
  end

  defmacro if(condition, do: do_block, else: else_block) do
    quote do
      cond do
        unquote(condition) -> unquote(do_block)
        true -> unquote(else_block)
      end
    end
  end

  defmacro if(condition, do: do_block) do
    quote do
      cond do
        unquote(condition) -> unquote(do_block)
        true -> nil
      end
    end
  end

  defmacro while(condition, do: block) do
    quote do
      try do
        for _ <- Stream.cycle([:ok]) do
          cond do
            unquote(condition) -> unquote(block)
            true -> throw(:break)
          end
        end
      catch
        :throw, :break -> :ok
      end
    end
  end

  def break, do: throw(:break)

  defmacro context() do
    # macro's context
    IO.puts("macro's context: #{__MODULE__}")

    quote do
      # caller's context
      IO.puts("caller's context: #{__MODULE__}")
    end
  end

  defmacro add_say_fn(msg) do
    name = String.to_atom("say_" <> msg)

    quote do
      def unquote(name)() do
        IO.puts(unquote(msg))
      end
    end
  end
end

defmodule Lec5.Tests do
  require Lec5

  IO.puts("Lec5.Tests")

  def test_while() do
    pid = spawn(fn -> Process.sleep(20_000) end)

    Lec5.while Process.alive?(pid) do
      IO.puts("*")
      Process.sleep(1000)
    end
  end

  #  def test_context() do
  Lec5.context()
  #  end

  Lec5.add_say_fn("goodbye")
  Lec5.add_say_fn("hell")
end

# unquote fragment
defmodule Example do
  for {k, v} <- [homer: 25, bart: 66] do
    # def is a macro, argument to def is a quoted expression & hence can use
    # unquote
    def unquote(k)(), do: unquote(v)
  end
end
