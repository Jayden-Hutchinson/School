defmodule ChatProxyTest do
  require Logger

  @host String.to_charlist("localhost")
  @port 6666

  def run_test() do
    clients =
      for nickname <- ["Zack", "Tanner", "Jayden"] do
        {:ok, socket} = :gen_tcp.connect(@host, @port, [:binary, active: false])

        send_command(socket, "/NCK #{nickname}")
        socket
      end

    Process.sleep(2000)
    send_command(Enum.at(clients, 0), "/MSG Jayden Hello")
    Process.sleep(2000)
    send_command(Enum.at(clients, 0), "/GRP #hutch Jayden,Tanner")
    Process.sleep(2000)
    send_command(Enum.at(clients, 0), "/MSG #hutch Hello")
    Process.sleep(2000)
    send_command(Enum.at(clients, 0), "/MSG #hutch,Tanner,Jayden Hello")
    Process.sleep(2000)
    send_command(Enum.at(clients, 0), "/MSG Tanner,Jayde Hello")
    Process.sleep(2000)
    send_command(Enum.at(clients, 0), "/NCK !Zack")
  end

  defp send_command(socket, command) do
    :gen_tcp.send(socket, command <> "\n")
  end
end
