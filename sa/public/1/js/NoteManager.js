export class NoteManager {
  constructor() {
    this.notes = [];
  }

  addNote(note) {
    if (this.ul) {
      this.notes.push(note);
    } else {
      console.log("Note manager container null");
    }
  }
}
