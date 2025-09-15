defmodule CardServer do
  def start() do
    deck = new()
    Process.register(spawn(fn -> loop(deck) end), __MODULE__)
  end

  def new() do
    suits = %{
      spades: <<0x2660::utf8>>,
      clubs: <<0x2663::utf8>>,
      hearts: <<0x2665::utf8>>,
      diamonds: <<0x2666::utf8>>
    }

    ranks = [2, 3, 4, 5, 6, 7, 8, 9, 10, :J, :Q, :K, :A]

    for rank <- ranks, suit <- Map.values(suits) do
      {rank, suit}
    end
  end

  def shuffle() do
    send(__MODULE__, :shuffle)
  end

  def count() do
    send(__MODULE__, {:count, self()})

    receive do
      count -> count
    end
  end

  def deal(n \\ 1) do
    send(__MODULE__, {:deal, n, self()})

    receive do
      {:ok, dealt} -> {:ok, dealt}
      {:error, reason} -> {:error, reason}
    end
  end

  def print() do
    send(__MODULE__, {:print, self()})

    receive do
      deck ->
        Enum.each(deck, fn {rank, suit} ->
          rank_string =
            case rank do
              :J -> "J"
              :Q -> "Q"
              :K -> "K"
              :A -> "A"
              n -> Integer.to_string(n)
            end

          IO.puts("#{rank_string}#{suit}")
        end)
    end
  end

  defp loop(deck) do
    receive do
      :shuffle ->
        loop(Enum.shuffle(deck))

      {:count, from} ->
        send(from, length(deck))
        loop(deck)

      {:print, from} ->
        send(from, deck)
        loop(deck)

      {:deal, n, from} when n < 0 ->
        send(from, {:error, "Cannot deal a negative number of cards"})
        loop(deck)

      {:deal, n, from} ->
        if n > length(deck) do
          send(from, {:error, "Not enough cards left to deal #{n} cards"})
          loop(deck)
        else
          {dealt, remaining} = Enum.split(deck, n)
          send(from, {:ok, dealt})
          loop(remaining)
        end
    end
  end
end
