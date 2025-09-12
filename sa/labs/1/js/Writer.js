import { HTML, TITLE, NOTE_LIST, WRITER } from "./env.js";

import { Note } from "./Note.js";

class Writer {
  constructor() {
    this.notes = [];
    this.container = document.getElementById(HTML.ID.APP);

    this.title = document.createElement(HTML.ELEMENT.DIV);
    this.title.id = HTML.ID.TITLE;
    this.title.textContent = TITLE;

    this.noteUL = document.createElement(HTML.ELEMENT.UL);
    this.noteUL.id = HTML.ID.NOTE_LIST;

    this.addButton = document.createElement(HTML.ELEMENT.BUTTON);
    this.addButton.className = HTML.CLASS.BUTTON;
    this.addButton.textContent = NOTE_LIST.ADD_BUTTON.TEXT_CONTENT;

    this.addButton.addEventListener(HTML.EVENT.CLICK, () => {
      const note = this.createNote();
      this.addNote(note);
    });

    this.container.appendChild(this.title);
    this.container.appendChild(this.noteUL);
    this.container.appendChild(this.addButton);

    setInterval(() => this.saveNotes(), WRITER.WRITE_INTERVAL);
  }

  createNote() {
    const key = localStorage.length;
    return new Note(key, (noteInstance) => this.removeNote(noteInstance));
  }

  addNote(note) {
    this.notes.push(note);
    this.noteUL.appendChild(note.container);
  }

  removeNote(note) {
    note.container.remove();
    localStorage.removeItem(note.key);
  }

  saveNotes() {
    if (this.notes) {
      this.notes.forEach((note) => {
        const text = note.getText();
        localStorage.setItem(note.key, text);
      });
    }
  }
}

new Writer();
