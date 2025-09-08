
export class NoteManager {
    constructor(container) {
        this.container = container
    }

    addNote(note) {
        if (container) {
            container.appendChild(note.element)
        } else {
            console.log("Note manager container null")
        }
    }
}