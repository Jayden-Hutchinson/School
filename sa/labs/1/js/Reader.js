import { HTML, TITLE, NOTE_LIST } from "./env.js";

import { Note } from "./Note.js";

class Reader {
  constructor() {
    this.notes = [];

    this.container = document.getElementById(HTML.ID.APP);

    this.title = document.createElement(HTML.ELEMENT.DIV);
    this.title.id = HTML.ID.TITLE;
    this.title.textContent = TITLE;

    this.noteList = document.createElement(HTML.ELEMENT.UL);
    this.noteList.id = HTML.ID.NOTE_LIST;

    this.container.appendChild(this.title);
    this.container.appendChild(this.noteList);
  }

  loadNotes() {
    for (let i = 0; i < localStorage.length; i++) {
      const key = localStorage.key(i);
      const text = localStorage.getItem(key);
      const note = new Note(key, text);
      this.notes.push(note);
    }
  }
}

new Reader();
