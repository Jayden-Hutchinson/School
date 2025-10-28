defmodule Name do
  defstruct [:first, :last]

  def new(firstname, lastname) do
    %Name{first: firstname, last: lastname}
  end
end

defmodule Student do
  defstruct [id: "", name: %Name{}, scores: %{}]

  def new(id, firstname, lastname, scores \\ %{}) do
    %Student{id: id, name: Name.new(firstname, lastname), scores: scores}
  end

  def add_score(s, course, score) do
    %Student{s | scores: Map.put(s.scores, course, score)}
  end
end
