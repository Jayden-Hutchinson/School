import { NOTE, ELEMENT, EVENT, CLASS_NAME } from "./env.js";

export class Note {
  constructor(text = null) {
    this.element = document.createElement(ELEMENT.DIV);
    this.element.className = CLASS_NAME.NOTE;

    this.textArea = document.createElement(ELEMENT.TEXT_AREA);
    this.textArea.className = CLASS_NAME.NOTE_TEXT_AREA;
    this.textArea.name = CLASS_NAME.NOTE_TEXT_AREA;
    this.textArea.rows = NOTE.TEXT_AREA.ROWS;
    this.textArea.textContent = text;

    this.removeButton = document.createElement(ELEMENT.BUTTON);
    this.removeButton.className = CLASS_NAME.BUTTON;
    this.removeButton.textContent = NOTE.REMOVE_BUTTON.TEXT_CONTENT;

    this.textArea.addEventListener(EVENT.INPUT, () => {
      this.textArea.style.height = `${this.textArea.scrollHeight}px`;
    });

    this.removeButton.addEventListener(EVENT.CLICK, () => {
      this.element.remove();
    });

    this.element.appendChild(this.textArea);
    this.element.appendChild(this.removeButton);
  }
}
