use std::env;

const C_MODE: &str = "-c";
const F_MODE: &str = "-f";
const COMMA: &str = ",";
const COLON: &str = ":";

#[derive(Debug)]
pub enum Range {
    N(usize),
    NToEnd(usize),
    NtoM(usize, usize),
    StartToM(usize),
}

impl Range {
    pub fn parse(s: &str) -> Option<Range> {
        let mut range = None;
        if s.contains(COLON) {
            let range_str: Vec<&str> = s.split(COLON).collect();

            match range_str.as_slice() {
                [value, ""] => {
                    let number: usize = value.parse().expect("Invalid usize");
                    range = Some(Range::NToEnd(number));
                }

                ["", value] => {
                    let number: usize = value.parse().expect("Invalid usize");
                    range = Some(Range::StartToM(number));
                }
                [value1, value2] => {
                    let number1: usize = value1.parse().expect("Invalid usize");
                    let number2: usize = value2.parse().expect("Invalid usize");
                    range = Some(Range::NtoM(number1, number2));
                }
                _ => panic!("Invalid Range Input"),
            }
        } else {
            let number: usize = s.parse().expect("Invalid usize");
            if number <= 0 {
                panic!("Range must be positive")
            }

            range = Some(Range::N(number))
        }

        range
    }
    pub fn contains(&self, n: usize) -> bool {
        true
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
pub fn process_input(input: &String) -> Vec<&str> {
    if !(input.starts_with(C_MODE) || input.starts_with(F_MODE)) {
        panic!("Error: first argument must start with -c or -f flag");
    }

    if input.len() < 2 {
        panic!("Error: no ranges provided after flag");
    }

    let input_str: &str = &input[2..]; // remove -c or -f
    let vec_range_str: Vec<&str> = input_str.split(COMMA).collect();
    vec_range_str
}

fn main() {
    //!TODO UNCOMMENT WHEN DONE
    let _cmd_args: Vec<String> = env::args().collect();
    let args: Vec<String> = vec!["".to_string(), "-c1,2:,:3,4:5,10:11,12:14".to_string()];
    println!("Program Running: {}", &args[0]);

    if args.len() < 2 {
        panic!("Usage: ./cut <-c|-f><ranges> <file>");
    }

    let input: &String = &args[1];

    let range_strings: Vec<&str> = process_input(input);

    println!("Ranges: {:?}", range_strings);
    for range_str in range_strings.iter() {
        let range = Range::parse(range_str).unwrap();
        println!("{:?}", range);
    }
}
