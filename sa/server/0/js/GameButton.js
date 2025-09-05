const CLASS_NAME = "GameButton";

export class GameButton extends HTMLButtonElement {
  constructor(index, colour) {
    super();
    this.index = index;
    this.style.backgroundColor = colour;
    this.className = CLASS_NAME;
    this.textContent = index;
  }

  setPosition(top, left) {
    this.style.top = top;
    this.style.left = left;
  }
}

customElements.define("game-button", GameButton, { extends: "button" });
