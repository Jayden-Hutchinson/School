class ColourButton extends HTMLButtonElement {
  constructor(index, colour, width, height, top, left) {
    super();

    this.index = index;
    this.style.color = colour;
    this.style.width = width;
    this.style.height = height;
    this.style.position = "absolute";
    this.style.top = top;
    this.style.left = left;
  }

  setPosition(top, left) {
    this.style.top = top;
    this.style.left = left;
  }
}

customElements.define("colour-button", ColourButton, { extends: "button" });
