import * as env from "./constants.js"


export class HexGenerator {
  randomHex() {
    let colour = env.HEX.symbol;
    for (let i = env.START_INDEX; i < env.HEX.digits; i++) {
      let randomHexValue = Math.floor(Math.random() * env.HEX.base);
      colour += env.HEX.letters[randomHexValue];
    }
    return colour;
  }
}
