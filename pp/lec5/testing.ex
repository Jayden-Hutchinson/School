defmodule Testing do
  defmacro check(predicate) do
    name = Macro.to_string(predicate)

    quote do
      cond do
        unquote(predicate) -> IO.puts("passed ... #{unquote(name)}")
        true -> IO.puts("FAILED ... #{unquote(name)}")
      end
    end
  end

  defmacro test(name, do: block) do
    title = "test_" <> name
    name = String.to_atom(title)

    quote do
      @tests [unquote(name) | @tests]
      def unquote(name)() do
        IO.puts(unquote(title) <> ":")
        unquote(block)
      end
    end
  end

  defmacro __using__(_opts) do
    quote do
      import unquote(__MODULE__)
      @tests []
    end
  end

  defmacro make_run() do
    quote do
      def run() do
        Enum.each(Enum.reverse(@tests), fn f -> apply(__MODULE__, f, []) end)
      end
    end
  end
end

defmodule Testing.Tests do
  use Testing

  test "equality" do
    check(1 + 1 == 2)
    check(1 - 1 == 1)
  end

  test "inequality" do
    check(1 + 1 != 1)
  end

  make_run()
end

Testing.Tests.run()
