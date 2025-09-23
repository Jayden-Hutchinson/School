const C_MODE: &str = "-c";
const F_MODE: &str = "-f";
const COMMA: &str = ",";

pub enum Range {
    N(u32),
    NToEnd(u32),
    NtoM(u32, u32),
    StartToM(u32),
}

impl Range {
    pub fn parse(s: &str) -> Option<Range> {
        None
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
        eprintln!("Error: first argument must start with -c or -f flag");
        std::process::exit(1);
    }

    if input.len() < 2 {
        eprintln!("Error: no ranges provided after flag");
        std::process::exit(1);
    }

    let input_str: &str = &input[2..]; // remove -c or -f
    let vec_range_str: Vec<&str> = input_str.split(COMMA).collect();
    vec_range_str
}
