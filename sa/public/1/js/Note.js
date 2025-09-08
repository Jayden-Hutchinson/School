
import { NOTE, ELEMENT, EVENT, } from "./env.js"

export class Note {
    constructor() {
        this.element = document.createElement(ELEMENT.DIV);
        this.element.className = NOTE.CLASS_NAME;

        this.textArea = document.createElement(ELEMENT.TEXT_AREA);
        this.textArea.className = NOTE.TEXT_AREA.CLASS_NAME;
        this.textArea.name = NOTE.TEXT_AREA.CLASS_NAME;
        this.textArea.rows = NOTE.TEXT_AREA.ROWS;
        this.textArea.addEventListener(EVENT.INPUT, () => {
            this.textArea.style.height = `${this.textArea.scrollHeight}px`;
        });
        this.removeButton = document.createElement(ELEMENT.BUTTON);
        this.removeButton.className = NOTE.REMOVE_BUTTON.CLASS_NAME;
        this.removeButton.textContent = NOTE.REMOVE_BUTTON.TEXT_CONTENT;


        this.element.appendChild(this.textArea);
        this.element.appendChild(this.removeButton);
    }
}