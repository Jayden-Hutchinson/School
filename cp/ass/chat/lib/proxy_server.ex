defmodule Chat.ProxyServer do
  use DynamicSupervisor
  require Logger

  @log_prefix "[#{inspect(self())}] [Proxy Server] "

  def start_link(port \\ 6666) do
    Logger.info("#{@log_prefix} Started")
    DynamicSupervisor.start_link(__MODULE__, port, name: __MODULE__)
  end

  @impl true
  def init(port) do
    Task.start_link(fn -> listen(port) end)
    DynamicSupervisor.init(strategy: :one_for_one)
  end

  def listen(port) do
    opts = [:binary, active: :once, packet: :line, reuseaddr: true]

    case :gen_tcp.listen(port, opts) do
      {:ok, listen_socket} ->
        Logger.info("#{@log_prefix} listening on port #{inspect(port)}")
        accept_loop(listen_socket)

      {:error, reason} ->
        Logger.error("#{@log_prefix} #{inspect(reason)}")
    end
  end

  def accept_loop(listen_socket) do
    case :gen_tcp.accept(listen_socket) do
      # connection established
      {:ok, socket} ->
        Logger.info("#{@log_prefix} Client connected: #{inspect(socket)}")
        spec = {Chat.Proxy, socket}
        # start the proxy
        {:ok, pid} = DynamicSupervisor.start_child(__MODULE__, spec)
        # assign the proxy pid to the socket
        :gen_tcp.controlling_process(socket, pid)
        # loop to continue listening for connections
        accept_loop(listen_socket)

      # error connecting
      {:error, reason} ->
        Logger.error("#{@log_prefix} #{inspect(reason)}")
    end
  end
end
