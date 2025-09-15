fn main() {
    let v = vec![3, 2, 7, 6, 8];
    print(v);
    // print(v);  // Error: v moved

    let mut v = vec![3, 2, 7, 6, 8];
    double(&mut v);
    print(v);

    let v = vec![3, 2, 7, 6, 8];
    let v = double2(v);
    println!("{v:?}");

    println!("{:?}", v.last()); // last is a method for slices
}

fn print(v: Vec<i32>) {
    for x in v {
        println!("{x}");
    }
}

fn double(v: &mut Vec<i32>) {
    for x in v {
        *x *= 2;
    }
}

fn double2(mut v: Vec<i32>) -> Vec<i32> {
    for x in &mut v {
        *x *= 2;
    }
    v
}

// into_iter(), iter(), iter_mut()
