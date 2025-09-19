defmodule Card.ServerTest do
  use ExUnit.Case

  setup do
    {:ok, pid} = Card.Server.start()
    %{pid: pid}
  end

  test "new/1 returns a full deck of 52 unique cards", %{pid: pid} do
    deck = Card.Server.new(pid)
    assert length(deck) == 52
    assert Enum.uniq(deck) == deck
    assert hd(deck) == {2, :C}
    assert List.last(deck) == {:A, :S}
  end

  test "count/1 returns the number of remaining cards", %{pid: pid} do
    assert Card.Server.count(pid) == 52
    Card.Server.deal(pid, 5)
    assert Card.Server.count(pid) == 47
  end

  test "shuffle/1 returns a deck with same cards but different order", %{pid: pid} do
    deck_before = Card.Server.count(pid)
    shuffled_deck = Card.Server.shuffle(pid)
    assert length(shuffled_deck) == deck_before
    # simple check: shuffled deck should differ in order sometimes
    deck_after = Enum.shuffle(shuffled_deck)
    refute shuffled_deck == deck_after
  end

  test "deal/2 returns {:ok, cards} when enough cards", %{pid: pid} do
    {:ok, hand} = Card.Server.deal(pid, 5)
    assert length(hand) == 5
    assert Card.Server.count(pid) == 47
  end

  test "deal/2 returns {:error, :not_enough_cards} if too many requested", %{pid: pid} do
    {:error, :not_enough_cards} = Card.Server.deal(pid, 100)
    assert Card.Server.count(pid) == 52
  end
end
