defmodule Chat.Proxy do
  use GenServer

  require Logger

  @log_prefix "[#{inspect(self())}] [Proxy]"

  def nck(input) do
    Logger.info("#{@log_prefix} [/NCK] #{inspect(input)}")

    # args = String.split(input)
  end

  def handle("/NCK" <> rest) do
    args = rest |> String.trim() |> String.split()

    Logger.info("#{@log_prefix} [/NCK] Input: #{inspect(args)}")

    case args do
      [nickname | _] -> Chat.Server.set_nickname(nickname, self())
      [] -> Logger.info("invalid input")
    end
  end

  def handle("/MSG" <> rest) do
    Logger.info("#{@log_prefix} [/MSG] #{inspect(rest)}")
    # args = String.split(rest)
  end

  def lst(input) do
    Logger.info("#{@log_prefix} [LST] #{inspect(input)}")
  end

  def msg(input) do
    Logger.info("#{@log_prefix} [MSG] #{inspect(input)}")
  end

  def grp(input) do
    Logger.info("#{@log_prefix} [GRP] #{inspect(input)}")
  end

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
  def handle_info({:tcp, socket, input}, socket) do
    # data = String.trim(data)

    # command = String.slice(input, 0, 4)
    handle(input)

    # case command do
    #   "/NCK" -> nck(input)
    #   "/LST" -> lst(input)
    #   "/MSG" -> msg(input)
    #   "/GRP" -> grp(input)
    #   _ -> Logger.alert("Invalid command")
    # end

    # Logger.info("#{@log_prefix} #{data}")
    :inet.setopts(socket, active: :once)
    :gen_tcp.send(socket, input)
    {:noreply, socket}
  end

  @impl true
  def handle_info({:tcp_closed, socket}, socket) do
    Logger.info("#{@log_prefix} #{inspect(socket)} Closed")
    :gen_tcp.close(socket)
    {:stop, :normal, socket}
  end

  @impl true
  def handle_info({:message, message}, socket) do
    Logger.info("#{@log_prefix} #{message}")
    {:noreply, socket}
  end
end
