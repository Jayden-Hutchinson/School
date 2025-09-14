import { HTML, NOTE } from "./env.js";

export class Note {
  /**
   *
   * @param {int} key - Note Key for local storage
   * @param {string} text - Text content of the note
   */
  constructor(key, text = null) {
    this.key = key;
    this.element = document.createElement(HTML.ELEMENT.DIV);
    this.element.className = HTML.CLASS.NOTE;

    this.textArea = document.createElement(HTML.ELEMENT.TEXT_AREA);
    this.textArea.className = HTML.CLASS.NOTE_TEXT_AREA;
    this.textArea.name = HTML.CLASS.NOTE_TEXT_AREA;
    this.textArea.rows = NOTE.TEXT_AREA.ROWS;
    this.textArea.value = text;
    this.textArea.addEventListener(HTML.EVENT.INPUT, () => {
      this.textArea.style.height = `${this.textArea.scrollHeight}px`;
    });
    this.element.appendChild(this.textArea);
  }

  getText() {
    return this.textArea.value;
  }
}
