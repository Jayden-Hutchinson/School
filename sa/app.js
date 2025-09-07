const expressID = "express";
const pathID = "path";
const fsID = "fs";
const defaultPort = 3000;

// app.js
const express = require(expressID);
const path = require(pathID);
const fs = require(fsID);
const app = express();
const port = process.env.PORT || defaultPort;

const rootPath = "/";
const publicDir = path.join(__dirname, "public")

const labs = fs.readdirSync(publicDir).filter((file) => {
  return fs.statSync(path.join(publicDir, file)).isDirectory();
});

labs.forEach((lab) => {
  const urlRoute = `/COMP4537/labs/${lab}`;

  app.use(urlRoute, express.static(path.join(publicDir, lab)));

  console.log(`Serving Lab ${lab}`);
});

app.get(rootPath, (req, res) => {
  res.send("Labs server running");
});

app.listen(port, () => {
  console.log(`App running at http://localhost:${port}`);
});
