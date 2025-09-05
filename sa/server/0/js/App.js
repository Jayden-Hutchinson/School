import { ButtonManager } from "./ButtonManager.js";

const MS_PER_SECOND = 1000;

const ID = {
  input: "num-buttons-input",
  startButton: "start-button",
  gameButtonContainer: "game-button-container",
  numButtonsForm: "num-buttons-form",
};

const EVENT = {
  click: "click",
  contentLoaded: "DOMContentLoaded",
};

const DISPLAY = {
  none: "none",
};

class App {
  constructor() {
    this.startButton = document.getElementById(ID.startButton);
    this.input = document.getElementById(ID.input);
    this.gameButtonContainer = document.getElementById(ID.gameButtonContainer);
    this.numButtonsForm = document.getElementById(ID.numButtonsForm);

    this.buttonManager = new ButtonManager();

    this.startButton.addEventListener(EVENT.click, (event) => {
      this.numButtonsForm.style.display = DISPLAY.none;
      event.preventDefault();

      const value = this.input.value;
      const numButtons = parseInt(value);
      const timeoutMillisecond = value * MS_PER_SECOND;

      this.buttonManager.createButtons(numButtons);

      this.buttonManager.gameButtons.forEach((button) => {
        this.gameButtonContainer.appendChild(button);
      });

      setTimeout(() => {
        this.buttonManager.scrambleButtons();
      }, timeoutMillisecond);
    });
  }
}

new App();
