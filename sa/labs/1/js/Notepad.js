import { NOTEPAD, NOTE, HTML } from "./env.js";

import { Note } from "./Note.js";

export class Notepad {
  constructor(mode) {
    this.notes = [];
    this.container = document.getElementById(HTML.ID.APP);

    this.title = document.createElement(HTML.ELEMENT.DIV);
    this.title.id = HTML.ID.TITLE;
    this.title.textContent = NOTEPAD.TITLE;

    this.noteUL = document.createElement(HTML.ELEMENT.UL);
    this.noteUL.id = HTML.ID.NOTEPAD;

    const numNotes = localStorage.length;
    for (let i = 0; i < numNotes; i++) {
      const text = localStorage.getItem(i);
      const note = new Note(i, text);

      this.notes.push(note);
      this.noteUL.appendChild(note.container);
    }

    this.container.appendChild(this.title);
    this.container.appendChild(this.noteUL);

    if (mode == NOTEPAD.MODE.WRITE) {
      this.addButton = document.createElement(HTML.ELEMENT.BUTTON);
      this.addButton.className = HTML.CLASS.BUTTON;
      this.addButton.textContent = NOTEPAD.ADD_BUTTON.TEXT_CONTENT;

      this.addButton.addEventListener(HTML.EVENT.CLICK, () => {
        const key = this.notes.length;
        const note = new Note(key);

        this.notes.push(note);
        console.log(this.notes);
        const removeButton = document.createElement(HTML.ELEMENT.BUTTON);
        removeButton.className = HTML.CLASS.BUTTON;
        removeButton.textContent = NOTE.REMOVE_BUTTON.TEXT_CONTENT;
        removeButton.addEventListener(HTML.EVENT.CLICK, () => {
          this.removeNote(note);
        });
        note.container.appendChild(removeButton);
        this.noteUL.appendChild(note.container);
      });

      const numNotes = localStorage.length;
      this.notes.forEach((note) => {
        const removeButton = document.createElement(HTML.ELEMENT.BUTTON);
        if (mode == NOTEPAD.MODE.WRITE) {
          removeButton.className = HTML.CLASS.BUTTON;
          removeButton.textContent = NOTE.REMOVE_BUTTON.TEXT_CONTENT;
          removeButton.addEventListener(HTML.EVENT.CLICK, () => {
            this.removeNote(note);
          });
          note.container.appendChild(removeButton);
        }
        this.noteUL.appendChild(note.container);
      });
      this.container.appendChild(this.addButton);
    }
  }

  removeNote(note) {
    note.container.remove();
    localStorage.removeItem(note.key);
    this.notes.splice(note.key, 1);
    console.log(this.notes);
    this.notes.forEach((note, index) => {
      localStorage.removeItem(note.key);
      note.key = index;
      localStorage.setItem(note.key, note.getText());
    });
  }
}
