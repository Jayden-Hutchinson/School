module.exports = {
  date: "Hello %1, What a beatiful day. Server current time is",

  writeFailure: {
    error: 400,
    message: "Error writing to the file",
  },
  appendFailure: {
    error: 500,
    message: "Error appending to the file",
  },
  readFailure: {
    error: 500,
    message: "Error reading the file",
  },
  notFound: {
    error: 404,
    message: "Not found",
  },
};
