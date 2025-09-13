import { HTML, NOTE } from "./env.js";

export class Note {
  /**
   *
   * @param {int} key - Note Key for local storage
   * @param {function} onRemove - Callback to remove the note from its list
   * @param {string} text - Text content of the note
   * @param {string} mode - Mode for Read or Write
   */
  constructor(key, text = null) {
    this.key = key;
    this.container = document.createElement(HTML.ELEMENT.DIV);
    this.container.className = HTML.CLASS.NOTE;

    this.textArea = document.createElement(HTML.ELEMENT.TEXT_AREA);
    this.textArea.className = HTML.CLASS.NOTE_TEXT_AREA;
    this.textArea.name = HTML.CLASS.NOTE_TEXT_AREA;
    this.textArea.rows = NOTE.TEXT_AREA.ROWS;
    this.textArea.value = text;
    this.textArea.addEventListener(HTML.EVENT.INPUT, () => {
      this.textArea.style.height = `${this.textArea.scrollHeight}px`;
    });
    this.container.appendChild(this.textArea);
  }

  getText() {
    return this.textArea.value;
  }
}
