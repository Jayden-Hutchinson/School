import { WRITER, HTML, LOCAL_STORAGE } from "./env.js";

import { Notepad } from "./Notepad.js";
import { Note } from "./Note.js";
import { messages } from "../lang/messages/en/user.js";

class Writer {
  constructor() {
    this.element = document.getElementById(HTML.ID.APP);

    const savedTime = localStorage.getItem(LOCAL_STORAGE.LAST_SAVED);

    this.lastSaved = document.createElement(HTML.ELEMENT.DIV);
    this.lastSaved.className = HTML.CLASS.LAST_SAVED;
    this.lastSaved.textContent = savedTime
      ? `${messages.lastSaved} ${savedTime}`
      : messages.lastSaved;

    this.notepad = new Notepad();
    console.log(this.notepad.notes);
    this.notepad.notes.forEach((note) => {
      const removeButton = document.createElement(HTML.ELEMENT.BUTTON);
      removeButton.className = HTML.CLASS.BUTTON;
      removeButton.textContent = messages.removeButton;
      removeButton.addEventListener(HTML.EVENT.CLICK, () => {
        this.removeNote(note);
        note.element.appendChild(removeButton);
      });
      note.element.appendChild(removeButton);
    });

    this.addButton = document.createElement(HTML.ELEMENT.BUTTON);
    this.addButton.className = HTML.CLASS.BUTTON;
    this.addButton.textContent = messages.addButton;

    this.addButton.addEventListener(HTML.EVENT.CLICK, () => {
      const key = this.notepad.notes.length;
      const note = new Note(key);

      this.notepad.noteUL.appendChild(note.element);

      this.notepad.notes.push(note);
      const removeButton = document.createElement(HTML.ELEMENT.BUTTON);
      removeButton.className = HTML.CLASS.BUTTON;
      removeButton.textContent = messages.removeButton;
      removeButton.addEventListener(HTML.EVENT.CLICK, () => {
        this.removeNote(note);
      });
      note.element.appendChild(removeButton);
    });

    this.element.appendChild(this.lastSaved);
    this.element.appendChild(this.notepad.element);
    this.element.appendChild(this.addButton);

    setInterval(() => this.saveNotes(), WRITER.WRITE_INTERVAL);
  }

  removeNote(note) {
    note.element.remove();
    localStorage.removeItem(note.key);

    if (this.notepad.notes) {
      this.notepad.notes.forEach((note, index) => {
        note.key = index;
      });
    }
  }

  currentTime() {
    return new Date().toLocaleDateString([], {
      hour: "2-digit",
      minute: "2-digit",
      second: "2-digit",
      hour12: true,
    });
  }

  saveNotes() {
    if (this.notepad.notes) {
      this.lastSaved.time = this.currentTime();
      localStorage.setItem(LOCAL_STORAGE.LAST_SAVED, this.currentTime());
      this.lastSaved.textContent = `${messages.lastSaved} ${this.lastSaved.time}`;

      this.notepad.notes.forEach((note) => {
        const text = note.getText();
        localStorage.setItem(note.key, text);
      });
    }
  }
}

new Writer();
