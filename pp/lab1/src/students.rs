use std::fs::File;
use std::io;
use std::io::BufReader;
use std::io::prelude::*;

pub fn print_lines() -> io::Result<()> {
    let file = File::open("student_data.txt")?;
    let reader = BufReader::new(file);

    for line in reader.lines() {
        println!("{}", line?);
    }
    Ok(())
}
