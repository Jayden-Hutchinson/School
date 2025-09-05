import { HexGenerator } from "./HexGenerator.js";
import { GameButton } from "./GameButton.js";

const START_INDEX = 0;
const FIRST_VALUE = 1;
const POSITION = {
  absolute: "absolute",
};

export class ButtonManager {
  constructor() {
    this.gameButtons = [];
    this.hexGenerator = new HexGenerator();
  }

  scrambleButtons() {
    this.gameButtons.forEach((button) => {
      button.style.position = POSITION.absolute;
    });
  }

  /**
   * Spawns the game buttons to be used during the game
   *
   * @param {int} numButtons - The number of game buttons to spawn
   */
  createButtons(numButtons) {
    for (let i = START_INDEX; i < numButtons; i++) {
      this.gameButtons.push(new GameButton(i, HexGenerator.getRandomHex()));
    }
  }
}
