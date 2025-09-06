import { HexGenerator } from "./HexGenerator.js";
import { GameButton } from "./GameButton.js";
import * as env from "./constants.js"

export class ButtonManager {
  constructor() {
    this.gameButtons = [];
    this.hexGenerator = new HexGenerator();
  }

  /**
   * 
   */
  createGameButtons(numButtons) {
    for (let i = env.FIRST_VALUE; i <= numButtons; i++) {
      const randomColour = this.hexGenerator.randomHex();
      const gameButton = new GameButton(i, randomColour)
      this.gameButtons.push(gameButton);
    }
  }

  appendGameButtons(gameBoard) {
    this.gameButtons.forEach((button) => {
      gameBoard.appendChild(button);
    })
  }

  scrambleGameButtons() {
    const startDelayMilliseconds = this.gameButtons.length * env.MS_PER_SECOND;
    return new Promise((resolve) => {
      setTimeout(() => {
        this.gameButtons.forEach((button) => {

          let repeatCount = env.START_INDEX;
          this.setRandomPosition(button);

          const interval = setInterval(() => {
            repeatCount++;
            this.setRandomPosition(button);

            if (repeatCount >= env.SCRAMBLE.repeats) {
              clearInterval(interval);
              resolve();
            }
          }, env.SCRAMBLE.repeatInterval);
        })
      }, startDelayMilliseconds)
    })
  }

  initializeGameButtons(onClick) {
    this.gameButtons.forEach((button) => {
      button.style.color = env.COLOR.transparent;

      button.addEventListener(env.EVENT.click, () => {
        onClick(button.value);
      })
    })
  }

  disableGameButtons(onClick) {
    this.gameButtons.forEach((button) => {
      button.removeEventListener(env.EVENT.click, onClick)
    })

  }

  revealGameButton(index) {
    this.gameButtons[index].style.color = env.COLOR.black;
  }

  revealGameButtons() {
    this.gameButtons.forEach((button) => {
      button.style.color = env.COLOR.black
    })
  }

  setRandomPosition(button) {
    const maxX = window.innerWidth - button.offsetWidth;
    const maxY = window.innerHeight - button.offsetHeight;

    const randomX = Math.floor(Math.random() * maxX);
    const randomY = Math.floor(Math.random() * maxY);

    button.style.position = env.POSITION.absolute;
    button.style.left = `${randomX}${env.UNIT.px}`;
    button.style.top = `${randomY}${env.UNIT.px}`;

  }
}
