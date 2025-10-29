require Logger
IO.puts("Server starting...")

server = Chat.ProxyServer.start()

case server do
  {:ok, pid} ->
    Logger.info("Chat.ProxyServer started #{inspect(pid)}")

  {:error, reason} ->
    Logger.error(reason)
end
