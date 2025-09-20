fn main() {
    let s1 = "hello";  // type: &str
    let s2 = "world".to_string(); // type: String
                                
    print_str(s1);
    print_string_ref(&s2);
    print_string(s2);  // note: s2 will be moved

    let s = String::from("world");
    // print_string_ref(s1); // error
    print_str(&s);  // works because of defer coercion
}

fn print_str(s: &str) {  // works for both &str and &String
    println!("{s}");
}

fn print_string(s: String) {
    println!("{s}");
}

fn print_string_ref(s: &String) {  // only works for &String
    println!("{s}");
}
