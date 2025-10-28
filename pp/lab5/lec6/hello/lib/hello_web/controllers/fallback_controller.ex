defmodule HelloWeb.FallbackController do
  use HelloWeb, :controller

  def call(conn, {:error, :bad_request}) do
    conn
    |> put_status(400)
    |> put_view(json: HelloWeb.ErrorJSON)
    |> render(:"400")
  end

end
