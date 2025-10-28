defmodule Counter.WorkerSupervisor do
  def start_link(_) do
    DynamicSupervisor.start_link(__MODULE__, nil, name: {:global, __MODULE__})
  end

  def start_worker(name) do
    DynamicSupervisor.start_child({:global, __MODULE__}, # name of parent
      %{ # child spec
        id: Counter.Worker,
        start: {Counter.Worker, :start_link, [name]}
      })
  end
end 


