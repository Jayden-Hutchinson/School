import { HTML, NOTE } from "./env.js";

export class Note {
  constructor(text = null) {
    this.container = document.createElement(HTML.ELEMENT.DIV);
    this.container.className = HTML.CLASS.NOTE;

    this.textArea = document.createElement(HTML.ELEMENT.TEXT_AREA);
    this.textArea.className = HTML.CLASS.NOTE_TEXT_AREA;
    this.textArea.name = HTML.CLASS.NOTE_TEXT_AREA;
    this.textArea.rows = NOTE.TEXT_AREA.ROWS;
    this.textArea.textContent = text;

    this.removeButton = document.createElement(HTML.ELEMENT.BUTTON);
    this.removeButton.className = HTML.CLASS.BUTTON;
    this.removeButton.textContent = NOTE.REMOVE_BUTTON.TEXT_CONTENT;

    this.textArea.addEventListener(HTML.EVENT.INPUT, () => {
      this.textArea.style.height = `${this.textArea.scrollHeight}px`;
    });

    this.removeButton.addEventListener(HTML.EVENT.CLICK, () => {
      this.container.remove();
    });

    this.container.appendChild(this.textArea);
    this.container.appendChild(this.removeButton);
  }

  getTextContent() {
    return this.textArea.textContent;
  }
}
