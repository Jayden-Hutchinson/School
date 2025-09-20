use std::ops::AddAssign;

fn main() {
    let v = vec![3, 2, 7, 6, 8];
    println!("{}", sum(&v));

    fn is_even(x: &i32) -> bool { x % 2 == 0 }
    if let Some(x) = find(&v, is_even) {
        println!("{x}");
    }

    let divisor = 6;

    /* doesn't compile; requires a closure in order to capture divisor
    fn is_divisible(x: &i32) -> bool { x % divisor == 0 }
    if let Some(x) = find(&v, is_divisible) {
        println!("{x}");
    }
    */

    /* doesn't compile because find takes a fn ptr not a closure
     * (unless the closure can be coerced into a fn ptr)
    if let Some(x) = find(&v, |x| x % divisor == 0) {
        println!("{x}");
    }
    */

    // since the closure doesn't capture anything, it can be coerced
    // into a fn ptr
    if let Some(x) = find(&v, |x| x % 2 == 0) {
        println!("{x}");
    }

    // find2 takes a closure
    if let Some(x) = find2(&v, |x| x % divisor == 0) {
        println!("{x}");
    }

    // as well as a fn ptr
    if let Some(x) = find2(&v, is_even) {
        println!("{x}");
    }
}


// precondition: s is not empty
fn sum<T: AddAssign + Clone>(s: &[T]) -> T {  // trait bound
    let mut total = s[0].clone();

    for x in s.iter().skip(1) {
        total += x.clone();
    }
    total.clone()
}

fn find<T>(s: &[T], f: fn(&T) -> bool) -> Option<&T> {// f must be a fn ptr
                                                      // can't accept a closure
    for x in s {
        if f(x) {
            return Some(x);
        }
    }
    None
}

fn find2<T, F>(s: &[T], f: F) -> Option<&T>
where F: Fn(&T) -> bool {
    for x in s {
        if f(x) {
            return Some(x);
        }
    }
    None
}

    
