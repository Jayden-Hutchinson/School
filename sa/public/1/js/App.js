import { ID } from "./env.js"

import { NoteConstructor } from "./NoteConstructor.js"
import { NoteManager } from "./NoteManager.js"

class App {
    constructor() {
        this.container = document.getElementById(ID.CONTAINER);

        this.noteConstructor = new NoteConstructor();
        this.noteManager = new NoteManager(container);

        const newNote = this.noteConstructor.createNote();
        this.noteManager.addNote(newNote)
        console.log("App contructor done")
    }

    run() {
        console.log("App running...")
    }
}

new App().run();