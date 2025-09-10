const HTML.CLASS = "GameButton";

export class GameButton extends HTMLButtonHTML.ELEMENT {
  constructor(index, colour) {
    super();
    this.value = index;
    this.style.backgroundColor = colour;
    this.className = HTML.CLASS;
    this.textContent = index;
  }

  setPosition(top, left) {
    this.style.top = top;
    this.style.left = left;
  }
}

customHTML.ELEMENTs.define("game-button", GameButton, { extends: "button" });
