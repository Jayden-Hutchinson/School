defmodule Card.Server do
  use GenServer

  # start unregistered server with a new deck of 52 cards
  def start() do
  end

  def init(_) do
    {:ok, new()}
  end

  # use a new sorted deck of 52 cards
  # sorted = [{2,:C}, {2,:D}, {2,:H}, {2,:S},
  #           {3,:C}, {3,:D}, {3,:H}, {3,:S},
  #           ...
  #           {:A,:C}, {:A,:D}, {:A,:H}, {:A,:S}]
  def new(pid) do
    suits = [:C, :D, :H, :S]
    ranks = [2, 3, 4, 5, 6, 7, 8, 9, 10, :J, :Q, :K, :A]

    for rank <- ranks, suit <- suits do
      {rank, suit}
    end
  end

  # shuffle deck of remaining cards
  def shuffle(pid) do
  end

  # return: (int) number of remaining cards
  def count(pid) do
  end

  # deal n cards
  # return: { :ok , [card] } | { :error, reason }
  def deal(pid, n \\ 1) do
  end

  def print(deck) do
    Enum.each(deck, fn {rank, suit} ->
      IO.puts("#{rank_string}#{suit}")
    end)
  end
end
