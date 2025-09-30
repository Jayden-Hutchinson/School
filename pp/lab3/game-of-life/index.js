// init corresponds to the default export __wbg_init in game_of_life.js
import init, { Universe } from "./pkg/game.js";

const pre = document.getElementById("game-of-life-canvas");

init().then(() => {
  const universe = Universe.new();
  pre.textContent = universe.render();

  const renderLoop = () => {
    pre.textContent = universe.render();
    universe.tick();

    requestAnimationFrame(renderLoop);
  };

  requestAnimationFrame(renderLoop);
});
