defmodule HelloWeb.HelloController do
  use HelloWeb, :controller

  def hello(conn, _params) do
    render(conn, :hello) 
  end

  def msg(conn, %{"messenger" => messenger}) do
    conn
    |> assign(:messenger, messenger)
    |> assign(:message, "world")
    |> render(:msg)

    # OR
    # render(conn, :msg, messenger: messenger)
  end
end
