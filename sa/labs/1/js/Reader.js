import { HTML, READER } from "./env.js";
import { messages } from "../lang/messages/en/user.js";

import { Note } from "./Note.js";
import { Notepad } from "./Notepad.js";

class Reader {
  constructor() {
    this.element = document.getElementById(HTML.ID.APP);

    const loadedTime = this.currentTime();

    this.lastLoaded = document.createElement(HTML.ELEMENT.DIV);
    this.lastLoaded.className = HTML.CLASS.LAST_SAVED;
    this.lastLoaded.textContent = `${messages.lastLoaded} ${loadedTime}`;

    this.notepad = new Notepad();
    this.notepad.notes.forEach((note) => {
      note.textArea.disabled = true;
    });

    this.backButton = document.createElement(HTML.ELEMENT.BUTTON);
    this.backButton.className = HTML.CLASS.BUTTON;
    this.backButton.textContent = messages.backButton;
    this.backButton.addEventListener(HTML.EVENT.CLICK, () => {
      history.back();
    });

    this.element.appendChild(this.lastLoaded);
    this.element.appendChild(this.notepad.element);
    this.element.appendChild(this.backButton);

    setInterval(() => this.loadNotes(), READER.READ_INTERVAL);
  }

  currentTime() {
    return new Date().toLocaleDateString([], {
      hour: "2-digit",
      minute: "2-digit",
      second: "2-digit",
      hour12: true,
    });
  }

  loadNotes() {
    const numNotes = localStorage.length - 1;
    this.notepad.noteUL.innerHTML = messages.none;

    for (let i = 0; i < numNotes; i++) {
      const text = localStorage.getItem(i);
      const note = new Note(i, text);
      this.notepad.notes.push(note);
      this.notepad.noteUL.appendChild(note.element);

      const loadedTime = this.currentTime();
      this.lastLoaded.textContent = `${messages.lastLoaded} ${loadedTime}`;
    }
  }
}

new Reader();
