mod primes;
mod students;
use std::io;

fn main() -> io::Result<()> {
    students::print_lines()?;
    let p = primes::primes(10, 100);
    print!("{:?}", p);
    Ok(())
}
