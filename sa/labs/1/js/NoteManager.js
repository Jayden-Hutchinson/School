export class NoteManager {
  constructor() {
    this.notes = [];
  }

  addNote(note) {
    this.notes.push(note);
    console.log(this.notes);
  }
}
