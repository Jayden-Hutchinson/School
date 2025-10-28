defmodule Card.Worker do
  use GenServer

  @name __MODULE__

  def start_link(_args) do
    IO.puts("[Card.Worker] Starting")
    GenServer.start_link(__MODULE__, :ok, name: @name)
  end

  ## Public API ##

  def new() do
    GenServer.call(@name, :new)
  end

  def shuffle() do
    GenServer.call(@name, :shuffle)
  end

  def count() do
    GenServer.call(@name, :count)
  end

  def deal(n \\ 1)

  def deal(n) when is_integer(n) and n > 0 do
    GenServer.call(@name, {:deal, n})
  end

  def deal(n) when n <= 0 do
    {:error, :invalid_number_of_cards}
  end

  ## Server Callbacks ##

  @impl true
  def init(:ok) do
    cards = Card.Store.get()
    IO.puts("[Card.Worker] Starting: #{Enum.join(cards, "  ")}")
    {:ok, if(cards == [], do: build_deck(), else: cards)}
  end

  @impl true
  def handle_call(:new, _from, _deck) do
    {:reply, :ok, build_deck()}
  end

  @impl true
  def handle_call(:shuffle, _from, deck) do
    shuffled = Enum.shuffle(deck)
    Card.Store.put(shuffled)
    {:reply, :ok, shuffled}
  end

  @impl true
  def handle_call(:count, _from, deck) do
    {:reply, length(deck), deck}
  end

  @impl true
  def handle_call({:deal, n}, _from, deck) do
    if n <= length(deck) do
      {hand, rest} = Enum.split(deck, n)
      Card.Store.put(rest)
      {:reply, {:ok, hand}, rest}
    else
      {:reply, {:error, :not_enough_cards}, deck}
    end
  end

  defp build_deck() do
    for rank <- ~w(A 2 3 4 5 6 7 8 9 10 J Q K), suit <- ~w(♤ ♡ ♢ ♧) do
      {rank, suit}
    end
  end
end
