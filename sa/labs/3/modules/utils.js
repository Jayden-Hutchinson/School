const messages = require("../locals/en/en.js");

function getDate(name) {
  const now = new Date().toString();
  const greeting = messages.date.replace("%1", name);
  return `<span style="color:blue; font-size:48px">${greeting} ${now}</span>`;
}

module.exports = getDate;
