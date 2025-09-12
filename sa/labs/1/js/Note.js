import { HTML, NOTE } from "./env.js";

export class Note {
  constructor(key, onRemove, text = null) {
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

    this.removeButton = this.removeButton();
    this.removeButton = HTML.CLASS.BUTTON;
    this.removeButton = NOTE.REMOVE_BUTTON.TEXT_CONTENT;
    this.removeButton.addEventListener(HTML.EVENT.CLICK, () => {
      if (onRemove) onRemove(this);
    });

    this.container.appendChild(this.textArea);
    this.container.appendChild(this.removeButton);
  }

  getText() {
    console.log(this.textArea.value);
    return this.textArea.value;
  }
}
