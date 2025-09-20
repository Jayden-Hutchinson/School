// how to implement an iterator for some vector
struct VecIntoIter<T> {
    v: Vec<T>,
    n: usize,
}

impl<T> VecIntoIter<T> {
    fn into_iter(v: Vec<T>) -> Self {
        Self {
            v: v,
            n: 0,
        }
    }
}

impl<T: Clone> Iterator for VecIntoIter<T> {
    type Item = T;

    fn next(&mut self) -> Option<Self::Item> {
        if self.n >= self.v.len() {
            return None;
        } else {
            let x = self.v[self.n].clone();
            self.n += 1;
            Some(x)
        }
    }
}

struct VecIter<'a, T> {
    v: &'a Vec<T>,
    n: usize,
}

impl<'a, T> VecIter<'a, T> {
    fn iter(v: &'a Vec<T>) -> Self {
        Self {
            v: v,
            n: 0,
        }
    }
}

impl<'a, T> Iterator for VecIter<'a, T> {
    type Item = &'a T;

    fn next(&mut self) -> Option<Self::Item> {
        if self.n >= self.v.len() {
            return None;
        } else {
            let x = &self.v[self.n];
            self.n += 1;
            Some(x)
        }
    }
}

fn main() {
    let v = vec![3,2,7,6,8];
    let mut it = VecIter::<i32>::iter(&v);

    while let Some(x) = it.next() {
        println!("{x}");
    }

    println!("{v:?}");

/*
    let mut it = VecIntoIter::<i32>::into_iter(v);

    while let Some(x) = it.next() {
        println!("{x}");
    }

    println!("{v:?}");
*/
}

fn max<'a>(x: &'a i32, y: &'a i32) -> &'a i32 {
    if *x > *y {
        x
    } else {
        y
    }
}   
