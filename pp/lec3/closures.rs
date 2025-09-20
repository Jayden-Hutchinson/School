fn main() {
    let mut v = vec![1, 2, 3, 4];
    let c = || println!("{v:?}");  // implements Fn
    c();
    c();
    println!("{v:?}");

    let mut c = |x| v.push(x);  // implements FnMut 
    c(-1);
    c(-2);
    println!("{v:?}");

    let c = || drop(v);  // implements FnOnce
    c();
    // println!("{v:?}");  // error: v moved
    // c();  // error: c is FnOnce

    let v = vec![1, 2, 3];
    let c = move || println!("{v:?}");  // v moved into closure; still Fn
    c();
    c();
    println!("{v:?}");
}
