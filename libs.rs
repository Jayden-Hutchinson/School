```mod utils;

use wasm_bindgen::prelude::*;
use rand::prelude::*;

#[wasm_bindgen]
#[derive(Clone, Copy, PartialEq, Eq)]
#[repr(u8)]

pub enum Cell {
    Dead = 0,
    Alive = 1,
    Dying = 2,
}

impl Cell {
    fn toggle(&mut self) {
        *self = match *self {
            Cell::Alive => Cell::Dead,
            Cell::Dead => Cell::Alive,
            Cell::Dying => Cell::Dead, // In case there are any Dying cells, convert them to Dead
        };
        
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
    fn get_index(&self, row: u32, col: u32) -> usize {
        (row * self.width + col) as usize
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
                    (Cell::Dead, 2) => Cell::Alive,  // Dead cell becomes alive with exactly 2 neighbors
                    (Cell::Alive, 2) => Cell::Alive, // Live cell stays alive with 2 neighbors
                    (Cell::Alive, 3) => Cell::Alive, // Live cell stays alive with 3 neighbors
                    (Cell::Alive, _) => Cell::Dying, // Live cell with wrong neighbor count becomes dying
                    (Cell::Dying, _) => Cell::Dead,  // Dying cell becomes dead
                    _ => Cell::Dead,                  // All other cases remain dead
                }
            }
        }
        self.cells = cells;
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

    pub fn new() -> Self {
        let width = 64;
        let height = 64;

        let mut rng = SmallRng::from_entropy();
        let cells = (0..width * height)
            .map(|_| {
                if rng.gen_bool(0.5) {
                    Cell::Alive
                } else {
                    Cell::Dead
                }
            })
            .collect();

        Self {
            cells,
            width,
            height,
        }
    }

    pub fn toggle_cell(&mut self, row: u32, col: u32) {
        let idx = self.get_index(row, col);
        self.cells[idx].toggle();
    }

    // Debug method to get neighbor count for a specific cell
    pub fn get_neighbor_count(&self, row: u32, col: u32) -> u8 {
        self.count_live_neighbours(row, col)
    }

    // Debug method to get cell state
    pub fn get_cell_state(&self, row: u32, col: u32) -> u8 {
        let idx = self.get_index(row, col);
        self.cells[idx] as u8
    }
}

use std::fmt;

impl fmt::Display for Universe {
    fn fmt(&self, f: &mut fmt::Formatter) -> fmt::Result {
        for r in 0..self.height {
            for c in 0..self.width {
                let symbol = if self.cells[self.get_index(r, c)] == Cell::Alive {
                    '■'
                } else {
                    '☐'
                };
                write!(f, "{symbol}")?;
            }
            write!(f, "\n")?;
        }
        Ok(())
    }
}```