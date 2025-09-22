const http = require("http");
const { URL } = require("url");

const server = http.createServer((req, res) => {
  const url = new URL(req.url, `https://${req.headers.host}`);
  const name = url.searchParams.get("name") || "Guest";

  res.send(`<p>hello<p/>`);
});

const PORT = 3000;
server.listen(PORT, () => {
  console.log(`Server running at http://localhost:${PORT}`);
});
