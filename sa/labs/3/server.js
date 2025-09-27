`use strict`;
const http = require("http");
const url = require("url");
const fs = require("fs");
const getDate = require("./modules/utils.js");

const PORT = 3000;

const SUCCESS = 200;
const FAILURE = 500;

const FILE_NAME = "file.txt";
const NEW_LINE = "\n";
const EMPTY_STRING = "";

const CONTENT_TYPE = {
  HTML: "text/html",
};

const SERVER_PATH = {
  GET_DATE: "/COMP/labs/3/getDate/",
  READ_FILE: "/COMP4537/labs/3/readFile/",
};

http
  .createServer((req, res) => {
    const parsedUrl = url.parse(req.url, true);
    const pathName = parsedUrl.pathname;
    const query = parsedUrl.query;

    if (pathName === SERVER_PATH.GET_DATE) {
      res.writeHead(SUCCESS, CONTENT_TYPE.HTML);
      const message = getDate(query.name);
      res.end(message);
    }

    if (
      pathName === SERVER_PATH.READ_FILE &&
      query.text.trim() !== EMPTY_STRING
    ) {
      console.log(query.text);
      const textToAppend = query.text + NEW_LINE;

      fs.appendFile(FILE_NAME, textToAppend, (err) => {
        if (err) {
          res.writeHead(FAILURE, CONTENT_TYPE.HTML);
        } else {
          res.writeHead(SUCCESS, CONTENT_TYPE.HTML);
          res.end(query.text);
        }
      });
    } else {
      res.writeHead(400, CONTENT_TYPE.HTML);
      return res.end("Error: empty string");
    }
  })
  .listen(PORT, () => {
    console.log(`Server running at http://localhost:${PORT}`);
  });
