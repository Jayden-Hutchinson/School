import { ID, EVENT } from "./env.js";

import { NoteManager } from "./NoteManager.js";
import { NoteConstructor } from "./NoteConstructor.js";
import { ElementConstructor } from "./ElementConstructor.js";

class App {
  constructor() {
    this.noteConstructor = new NoteConstructor();
    this.noteManager = new NoteManager();

    this.app = document.getElementById(ID.APP);

    this.title = ElementConstructor.createTitle();
    this.noteList = ElementConstructor.createNoteList();
    this.addButton = ElementConstructor.createAddButton();

    this.addButton.addEventListener(EVENT.CLICK, () => {
      const note = this.noteConstructor.createNote();
      this.noteManager.addNote(note);
      this.noteList.appendChild(note.element);
    });

    this.app.appendChild(this.title);
    this.app.appendChild(this.noteList);
    this.app.appendChild(this.addButton);

    console.log("App contructor done");
  }
}

new App();
