use std::env;
use std::io::{self, BufRead};
use std::panic;

const C_MODE: &str = "-c";
const F_MODE: &str = "-f";
const COMMA: &str = ",";
const COLON: &str = ":";
const TAB: &str = "\t";

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
                        panic!("Invalid range: start > end")
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
        match self {
            Range::N(x) => n == *x,
            Range::NToEnd(x) => n >= *x,
            Range::NtoM(start, end) => n >= *start && n <= *end,
            Range::StartToM(end) => n <= *end,
        }
    }
}

fn main() {
    let args: Vec<String> = env::args().collect();

    let query: &str = validate_args(&args);
    let ranges: Vec<Range> = process_query(&query);
    let is_char_mode = query.starts_with(C_MODE);

    let stdin = io::stdin();
    for line in stdin.lock().lines() {
        let line: String = line.unwrap();
        let result = if is_char_mode {
            cut_chars(&line, &ranges)
        } else {
            cut_fields(&line, &ranges)
        };

        println!("{}", result)
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
pub fn process_query(query: &str) -> Vec<Range> {
    if !(query.starts_with(C_MODE) || query.starts_with(F_MODE)) {
        panic!("Error: first argument must start with -c or -f flag");
    }

    if query.len() < 2 {
        panic!("Error: no ranges provided after flag");
    }

    let ranges_str: &str = &query[2..]; // remove -c or -f
    ranges_str
        .split(COMMA)
        .map(|s| Range::parse(s).unwrap())
        .collect()
}

fn validate_args(args: &Vec<String>) -> &str {
    if args.len() <= 1 {
        panic!("Usage: ./cut <-c|-f><ranges> <file>");
    }

    if args.len() > 2 {
        panic!("Usage: one argument allowed")
    }
    &args[1]
}

fn cut_chars(line: &str, ranges: &[Range]) -> String {
    line.chars()
        .enumerate()
        .filter(|(i, _)| ranges.iter().any(|r| r.contains(i + 1)))
        .map(|(_, c)| c)
        .collect()
}

fn cut_fields(line: &str, ranges: &[Range]) -> String {
    line.split(TAB)
        .enumerate()
        .filter(|(i, _)| !ranges.iter().any(|r| r.contains(*i)))
        .map(|(_, field)| field)
        .collect::<Vec<_>>()
        .join(TAB)
}
