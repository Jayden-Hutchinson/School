import { ButtonManager } from "./ButtonManager.js";
import { FormManger } from "./FormManager.js";
import { messages } from "../lang/messages/en/user.js"
import * as env from "./constants.js"

class App {
  constructor() {
    this.buttonsClicked = env.FIRST_VALUE;
    this.gameButtonIndex = env.START_INDEX;

    this.buttonManager = new ButtonManager();
    this.formManager = new FormManger();

    this.gameBoard = document.getElementById(env.ID.gameBoard);
    this.startButton = document.getElementById(env.ID.startButton);


    this.start = this.start.bind(this)

    this.formManager.setText();
    this.formManager.onStartClicked(this.start)
  }

  async start(numButtons) {
    console.log("start")
    this.gameBoard.innerHTML = env.EMPTY_STRING;

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

      if (this.buttonsClicked >= this.buttonManager.numButtons) {
        this.endGame();
      }
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
    console.log("endGame()")
    this.buttonManager.revealGameButtons();
    this.buttonManager.disableGameButtons(this.checkAnswer);
  }

  updateCounters() {
    this.buttonsClicked++;
    this.gameButtonIndex++;
  }
}

new App();
