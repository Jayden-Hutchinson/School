import { Note } from "./Note.js";

export class NoteConstructor {
  createNote(text = null) {
    return new Note(text);
  }
}
