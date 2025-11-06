defmodule Chat do
  @moduledoc """
  Documentation for `Chat`.
  """

  def parse_nickname(input) do
    nickname = String.trim(input)

    cond do
      is_empty(nickname) ->
        {:error, "Please enter nickname"}

      not starts_with_alpha(nickname) ->
        {:error, "Nickname must start with alphabet character"}

      not is_alphanumeric(nickname) ->
        {:error, "Nickname must be alphanumeric and can only include the '_' character"}

      not is_correct_length(nickname, 10) ->
        {:error, "Nickname must be max 10 characters."}

      true ->
        {:nickname, nickname}
    end
  end

  def parse_group_name(input) do
    input = String.trim(input)

    {_first_char, group_name} = input |> String.split_at(1)

    cond do
      not starts_with_hash(input) ->
        {:error, "Group name must start with '#'."}

      not is_alphanumeric(group_name) ->
        {:error, "Group name must be alphanumeric and can only include the '_' character"}

      not is_correct_length(group_name, 11) ->
        {:error, "Group name must be max 10 characters."}

      true ->
        {:group_name, input}
    end
  end

  defp starts_with_hash(string) do
    String.starts_with?(string, "#")
  end

  defp is_correct_length(string, length) do
    String.length(string) <= length
  end

  defp starts_with_alpha(string) do
    {first_char, _rest} = String.split_at(string, 1)
    String.match?(first_char, ~r/^[A-Za-z]/)
  end

  defp is_alphanumeric(string) do
    String.match?(string, ~r/^[A-Za-z0-9_]+$/)
  end

  defp is_empty(string) do
    string == ""
  end
end
