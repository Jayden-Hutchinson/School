enum Shape {
    Circle(f32, f32, f32),
    Square(f32),
}

impl Shape {
    fn area(&self) -> f32 {
        match self {
            Self::Circle(_, _, r) =>
                std::f32::consts::PI * r * r,
            Self::Square(x) =>
                x * x,
        }
    }

    fn print(&self) {
        match self {
            Self::Circle(x, y, r) =>
                println!("C: ({x}, {y}), {r}"),
            Self::Square(x) =>
                println!("S: {x}"),
        }
    }
}
 
fn total_area(v: &Vec<Shape>) -> f32 {
    let mut total = 0.0;

    for x in v {
        total += x.area();
    }
    total
}

fn main() {
    let v = vec![Shape::Circle(0.0, 0.0, 1.0), Shape::Square(2.0)];
    for x in &v {
        x.print();
    }
    println!("{}", total_area(&v));
}
