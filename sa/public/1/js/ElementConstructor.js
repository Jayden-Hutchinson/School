import { ELEMENT, ID, TITLE, CLASS_NAME, NOTE_LIST } from "./env.js";

export class ElementConstructor {
  static createTitle() {
    const title = document.createElement(ELEMENT.DIV);
    title.id = ID.TITLE;
    title.textContent = TITLE;
    return title;
  }
  static createAddButton() {
    const addButton = document.createElement(ELEMENT.BUTTON);
    addButton.className = CLASS_NAME.BUTTON;
    addButton.textContent = NOTE_LIST.ADD_BUTTON.TEXT_CONTENT;
    return addButton;
  }
  static createNoteList() {
    const note_list = document.createElement(ELEMENT.UL);
    note_list.id = ID.NOTE_LIST;
    return note_list;
  }
}
