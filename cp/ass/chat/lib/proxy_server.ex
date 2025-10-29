defmodule Chat.ProxyServer do
  use GenServer

  def start(port \\ 6666) do
    GenServer.start(__MODULE__, port)
  end

  @impl true
  def init(port) do
    opts = [:binary, active: :once, packet: :line, reuseaddr: true]

    case :gen_tcp.listen(port, opts) do
      {:ok, listen_socket} ->
        send(self(), :accept)
        {:ok, listen_socket}

      {:error, reason} ->
        {:stop, reason}
    end
  end

  @impl true
  def handle_info(:accept, listen_socket) do
    case :gen_tcp.accept(listen_socket) do
      # connection established
      {:ok, socket} ->
        # start the proxy
        {:ok, pid} = Chat.Proxy.start_link(socket)

        # assign the proxy pid to the socket
        :gen_tcp.controlling_process(socket, pid)

        # loop to continue listening for connections
        send(self(), :accept)

        # continue running on the socket
        {:noreply, listen_socket}

      # error connecting
      {:error, reason} ->
        {:stop, reason, listen_socket}
    end
  end
end
