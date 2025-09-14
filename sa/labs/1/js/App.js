import { HTML, READER, WRITER } from "./env.js";
import { messages } from "../lang/messages/en/user.js";

class App {
  constructor() {
    this.title = document.createElement(HTML.ELEMENT.DIV);
    this.title.id = HTML.ID.TITLE;
    this.title.textContent = messages.appTitle;

    this.readerLink = this.createLink(messages.readerLink, READER.LINK);
    this.writerLink = this.createLink(messages.writerLink, WRITER.LINK);

    this.links = document.createElement(HTML.ELEMENT.DIV);
    this.links.className = HTML.CLASS.LINKS;
    this.links.appendChild(this.readerLink);
    this.links.appendChild(this.writerLink);

    this.element = document.getElementById(HTML.ID.APP);
    this.element.appendChild(this.title);
    this.element.appendChild(this.links);
  }

  createLink(text, href) {
    const link = document.createElement(HTML.ELEMENT.A);
    link.textContent = text;
    link.href = href;
    return link;
  }
}

new App();
