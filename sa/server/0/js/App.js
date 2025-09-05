import { ButtonManager } from "./ButtonManager.js";
import { GameButton } from "./GameButton.js";

const START_INDEX = 0;
const FIRST_VALUE = 1;
const MS_PER_SECOND = 1000;

const HEX_LENGTH = 6;
const BASE_HEX = 16;
const HEX_SYMBOL = "#";
const HEX_LETTERS = "0123456789ABCDEF";

const ID = {
  input: "num-buttons-input",
  startButton: "start-button",
  gameButtonContainer: "game-button-container",
};

const EVENT = {
  click: "click",
  contentLoaded: "DOMContentLoaded",
};

const POSITION = {
  absolute: "absolute",
};

class App {
  constructor() {
    this.startButton = document.getElementById(ID.startButton);
    this.input = document.getElementById(ID.input);
    this.buttonManager = new ButtonManager();
    this.gameButtonContainer = document.getElementById(ID.gameButtonContainer);

    this.startButton.addEventListener(EVENT.click, () => {
      const value = this.input.value;
      const timeoutMillisecond = value * MS_PER_SECOND;

      this.spawnButtons(value);

      setTimeout(() => {
        this.scrambleButtons();
      }, timeoutMillisecond);
    });
  }

  scrambleButtons() {
    console.log(this.gameButtonContainer);
    Array.from(this.gameButtonContainer.children).forEach((child) => {
      child.style.position = POSITION.absolute;
    });
  }

  /**
   * Spawns the game buttons to be used during the game
   *
   * @param {int} numButtons - The number of game buttons to spawn
   */
  spawnButtons(numButtons) {
    for (let i = FIRST_VALUE; i <= numButtons; i++) {
      this.gameButtonContainer.appendChild(
        new GameButton(i, this.getRandomHex())
      );
    }
  }

  getRandomHex() {
    let colour = HEX_SYMBOL;
    for (let i = START_INDEX; i < HEX_LENGTH; i++) {
      let randomHexValue = Math.floor(Math.random() * BASE_HEX);
      colour += HEX_LETTERS[randomHexValue];
    }
    return colour;
  }
}

new App();
