mod utils;

use wasm_bindgen::prelude::*;

#[wasm_bindgen]
#[derive(Copy, Clone, Eq, PartialEq)]
#[repr(u8)]
pub enum Cell {
    Alive,
    Dying,
    Dead,

}

#[wasm_bindgen]
impl Cell {
    fn toggle(&mut self) {
        *self = if *self == Cell::Alive {
            Cell::Dead
        } else {
            Cell::Alive
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
                    (Cell::Alive, _) => Cell::Dying,
                    (Cell::Dying, _) => Cell::Dead,
                    (Cell::Dead, 2 ) => Cell::Alive
                    other => other,
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

        let cells = (0..width * height)
            .map(|i| {
                if i % 2 == 0 || i % 7 == 0 {
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
}
