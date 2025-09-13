import { WRITER } from "./env.js";

import { Notepad } from "./Notepad.js";

class Writer {
  constructor() {
    this.notepad = new Notepad(WRITER.NOTEPAD_MODE);
    setInterval(() => this.saveNotes(), WRITER.WRITE_INTERVAL);
  }

  removeNote(note) {
    note.container.remove();
    localStorage.removeItem(note.key);

    if (this.notepad.notes) {
      console.log(this.notepad.notes);
      this.notepad.notes.forEach((note, index) => {
        note.key = index;
      });
    }
  }

  saveNotes() {
    if (this.notepad.notes) {
      this.notepad.notes.forEach((note) => {
        const text = note.getText();
        localStorage.setItem(note.key, text);
      });
    }
  }
}

new Writer();
