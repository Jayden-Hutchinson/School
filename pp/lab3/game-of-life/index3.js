// with play/pause button
import init, { Cell, Universe } from "./pkg/game_of_life.js";

const CELL_SIZE = 5; //px
const GRID_COLOR = "#CCCCCC";
const DEAD_COLOR = "#FFFFFF";
const ALIVE_COLOR = "#000000";
const DYING_COLOR = "#FF0000";

init().then((wasm) => {
  // note wasm here
  const universe = Universe.new();
  const width = universe.width();
  const height = universe.height();

  const playPauseButton = document.getElementById("play-pause");
  console.log(playPauseButton);

  const stepButton = document.getElementById("step-button");
  stepButton.textContent = "Step";

  const canvas = document.getElementById("game-of-life-canvas");
  canvas.height = (CELL_SIZE + 1) * height + 1;
  canvas.width = (CELL_SIZE + 1) * width + 1;

  const ctx = canvas.getContext("2d");

  let animationId = null;

  const renderLoop = () => {
    universe.tick();

    drawGrid();
    drawCells();

    animationId = requestAnimationFrame(renderLoop);
  };

  const isPaused = () => {
    return animationId === null;
  };

  const play = () => {
    playPauseButton.textContent = "⏸";
    renderLoop();
  };

  const pause = () => {
    playPauseButton.textContent = "▶";
    cancelAnimationFrame(animationId);
    animationId = null;
  };

  playPauseButton.addEventListener("click", (event) => {
    if (isPaused()) {
      play();
    } else {
      pause();
    }
  });

  stepButton.addEventListener("click", (event) => {
    if (isPaused()) {
      universe.tick();
      drawGrid();
      drawCells();
    }
  });

  const drawGrid = () => {
    ctx.beginPath();
    ctx.strokeStyle = GRID_COLOR;

    // vertical lines
    for (let i = 0; i <= width; i++) {
      ctx.moveTo(i * (CELL_SIZE + 1) + 1, 0);
      ctx.lineTo(i * (CELL_SIZE + 1) + 1, (CELL_SIZE + 1) * height + 1);
    }

    //horizontal lines
    for (let j = 0; j <= height; j++) {
      ctx.moveTo(0, j * (CELL_SIZE + 1) + 1);
      ctx.lineTo((CELL_SIZE + 1) * width + 1, j * (CELL_SIZE + 1) + 1);
    }

    ctx.stroke();
  };

  const getIndex = (row, column) => {
    return row * width + column;
  };

  const drawCells = () => {
    const cellsPtr = universe.cells();
    // note use of wasm below to access memory buffer
    const cells = new Uint8Array(wasm.memory.buffer, cellsPtr, width * height);

    ctx.beginPath();

    for (let row = 0; row < height; row++) {
      for (let col = 0; col < width; col++) {
        const idx = getIndex(row, col);

        if (cells[idx] === Cell.Alive) {
          ctx.fillStyle = ALIVE_COLOR;
        } else if (cells[idx] === Cell.Dying) {
          ctx.fillStyle = DYING_COLOR;
        } else {
          ctx.fillStyle = DEAD_COLOR;
        }

        ctx.fillRect(
          col * (CELL_SIZE + 1) + 1,
          row * (CELL_SIZE + 1) + 1,
          CELL_SIZE,
          CELL_SIZE
        );
      }
    }
    ctx.stroke();
  };

  drawGrid();
  drawCells();
  play();
});
