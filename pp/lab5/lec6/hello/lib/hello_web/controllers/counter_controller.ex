defmodule HelloWeb.CounterController do
  use HelloWeb, :controller

  action_fallback HelloWeb.FallbackController

  def home(conn, _params) do
    redirect(conn, to: ~p"/counter/value")
  end

  def value(conn, _params) do
    value = Counter.Worker.value()
    render(conn, :value, value: value)
  end

  def inc(conn, %{"amt" => amt}) when is_integer(amt) do
    Counter.Worker.inc(amt)
    json(conn, %{status: :ok})
  end

  def inc(conn, %{"amt" => amt}) do
    case Integer.parse(amt) do
      {n, ""} ->
        Counter.Worker.inc(n)
        json(conn, %{status: :ok})
      _ ->
        #json(conn, %{error: "not an integer"})
        {:error, :bad_request}
    end
  end
end
