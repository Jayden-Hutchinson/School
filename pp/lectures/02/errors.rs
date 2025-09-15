fn mysqrt(x: f32) -> Option<f32> {
    if x < 0.0 {
        None
    } else {
        Some(x.sqrt())
    }
}

fn sqrt_ln(x: f32) -> Option<f32> {
    Some(mysqrt(x)?.ln())
}

#[allow(unused)]
fn mysqrt2(x: f32) -> Result<f32, String> {
    if x < 0.0 {
        Err("negative argument".to_string())
    } else {
        Ok(x.sqrt())
    }
}

#[allow(unused)]
fn sqrt_ln2(x: f32) -> Result<f32, String> {
    Ok(mysqrt2(x)?.ln())
}

fn main() {
    // panic!("hell");

    println!("{}", mysqrt(5.0).unwrap());
    // println!("{}", mysqrt(-5.0).unwrap()); // panics

    println!("{}", mysqrt(5.0).expect("negative argument"));
    // println!("{}", mysqrt(-5.0).expect("negative argument"));  // panics

    match sqrt_ln(-1.0) {
        None => println!("No answer"),
        Some(x) => println!("{x}"),
    }
}
