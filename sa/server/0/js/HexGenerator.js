const HEX_LENGTH = 6;
const BASE_HEX = 16;
const HEX_SYMBOL = "#";
const HEX_LETTERS = "0123456789ABCDEF";
const START_INDEX = 0;

export class HexGenerator {
  static getRandomHex() {
    let colour = HEX_SYMBOL;
    for (let i = START_INDEX; i < HEX_LENGTH; i++) {
      let randomHexValue = Math.floor(Math.random() * BASE_HEX);
      colour += HEX_LETTERS[randomHexValue];
    }
    return colour;
  }
}
