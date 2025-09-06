import { ID, EVENT, DISPLAY } from "./constants.js"

export class FormManger {
    constructor() {
        this.startGameForm = document.getElementById(ID.startGameForm);
        this.input = document.getElementById(ID.input);
        this.startButton = document.getElementById(ID.startButton);
    }

    onStartClicked(callback) {
        this.startButton.addEventListener(EVENT.click, (event) => {
            event.preventDefault();
            this.startGameForm.style.display = DISPLAY.none;

            callback(parseInt(this.input.value));
        })

    }
}