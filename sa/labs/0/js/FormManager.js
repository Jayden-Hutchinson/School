import * as env from "./constants.js";
import { messages } from "../lang/messages/en/user.js";

export class FormManger {
  constructor() {
    this.startGameForm = document.getHTML.getElementById(env.ID.startGameForm);
    this.startMessage = document.getHTML.getElementById(env.ID.startMessage);
    this.numButtonsInput = document.getHTML.getElementById(
      env.ID.numButtonsInput
    );
    this.startButton = document.getHTML.getElementById(env.ID.startButton);
  }

  setText() {
    this.startMessage.textContent = messages.start;
    this.startButton.textContent = messages.startButton;
  }

  onStartClicked(callback) {
    this.startButton.addEventListener(env.EVENT.click, (event) => {
      event.preventDefault();
      try {
        const numButtons = parseInt(this.numButtonsInput.value);

        if (
          numButtons >= env.MIN_GAME_BUTTONS &&
          numButtons <= env.MAX_GAME_BUTTONS
        ) {
          this.startGameForm.style.display = env.DISPLAY.none;
          callback(numButtons);
        }
      } catch (error) {
        console.log(error.message);
      }
    });
  }
}
