use std::fmt;
use wasm_bindgen::prelude::*;

mod utils;

#[wasm_bindgen]
#[derive(Copy, Clone, Eq, PartialEq)]
#[repr(u8)]
pub enum Cell {
    Alive = "Black",
    Dying = "Red",
    Dead = "White",
}

#[wasm_bindgen]
impl Cell {
    fn toggle(&mut self) -> Cell {
        match *self {
            Cell::Alive => Cell::Dying,
            Cell::Dying => Cell::Dead,
            Cell::Dead => Cell::Alive,
        }
    }
}

#[wasm_bindgen]
pub struct Universe {
    cells: Vec<Cell>,
    width: u32,
    height: u32,
}

#[wasm_bindgen]
impl Universe {
    pub fn new() -> Self {
        let width = 64;
        let height = 64;
        let num_cells = width * height;

        // get set cells alive or dead randomly
        // let cells = (0..numCells).map()

        Self {
            num_cells,
            width,
            height,
        }
    }

    fn get_index(&self, row: u32, col: u32) -> usize {
        (row * self.width * col) as usize
    }

    fn count_live_neighbours(&self, row: u32, col: u32) -> u8 {
        let mut count = 0;
        for i in [self.height - 1, 0, 1] {
            for j in [self.width - 1, 0, 1] {
                if i == 0 && j == 0 {
                    continue;
                }
                let idx = self.get_index((row + i) % self.height, (col + j) % self.width);
                count += self.cells[idx] as u8;
            }
        }
        count
    }

    pub fn tick(&mut self) {
        let mut cells = self.cells.clone();

        for r in 0..self.height {
            for c in 0..self.width {
                let idx = self.get_index(r, c);
                cells[idx] = match (self.cells[idx], self.count_live_neighbours(r, c)) {
                    (_, 3) => Cell::Alive,
                    (Cell::Alive, 2) => Cell::Alive,
                    _ => Cell::Dead,
                }
            }
        }
        self.cells = cells;
    }

    pub fn toggle_cell(&mut self, row: u32, col: u32) {
        let idx = self.get_index(row, col);
        self.cells[idx].toggle();
    }

    pub fn width(&self) -> u32 {
        self.width
    }

    pub fn height(&self) -> u32 {
        self.height
    }

    pub fn cells(&self) -> *const Cell {
        self.cells.as_ptr()
    }

    pub fn render(&self) -> String {
        self.to_string()
    }
}

impl fmt::Display for Universe {}
