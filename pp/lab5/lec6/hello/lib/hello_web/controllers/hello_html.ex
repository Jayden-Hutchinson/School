defmodule HelloWeb.HelloHTML do
  use HelloWeb, :html

  def hello(assigns) do
    ~H"""
    <section>
      <h2>Hello</h2>
    </section>
    """
  end

  embed_templates "hello_html/*"
end
