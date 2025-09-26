import messages from "../locals/en/en.js";

export function getDate(name) {
  const now = new Date().toDateString();
  const greeting = messages.greeting.replace("%1", name);
  return `<span style="color:blue;">${greeting} ${now}</span>`;
}
