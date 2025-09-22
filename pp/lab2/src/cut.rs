use std::env;

const C_MODE: &str = "-c";
const F_MODE: &str = "-f";
const COMMA: &str = ",";

enum Range {
    N(u32),
    NToEnd(u32),
    NtoM(u32, u32),
    StartToM(u32),
}

// impl Range {
//     fn parse(s: &str) -> Option<Range> {}
//     fn contains(&self, n: usize) -> bool {}
// }

fn main() {
    // ranges = N(low) and M(high)
    // ex.
    // $ ./cut -c2,:3,10,5:7,13: < data.txt
    // $ ./cut -f:3,5,8 < data.txt
    let args: Vec<String> = env::args().collect();
    let input_ranges = process_args(&args);

    for (i, arg) in processed_args.iter().enumerate().skip(1) {
        println!("Argument {}: {}", i, arg);
    }
}

/// preconditions:
/// N <= M
/// argument starts with -c or -f
/// range is valid
/// successive ranges separated by commas
/// must have exactly one argument
/// must have at least one range
/// command argument starts with -c or -f
/// -c = cut
/// -
/// followed by one or more ranges separated by commas
fn process_args(args: &Vec<String>) -> Vec<Range> {
    println!("Program Running: {}", &args[0]);

    if args.len() < 2 {
        eprintln!("Usage: ./cut <-c | -f><ranges> <file>");
        std::process::exit(1);
    }

    let input: &String = &args[1];

    if !(input.starts_with(C_MODE) || input.starts_with(F_MODE)) {
        eprintln!("Error: first argument must start with -c or -f flag");
        std::process::exit(1);
    }

    if input.len() < 2 {
        eprintln!("Error: no ranges provided after flag");
        std::process::exit(1);
    }

    let ranges_str: &str = &input[2..]; // remove -c or -f
    let input_ranges: Vec<&str> = ranges.split(COMMA).collect();

    for input_range in input_ranges {
        let range = Range::parse(input_range).unwrap();
    }
    let range: Range = Range::parse(arg).unwrap();
    input_ranges
}

// mod tests {
//     use super::*;

//     #[test]
//     fn test_() {}

//     #[test]
//     fn test() {}
// }
