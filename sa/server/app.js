const expressID = "express";
const pathID = "path";
const defaultPort = 3000;
const rootPath = "/";
const labsFolder = "/COMP4537/labs/";
const lab0FolderName = "Lab0";
const lab0Route = `${labsFolder}0`;

// app.js
const express = require(expressID);
const path = require(pathID);
const app = express();
const port = process.env.PORT || defaultPort;

app.use(lab0Route, express.static(path.join(__dirname, lab0FolderName)));

app.get(rootPath, (req, res) => {
  res.send("Hello");
});

app.listen(port, () => {
  console.log(`App running at http://localhost:${port}`);
});
