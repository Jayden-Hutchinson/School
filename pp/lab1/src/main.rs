mod primes;
mod students;
use std::io;

fn main() -> io::Result<()> {
    students::print_lines()?;
    primes::primes(5, 6);
    Ok(())
}
