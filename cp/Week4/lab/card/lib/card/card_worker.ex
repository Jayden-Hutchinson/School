defmodule Card.Worker do
  use GenServer

  def start() do
    Genserver.start_link(__MODULE__, :ok, name: __MODULE__)
  end

  ## Client API ##

  def new(), do: GenServer.call(__MODULE__, :new)

  def shuffle(), do: GenServer.call(__MODULE__, :shuffle)

  def count(), do: GenServer.call(__MODULE__, :count)

  def deal(n \\ 1) when is_integer(n) and n > 0, do: GenServer.call(__MODULE__, {:deal, n})
  def deal(n) when n <= 0, do: {:error, :invalid_number_of_cards}

  ## Server Callbacks ##

  @impl true
  def init(:ok) do
    {:ok, build_deck()}
  end

  @impl true
  def handle_call(:new, _from, _deck) do
    {:reply, :ok, build_deck()}
  end

  @impl true
  def handle_call(:shuffle, _from, deck) do
    {:reply, :ok, Enum.shuffle(deck)}
  end

  @impl true
  def handle_call(:count, _from, deck) do
    {:reply, length(deck), deck}
  end

  @impl true
  def handle_call({:deal, n}, _from, deck) do
    if n <= length(deck) do
      {hand, rest} = Enum.split(deck, n)
      {:reply, {:ok, hand}, rest}
    else
      {:reply, {:error, :not_enough_cards}, deck}
    end
  end

  defp build_deck() do
    for rank <- [2, 3, 4, 5, 6, 7, 8, 9, 10, :J, :Q, :K, :A],
        suit <- [:C, :D, :H, :S] do
      {rank, suit}
    end
  end
end
