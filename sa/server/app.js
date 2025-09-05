const expressID = "express";
const pathID = "path";
const fsID = "fs";
const defaultPort = 3000;
const rootPath = "/";

// app.js
const express = require(expressID);
const path = require(pathID);
const fs = require(fsID);
const app = express();
const port = process.env.PORT || defaultPort;

const labs = fs.readdirSync(__dirname).filter((file) => {
  return fs.statSync(path.join(__dirname, file)).isDirectory();
});

labs.forEach((lab) => {
  const folderPath = path.join(__dirname, lab);
  const routePath = `/COMP4537/labs/${lab}`;

  app.use(routePath, express.static(folderPath));

  console.log(`Serving Lab ${lab}`);
});

app.get(rootPath, (req, res) => {
  res.send("Labs server running");
});

app.listen(port, () => {
  console.log(`App running at http://localhost:${port}`);
});
