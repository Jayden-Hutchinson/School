import { HTML, READER, TITLE } from "./env.js";

import { Note } from "./Note.js";
import { Notepad } from "./Notepad.js";

class Reader {
  constructor() {
    this.Notepad = new Notepad(READER.NOTEPAD_MODE);
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
