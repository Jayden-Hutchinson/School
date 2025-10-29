IO.puts("Server starting...")

server = Chat.ProxyServer.start()

case server do
  {:ok, pid} ->
    IO.puts("ProxyServer PID: #{inspect(pid)}")

  {:error, reason} ->
    IO.puts(reason)
end
