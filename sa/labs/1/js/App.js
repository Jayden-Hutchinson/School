import { HTML, READER, WRITER, TITLE } from "./env.js";

class App {
  constructor() {
    this.title = document.createElement(HTML.ELEMENT.DIV);
    this.title.id = HTML.ID.TITLE;
    this.title.textContent = TITLE;

    this.readerLink = this.createLink(READER.TEXT, READER.LINK);
    this.writerLink = this.createLink(WRITER.TEXT, WRITER.LINK);

    this.links = document.createElement(HTML.ELEMENT.DIV);
    this.links.className = HTML.CLASS.LINKS;
    this.links.appendChild(this.readerLink);
    this.links.appendChild(this.writerLink);

    this.container = document.getElementById(HTML.ID.APP);
    this.container.appendChild(this.title);
    this.container.appendChild(this.links);
  }

  createLink(text, href) {
    const link = document.createElement(HTML.ELEMENT.A);
    link.textContent = text;
    link.href = href;
    return link;
  }
}

new App();
