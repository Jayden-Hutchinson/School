defmodule HelloWeb.CounterJSON do
  def value(%{value: value}) do
    %{value: value}
  end
end
