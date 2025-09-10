import { HTML, TITLE, NOTE_LIST } from "./env.js";

import { NoteManager } from "./NoteManager.js";

class Reader {
  constructor() {
    this.noteManager = new NoteManager();

    this.container = document.getElementById(HTML.ID.APP);

    this.title = document.createElement(HTML.ELEMENT.DIV);
    this.title.id = HTML.ID.TITLE;
    this.title.textContent = TITLE;

    this.noteList = document.createElement(HTML.ELEMENT.UL);
    this.noteList.id = HTML.ID.NOTE_LIST;

    this.container.appendChild(this.title);
    this.container.appendChild(this.noteList);
  }
}

new Reader();
