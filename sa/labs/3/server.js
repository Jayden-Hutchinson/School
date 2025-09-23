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

  res.writeHead(200, { "Content-Type": "text/html; color: blue" });
  res.end(
    `<h1 style="color:blue">Hello, ${name}!<br>Server time: ${new Date().toLocaleString()}</h1>`
  );
});

const PORT = 3000;
server.listen(PORT, () => {
  console.log(`Server running at http://localhost:${PORT}`);
});
