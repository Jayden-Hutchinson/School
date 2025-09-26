const https = require("https");
const fs = require("fs");
const { URL } = require("url");

const options = {
  key: fs.readFileSync("server.key"),
  cert: fs.readFileSync("server.cert"),
};

const server = https.createServer(options, (req, res) => {
  const url = new URL(req.url, `https://${req.headers.host}`);
  const name = url.searchParams.get("name") || "Guest";
  const message = getDate(name);
  console.log(message);

  res.writeHead(200, { "Content-Type": "text/html" });
  res.end(message);
});

const PORT = 3000;
server.listen(PORT, () => {
  console.log(`Server running at http://localhost:${PORT}`);
});
