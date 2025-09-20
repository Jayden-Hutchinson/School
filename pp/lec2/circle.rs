struct Point(f32, f32);  // tuple struct

struct Circle {
    center: Point,
    radius: f32,
}

impl Circle {
    fn new(center: Point, radius: f32) -> Self {
        Self {
            center,
            radius,
        }
    }

    fn area(&self) -> f32 {
        std::f32::consts::PI * self.radius * self.radius
    }
}

fn main() {
    let c = Circle::new(Point(1.0, 2.0), 1.0);
    println!("{}", c.area());
    println!("{}", Circle::area(&c));
}
