class GameButton extends HTMLButtonElement {
  constructor(index, colour) {
    super();
    this.index = index;
    this.style.color = colour;
  }

  setPosition(top, left) {
    this.style.top = top;
    this.style.left = left;
  }
}

customElements.define("colour-button", ColourButton, { extends: "button" });
