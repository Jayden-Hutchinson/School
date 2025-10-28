defmodule Counter.WorkerSupervisor do
  use DynamicSupervisor

  def start_link(_) do
    DynamicSupervisor.start_link(__MODULE__, nil, name: {:global, __MODULE__})
  end

  def start_worker(name) do
    DynamicSupervisor.start_child({:global, __MODULE__}, {Counter.Worker, name})
  end

  @impl true
  def init(_) do
    DynamicSupervisor.init(strategy: :one_for_one)
  end
end
