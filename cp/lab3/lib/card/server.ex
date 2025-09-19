defmodule Card.Server do
  use GenServer

  @doc """
  start unregistered server with a new deck of 52 cards
  """
  @spec start() :: {:ok, pid()}
  def start() do
    {:ok, pid} = GenServer.start(__MODULE__, :ok)

    # request a new deck from the server
    new(pid)
    {:ok, pid}
  end

  ## Client API ##

  @doc """
  use a new sorted deck of 52 cards
  sorted = [{2,:C}, {2,:D}, {2,:H}, {2,:S}, {3,:C}, {3,:D}, {3,:H}, {3,:S}, ... {:A,:C}, {:A,:D}, {:A,:H}, {:A,:S}]
  """
  @spec new(pid()) :: {tuple()}
  def new(pid), do: GenServer.call(pid, :new)

  @doc """
  shuffle deck of remaining cards
  """
  @spec shuffle(pid()) :: [tuple()]
  def shuffle(pid), do: GenServer.call(pid, :shuffle)

  @doc """
  the count of remaining cards in the deck
  """
  @spec count(pid()) :: non_neg_integer()
  def count(pid), do: GenServer.call(pid, :count)

  @doc """
  deals `n` cards from the remaining deck of cards
  """
  @spec deal(pid(), non_neg_integer()) :: {:ok, [tuple()]} | {:error, :not_enough_cards}
  def deal(pid, n \\ 1), do: GenServer.call(pid, {:deal, n})

  # @spec print([tuple()]) ::
  def print(deck) do
    Enum.each(deck, fn {rank, suit} ->
      IO.puts("#{rank}#{suit}")
    end)
  end

  ## Server ##

  # start the server with an empty list
  @spec init(:ok) :: {:ok, []}
  def init(:ok) do
    {:ok, []}
  end

  def handle_call(:new, _from, _state) do
    deck =
      for rank <- [2, 3, 4, 5, 6, 7, 8, 9, 10, :J, :Q, :K, :A],
          suit <- [:C, :D, :H, :S] do
        {rank, suit}
      end

    {:reply, deck, deck}
  end

  def handle_call(:shuffle, _from, state) do
    shuffled = Enum.shuffle(state)
    {:reply, shuffled, shuffled}
  end

  def handle_call(:count, _from, state) do
    {:reply, length(state), state}
  end

  def handle_call({:deal, n}, _from, state) do
    if n <= length(state) do
      {hand, rest} = Enum.split(state, n)
      {:reply, {:ok, hand}, rest}
    else
      {:reply, {:error, :not_enough_cards}, state}
    end
  end
end
