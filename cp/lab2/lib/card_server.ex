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

    ranks = [2, 3, 4, 5, 6, 7, 8, 9, :J, :Q, :K, :A]

    for rank <- ranks, suit <- Map.values(suits) do
      {rank, suit}
    end
  end

  def shuffle() do
    send(__MODULE__, :shuffle)
  end

  def count() do
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

  # def deal(n \\ 1) do
  # end

  defp loop(deck) do
    receive do
      :shuffle ->
        loop(Enum.shuffle(deck))

      {:print, from} ->
        send(from, deck)
        loop(deck)
    end
  end
end
