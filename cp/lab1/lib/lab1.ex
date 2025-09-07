defmodule Lab1 do
  def inverse_mod(a, n) do
    if a < 0 do
      inverse_mod(-a, n)
    else
      inverse_mod(a, n, n, a, 0, 1)
    end
  end

  defp inverse_mod(_a, n, r, 0, t, _new_t) do
    if r > 1 do
      :not_invertible
    else
      rem(t + n, n)
    end
  end

  defp inverse_mod(a, n, r, new_r, t, new_t) do
    quotient = div(r, new_r)

    inverse_mod(
      a,
      n,
      new_r,
      r - quotient * new_r,
      new_t,
      t - quotient * new_t
    )
  end

  # returns a^m mod n
  def pow_mod(a, m, n) when m > 0 do
    a_mod = rem(a + n, n)

    if rem(m, 2) == 0 do
      pow_mod(rem(a_mod * a_mod, n), div(m, 2), n)
    else
      rem(a_mod * pow_mod(a_mod, m - 1, n), n)
    end
  end

  def pow_mod(_a, 0, _n) do
    1
  end
end
