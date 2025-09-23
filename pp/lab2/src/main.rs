use cut::cut;
use std::env;

fn main() {
    //!TODO UNCOMMENT WHEN DONE
    // let cmd_args: Vec<String> = env::args().collect();
    let args: Vec<String> = vec!["".to_string(), "-c1,2:,:3,4:5".to_string()];
    println!("Program Running: {}", &args[0]);

    if args.len() < 2 {
        eprintln!("Usage: ./cut <-c|-f><ranges> <file>");
        std::process::exit(1);
    }

    let input: &String = &args[1];

    let range_strings: Vec<&str> = cut::process_input(input);

    println!("Ranges: {:?}", range_strings);
    for range_str in range_strings.iter() {
        let _range = cut::Range::parse(range_str);
    }
}
