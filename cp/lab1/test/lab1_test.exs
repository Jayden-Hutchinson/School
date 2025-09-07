defmodule Lab1Test do
  use ExUnit.Case
  doctest Lab1

  # inverse_mod()/2
  # Valid cases (inverse exists)
  test "3 has inverse mod 7" do
    assert Lab1.inverse_mod(3, 7) == 5
    assert rem(3 * Lab1.inverse_mod(3, 7), 7) == 1
  end

  test "4 has inverse mod 9" do
    assert Lab1.inverse_mod(4, 9) == 7
    assert rem(4 * Lab1.inverse_mod(4, 9), 9) == 1
  end

  test "10 has inverse mod 17" do
    assert Lab1.inverse_mod(10, 17) == 12
    assert rem(10 * Lab1.inverse_mod(10, 17), 17) == 1
  end

  # Invalid cases (no inverse)
  test "2 has no inverse mod 4" do
    assert Lab1.inverse_mod(2, 4) == :not_invertible
  end

  test "6 has no inverse mod 15" do
    assert Lab1.inverse_mod(6, 15) == :not_invertible
  end

  test "8 has no inverse mod 20" do
    assert Lab1.inverse_mod(8, 20) == :not_invertible
  end

  # ️ Edge cases
  test "1 always has inverse mod m (should be 1)" do
    assert Lab1.inverse_mod(1, 5) == 1
    assert Lab1.inverse_mod(1, 13) == 1
  end

  test "negative numbers should still work" do
    assert Lab1.inverse_mod(-3, 7) == 5
  end

  test "a larger than modulus is reduced first" do
    assert Lab1.inverse_mod(10, 7) == Lab1.inverse_mod(3, 7)
  end

  # pow_mod()/2
  test "small numbers" do
    # 2^3 = 8, 8 mod 5 = 3
    assert Lab1.pow_mod(2, 3, 5) == 3
    # 3^2 = 9, 9 mod 7 = 2
    assert Lab1.pow_mod(3, 2, 7) == 2
  end

  test "power of zero" do
    assert Lab1.pow_mod(2, 0, 5) == 1
    assert Lab1.pow_mod(10, 0, 17) == 1
  end

  test "mod 1 always returns 0" do
    assert Lab1.pow_mod(2, 10, 1) == 0
    assert Lab1.pow_mod(100, 100, 1) == 0
  end

  test "large powers" do
    # 2^10 = 1024, 1024 mod 1000 = 24
    assert Lab1.pow_mod(2, 10, 1000) == 24
    # 3^20 mod 7
    assert Lab1.pow_mod(3, 20, 7) == 2
  end

  test "a greater than n" do
    # 10^3 = 1000, 1000 mod 6 = 4
    assert Lab1.pow_mod(10, 3, 6) == 4
  end

  test "negative base" do
    # (-2)^3 = -8, -8 mod 5 = 2
    assert Lab1.pow_mod(-2, 3, 5) == 2
    # (-3)^4 = 81, 81 mod 7 = 4
    assert Lab1.pow_mod(-3, 4, 7) == 4
  end
end
