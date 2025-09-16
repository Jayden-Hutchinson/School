mod students;
use std::io;

fn main() -> io::Result<()> {
    students::print_lines()?;
    Ok(())
}
