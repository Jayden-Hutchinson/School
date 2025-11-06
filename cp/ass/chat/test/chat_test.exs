defmodule ChatTest do
  use ExUnit.Case
  doctest Chat

  # Parse Nickname

  test "Valid nickname input" do
    nickname = Chat.parse_nickname("Jayden")
    assert nickname == {:nickname, "Jayden"}
  end

  test "Empty input error" do
    nickname = Chat.parse_nickname("")
    assert nickname == {:error, "Please enter nickname"}

    nickname = Chat.parse_nickname(" ")
    assert nickname == {:error, "Please enter nickname"}

    nickname = Chat.parse_nickname("          ")
    assert nickname == {:error, "Please enter nickname"}
  end

  test "Nickname must start with alphabet character error" do
    nickname = Chat.parse_nickname("1Jayden")
    assert nickname == {:error, "Nickname must start with alphabet character"}

    nickname = Chat.parse_nickname("_Jayden")
    assert nickname == {:error, "Nickname must start with alphabet character"}

    nickname = Chat.parse_nickname(".Jayden")
    assert nickname == {:error, "Nickname must start with alphabet character"}
  end

  test "Nickname must be alphanumeric error" do
    nickname = Chat.parse_nickname("Jayden!")

    assert nickname ==
             {:error, "Nickname must be alphanumeric and can only include the '_' character"}

    nickname = Chat.parse_nickname("Jayden-747")

    assert nickname ==
             {:error, "Nickname must be alphanumeric and can only include the '_' character"}

    nickname = Chat.parse_nickname("Jayden-747")

    assert nickname ==
             {:error, "Nickname must be alphanumeric and can only include the '_' character"}
  end

  test "Nickname must be max 10 characters error" do
    nickname = Chat.parse_nickname("JaydenHutchinson")

    assert nickname ==
             {:error, "Nickname must be max 10 characters."}

    nickname = Chat.parse_nickname("ABCDEFGHIJK")

    assert nickname ==
             {:error, "Nickname must be max 10 characters."}
  end

  # Parse Group Name
  test "Valid group name input" do
    group_name = Chat.parse_group_name("#raiders")
    assert group_name == {:group_name, "#raiders"}
  end

  test "Group name must start with '#'" do
    group_name = Chat.parse_group_name("raiders")
    assert group_name == {:error, "Group name must start with '#'."}

    group_name = Chat.parse_group_name("_raiders")
    assert group_name == {:error, "Group name must start with '#'."}

    group_name = Chat.parse_group_name("1raiders")
    assert group_name == {:error, "Group name must start with '#'."}
  end

  test "" do
  end
end
