defmodule Card.Store do
  use GenServer

  @name __MODULE__

  def start_link(filename \\ "cards.db") do
    GenServer.start_link(__MODULE__, filename, name: @name)
  end

  def get() do
    GenServer.call(@name, :get)
  end

  def put(value) do
    GenServer.call(@name, {:put, value})
  end

  @impl true
  def init(filename) do
    cards =
      if File.exists?(filename) do
        File.read!(filename) |> :erlang.binary_to_term()
      else
        []
      end

    {:ok, %{filename: filename, cards: cards}}
  end

  @impl true
  def handle_call(:get, _from, state) do
    {:reply, state.cards, state}
  end

  @impl true
  def handle_call({:put, value}, _from, state) do
    File.write(state.filename, :erlang.term_to_binary(value))
    {:reply, :ok, %{state | cards: value}}
  end
end
