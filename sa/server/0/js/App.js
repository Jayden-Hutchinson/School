import { ButtonManager } from "./ButtonManager.js";
import { FormManger } from "./FormManager.js";
import * as env from "./constants.js"

class App {
  constructor() {
    this.gameBoard = document.getElementById(env.ID.gameBoard);
    this.buttonManager = new ButtonManager();
    this.formManager = new FormManger();
    this.buttonsClicked = env.FIRST_VALUE;
    this.gameButtonIndex = env.START_INDEX;

    this.start = this.start.bind(this)
    this.formManager.onStartClicked(this.start)
  }

  async start(numButtons) {
    this.buttonManager.createGameButtons(numButtons);
    this.buttonManager.appendGameButtons(this.gameBoard)

    await this.buttonManager.scrambleGameButtons();

    this.checkAnswer = this.checkAnswer.bind(this)
    this.buttonManager.initializeGameButtons(this.checkAnswer);
  }


  checkAnswer(value) {
    const isCorrect = value == this.buttonsClicked

    if (isCorrect) {
      this.updateGame();
    }
    else {
      this.endGame();
    }
    this.updateCounters();
  }

  updateGame() {
    this.buttonManager.revealGameButton(this.gameButtonIndex)
  }

  endGame() {
    this.buttonManager.revealGameButtons();
    this.buttonManager.disableGameButtons(this.checkAnswer);
  }

  updateCounters() {
    this.buttonsClicked++;
    this.gameButtonIndex++;
  }

}

new App();
