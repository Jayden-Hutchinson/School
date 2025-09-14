import { NOTEPAD, NOTE, HTML } from "./env.js";
import { messages } from "../lang/messages/en/user.js";

import { Note } from "./Note.js";

export class Notepad {
  constructor() {
    this.notes = [];
    this.element = document.createElement(HTML.ELEMENT.DIV);
    this.element.id = HTML.ID.NOTEPAD;

    this.title = document.createElement(HTML.ELEMENT.DIV);
    this.title.id = HTML.ID.TITLE;
    this.title.textContent = messages.notepadTitle;

    this.noteUL = document.createElement(HTML.ELEMENT.UL);

    const numNotes = localStorage.length - 1;
    for (let i = 0; i < numNotes; i++) {
      const text = localStorage.getItem(i);
      const note = new Note(i, text);

      this.notes.push(note);
      this.noteUL.appendChild(note.element);
    }

    this.element.appendChild(this.title);
    this.element.appendChild(this.noteUL);
  }

  addNote(note) {
    this.noteUL.appendChild(note.element);
  }

  removeNote(note) {
    note.element.remove();
    localStorage.removeItem(note.key);
    this.notes.splice(note.key, 1);

    this.notes.forEach((note, index) => {
      localStorage.removeItem(note.key);
      note.key = index;
      localStorage.setItem(note.key, note.getText());
    });
  }
}
