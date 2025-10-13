# how to implement a generic server
defmodule GenericServer do
  def start(m, init) do
    spawn(fn -> loop(m, init) end)
  end

  def call(pid, msg) do
    send(pid, {self(), msg})
    receive do
      x -> x
    end
  end
  
  defp loop(m, state) do
    receive do
      {from, msg} ->
        {response, new_state} = m.handle_call(msg, from, state)
        send(from, response)
        loop(m, new_state)
    end
  end
end

defmodule ArithmeticServer do
  def start() do
    GenericServer.start(__MODULE__, nil)
  end

  def square(pid, x) do
    GenericServer.call(pid, {:square, x})
  end

  def sqrt(pid, x) do
    GenericServer.call(pid, {:sqrt, x})
  end

  def handle_call({:square, x}, _from, state) do
    {x * x, state}
  end
  
  def handle_call({:sqrt, x}, _from, state) do
    response = if x < 0, do: :error, else: :math.sqrt(x)
    {response, state}
  end
end

# how to implement a generic server
defmodule GenericServerV2 do
  def start(m, arg) do
    state = m.init(arg)
    spawn(fn -> loop(m, state) end)
  end

  def call(pid, msg) do
    send(pid, {:call, self(), msg})
    receive do
      x -> x
    end
  end
  
  def cast(pid, msg) do
    send(pid, {:cast, msg})
  end

  defp loop(m, state) do
    receive do
      {:call, from, msg} ->
        {response, new_state} = m.handle_call(msg, from, state)
        send(from, response)
        loop(m, new_state)
      {:cast, msg} ->
        new_state = m.handle_cast(msg, state)
        loop(m, new_state)
    end
  end
end

# more general verions: 2 types of messages (calls & casts)
defmodule CounterServer do
  def start(n \\ 0) do
    GenericServerV2.start(__MODULE__, n)
  end

  def inc(pid) do
    GenericServerV2.cast(pid, :inc)
  end

  def value(pid) do
    GenericServerV2.call(pid, :value)
  end

  def init(arg) do
    arg + 1
  end

  def handle_call(:value, _from, state) do
    {state, state}
  end

  def handle_cast(:inc, state) do
    state + 1
  end
end


