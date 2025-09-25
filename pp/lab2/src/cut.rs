use std::env;
use std::fs;
use std::io::{self, BufRead};

const C_MODE: &str = "-c";
const F_MODE: &str = "-f";
const COMMA: &str = ",";
const COLON: &str = ":";

// Start

#[derive(Debug)]
pub enum Range {
    N(usize),
    NToEnd(usize),
    NtoM(usize, usize),
    StartToM(usize),
}

impl Range {
    pub fn parse(s: &str) -> Option<Range> {
        fn parse_positive(s: &str) -> usize {
            let number = s.parse::<usize>().expect("Invalid usize");
            if number <= 0 {
                panic!("Range must be positive");
            }
            number
        }

        if s.contains(COLON) {
            let range_str: Vec<&str> = s.split(COLON).collect();

            match range_str.as_slice() {
                [n_str, ""] => Some(Range::NToEnd(parse_positive(n_str))),
                ["", m_str] => Some(Range::StartToM(parse_positive(m_str))),
                [n_str, m_str] => {
                    let n = parse_positive(n_str);
                    let m = parse_positive(m_str);
                    if n > m {
                        panic!("First value in range must not be larger than the second")
                    }
                    Some(Range::NtoM(n, m))
                }
                _ => panic!("Invalid Range Input"),
            }
        } else {
            Some(Range::N(parse_positive(s)))
        }
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
pub fn process_query(input: &str) -> Vec<&str> {
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

// fn readFile(file_name: &String) ->  {
// let file_content = fs::read_to_string(file_path).expect("Error reading file");
// }

fn validate_args(args: &Vec<String>) -> &str {
    if args.len() <= 1 {
        panic!("Usage: ./cut <-c|-f><ranges> <file>");
    }
    &args[1]
}

fn main() {
    //!TODO WHEN DONE
    let args: Vec<String> = env::args().collect();

    let query: &str = validate_args(&args);
    let range_strings: Vec<&str> = process_query(&query);

    let ranges: Vec<Range> = range_strings
        .iter()
        .map(|range_str| Range::parse(range_str).unwrap())
        .collect();

    let stdin = io::stdin();
    for line in stdin.lock().lines() {
        let line: String = line.unwrap();
        let mut result = String::new();

        println!("Got line: {}", line);

        for range in &ranges {
            let chars: Vec<char> = line.chars().collect();

            let slice = match range {
                Range::N(n) => {
                    if *n == 0 || *n > chars.len() {
                        "".to_string()
                    } else {
                        chars[n - 1..*n].iter().collect()
                    }
                }
                Range::NToEnd(n) => {
                    if *n == 0 || *n > chars.len() {
                        "".to_string()
                    } else {
                        chars[n - 1..].iter().collect()
                    }
                }
                Range::NtoM(n, m) => {
                    if *n == 0 || *n > chars.len() {
                        "".to_string()
                    } else {
                        let end = (*m).min(chars.len());
                        chars[n - 1..end].iter().collect()
                    }
                }
                Range::StartToM(m) => {
                    let end = (*m).min(chars.len());
                    chars[..end].iter().collect()
                }
            };
            println!("slice: {}", slice);
            result.push_str(&slice);
        }

        // PROGRAM ON LINES HERE
        println!("result {}", result);
    }

    println!("Ranges: {:?}", ranges);
}
