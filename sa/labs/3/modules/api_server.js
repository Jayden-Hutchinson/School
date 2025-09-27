"use strict";
const http = require("http");
const url = require("url");
const fs = require("fs");
const getDate = require("./utils.js");
const { match } = require("assert");

const PORT = 3000;
const SUCCESS = 200;
const FAILURE = 500;
const FILE_NAME = "file.txt";
const NEW_LINE = "\n";
const EMPTY_STRING = "";
const TEXT = { UTF8: "utf8" };
const CONTENT_TYPE = { HTML: { "Content-Type": "text/html" } };
const SERVER_URL = {
  GET_DATE: "/COMP4537/labs/3/getDate/",
  READ_FILE: "/COMP4537/labs/3/readFile/",
  WRITE_FILE: "/COMP4537/labs/3/writeFile/",
};

class ApiServer {
  constructor() {
    http
      .createServer((req, res) => {
        const parsedUrl = url.parse(req.url, true);
        const pathName = parsedUrl.pathname;
        const query = parsedUrl.query;

        switch (pathName) {
          case SERVER_URL.GET_DATE:
            this.handleGetDate(query, res);
            return;

          case SERVER_URL.READ_FILE:
            this.handleReadFile(res);
            return;

          case SERVER_URL.WRITE_FILE:
            this.handleWriteFile(query, res);
            return;
        }
        // Incorrect path given
        res.writeHead(404, CONTENT_TYPE.HTML);
        return res.end("404 Not Found");
      })
      .listen(PORT, () => {
        console.log(`Server running at http://localhost:${PORT}`);
      });
  }

  handleGetDate(query, res) {
    res.writeHead(SUCCESS, CONTENT_TYPE.HTML);
    const name = query.name?.trim() || "Guest";
    const message = getDate(name);
    return res.end(message);
  }

  handleReadFile(res) {
    fs.readFile(FILE_NAME, TEXT.UTF8, (err, data) => {
      if (err) {
        res.writeHead(FAILURE, CONTENT_TYPE.HTML);
        return res.end(err.message);
      }
      res.writeHead(SUCCESS, CONTENT_TYPE.HTML);
      return res.end(data);
    });
  }

  handleWriteFile(query, res) {
    if (!query.text || query.text.trim() === EMPTY_STRING) {
      res.writeHead(400, CONTENT_TYPE.HTML);
      return res.end("Error: empty string");
    }
    const textToAppend = query.text + NEW_LINE;
    fs.appendFile(FILE_NAME, textToAppend, (err) => {
      if (err) {
        res.writeHead(FAILURE, CONTENT_TYPE.HTML);
        return res.end("Error writing file");
      }
      res.writeHead(SUCCESS, CONTENT_TYPE.HTML);
      return res.end(query.text);
    });
  }
}

module.exports = { ApiServer };
