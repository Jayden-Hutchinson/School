// we can't add methods to a type that we don't implement
// but we can have it implement a trait (extension trait)
trait Square {
    fn square(self) -> Self;
}

impl Square for i32 {
    fn square(self) -> Self { self * self }
}

fn main() {
    println!("{}", 42.square());
}
