defmodule Card.Application do
  # See https://hexdocs.pm/elixir/Application.html

  use Application

  @impl true
  def start(_type, _args) do
    # Create an ETS Tabel with the name Card.Store
    :ets.new(Card.Store, [:public, :named_table])

    children = [
      # Start the Process Registry
      # Registry.start_link(name: Counter.Registry, keys: :unique)
      {Registry, name: Card.Registry, keys: :unique},

      # Start the Pertition Supervisor
      {PartitionSupervisor,
       name: Card.WorkerSupervisors, child_spec: Card.WorkerSupervisor, partition: 4}
    ]

    Supervisor.start_link(children, name: Card.Supervisor, strategy: :one_for_one)
  end
end
