import { HTML, TITLE, NOTE_LIST } from "./env.js";

import { Note } from "./Note.js";
import { NoteManager } from "./NoteManager.js";

class Writer {
  constructor() {
    this.noteManager = new NoteManager();

    this.container = document.getElementById(HTML.ID.APP);

    this.title = document.createElement(HTML.ELEMENT.DIV);
    this.title.id = HTML.ID.TITLE;
    this.title.textContent = TITLE;

    this.noteList = document.createElement(HTML.ELEMENT.UL);
    this.noteList.id = HTML.ID.NOTE_LIST;

    this.addButton = document.createElement(HTML.ELEMENT.BUTTON);
    this.addButton.className = HTML.CLASS.BUTTON;
    this.addButton.textContent = NOTE_LIST.ADD_BUTTON.TEXT_CONTENT;

    this.addButton.addEventListener(HTML.EVENT.CLICK, () => {
      const note = new Note();
      this.noteManager.addNote(note);
      this.noteList.appendChild(note.container);
    });

    this.container.appendChild(this.title);
    this.container.appendChild(this.noteList);
    this.container.appendChild(this.addButton);
  }
}

new Writer();
