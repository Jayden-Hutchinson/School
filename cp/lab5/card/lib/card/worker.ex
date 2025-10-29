defmodule Card.Worker do
  use GenServer
  @store Card.Store

  def start_link(name) do
    GenServer.start_link(__MODULE__, name, name: via(name))
  end

  def via(name) do
    {:via, Registry, {Card.Registry, {__MODULE__, name}}}
  end

  ## API ##

  def new(name) do
    GenServer.cast(via(name), :new)
  end

  def shuffle(name) do
    GenServer.call(via(name), :shuffle)
  end

  def count(name) do
    GenServer.call(via(name), :count)
  end

  def deal(name, n \\ 1) do
    GenServer.call(via(name), {:deal, n})
  end

  ## Server Callbacks ##

  @impl true
  def init(name) do
    IO.puts("Card.Worker starting... #{name}(#{inspect(self())})")
    Process.flag(:trap_exit, true)
    name = {__MODULE__, name}

    value =
      case :ets.lookup(@store, name) do
        [] -> build_deck()
        [{_, x}] -> x
      end

    {:ok, {name, value}}
  end

  @impl true
  def handle_call(:new, _from, _deck) do
    {:reply, :ok, build_deck()}
  end

  @impl true
  def handle_call(:shuffle, _from, {name, deck}) do
    shuffled = Enum.shuffle(deck)
    {:reply, :ok, {name, shuffled}}
  end

  @impl true
  def handle_call(:count, _from, {_, deck} = state) do
    {:reply, length(deck), state}
  end

  @impl true
  def handle_call({:deal, n}, _from, {name, deck} = state) do
    unless is_integer(n) do
      raise "#{inspect(name)} crashed."
    end

    if n <= length(deck) do
      {hand, rest} = Enum.split(deck, n)
      {:reply, {:ok, hand}, {name, rest}}
    else
      {:reply, {:error, :not_enough_cards}, state}
    end
  end

  @impl true
  def terminate(reason, {name, deck}) do
    IO.puts("#{inspect(name)} crashed #{inspect(reason)}")
    :ets.insert(@store, {name, deck})
  end

  defp build_deck() do
    for rank <- ~w(A 2 3 4 5 6 7 8 9 10 J Q K), suit <- ~w(♤ ♡ ♢ ♧) do
      {rank, suit}
    end
  end
end

defmodule Test do
  def test1() do
    Card.WorkerSupervisor.start_worker("w1")

    Card.Worker.deal("w1", 5)
    Card.Worker.deal("w1", :die)
    Card.Worker.count("w1")
  end

  def test2() do
    Card.Worker.count("w1")
  end
end
