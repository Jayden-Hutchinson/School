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
      console.log(localStorage.length);
      const index = localStorage.length;
      const note = new Note(index);
      this.notes.push(note);
      this.noteUL.appendChild(note.container);
    });

    this.container.appendChild(this.title);
    this.container.appendChild(this.noteUL);
    this.container.appendChild(this.addButton);

    setInterval(() => this.saveNotes(), WRITER.WRITE_INTERVAL);
  }

  addNote() {
    console.log("adding note");
    const key = localStorage.length;
    const note = new Note(key, (noteInstance) => this.removeNote(noteInstance));
    console.log(note.key);
    this.notes.push(note);
    this.container.appendChild(note.container);
  }

  removeNote(note) {
    console.log("removing note");
    this.notes.splice(note.key, 1);
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
