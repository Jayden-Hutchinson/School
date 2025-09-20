// how to implement an iterator for some vector
struct Veci32Iter {
    v: Vec<i32>,
    n: usize,
}

impl Veci32Iter {
    fn into_iter(v: Vec<i32>) -> Self {
        Self {
            v: v,
            n: 0,
        }
    }
}

impl Iterator for Veci32Iter {
    type Item = i32;

    fn next(&mut self) -> Option<Self::Item> {
        if self.n >= self.v.len() {
            return None;
        } else {
            let x = self.v[self.n];
            self.n += 1;
            Some(x)
        }
    }
}

fn main() {
    let v = vec![3,2,7,6,8];
    let mut it = Veci32Iter::into_iter(v);

    while let Some(x) = it.next() {
        println!("{x}");
    }
}
