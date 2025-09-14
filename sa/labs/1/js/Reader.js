import { HTML } from "./env.js";

import { Note } from "./Note.js";
import { Notepad } from "./Notepad.js";

class Reader {
  constructor() {
    this.element = document.getElementById(HTML.ID.APP);
    this.notepad = new Notepad();
    this.notepad.notes.forEach((note) => {
      note.textArea.disabled = true;
    });
    this.element.appendChild(this.notepad.element);
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
