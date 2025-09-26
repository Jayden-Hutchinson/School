defmodule Lec4 do
  def create(n \\ 4) do
    Enum.map(1..n, fn _ -> pid = self(); spawn(fn -> loop(pid) end) end)
  end

  def trap_exit(pid) do
    send(pid, :trap_exit)
  end

  def no_trap_exit(pid) do
    send(pid, :no_trap_exit)
  end

  def link(pid, other) do
    send(pid, {:link, other})
  end

  def info(pid) when is_pid(pid) do
    if Process.alive?(pid) do
      {pid, [{:alive, true}, Process.info(pid, :trap_exit), 
        Process.info(pid, :links)]}
    else
      {pid, [alive: false]}
    end
  end

  def info(pids) do
    Enum.map(pids, &info/1)
  end

  defp loop(parent) do
    receive do
      :trap_exit ->
        Process.flag(:trap_exit, true)
      :no_trap_exit ->
        Process.flag(:trap_exit, false)
      {:link, other} ->
        Process.link(other)
      x -> send(parent, {self(), x})
    end
    loop(parent)
  end
end
