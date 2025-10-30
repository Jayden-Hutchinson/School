defmodule Chat.Proxy do
  use GenServer

  require Logger

  @log_prefix "[#{inspect(self())}] [Proxy]"

  def start_link(socket) do
    Logger.info("#{@log_prefix} Started")
    GenServer.start_link(__MODULE__, socket)
  end

  @impl true
  def init(socket) do
    Logger.info("#{@log_prefix} Controlling #{inspect(socket)}")
    {:ok, socket}
  end

  @impl true
  def handle_info({:tcp, socket, data}, socket) do
    data = String.trim(data)

    validate(data)

    Logger.info("#{@log_prefix} #{data}")
    :inet.setopts(socket, active: :once)
    :gen_tcp.send(socket, data)
    {:noreply, socket}
  end

  @impl true
  def handle_info({:tcp_closed, socket}, socket) do
    Logger.info("#{@log_prefix} #{inspect(socket)} Closed")
    :gen_tcp.close(socket)
    {:stop, :normal, socket}
  end
end
