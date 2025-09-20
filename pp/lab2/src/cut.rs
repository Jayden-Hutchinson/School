// command argument starts with -c or -f
// -c = cut
// -
// followed by one or more ranges separated by commas

// ranges = N(low) and M(high)

// preconditions:
// N <= M
// argument starts with -c or -f
// range is valid
// successive ranges separated by commas
// must have exactly one argument
// must have at least one range

// ex.
// $ ./cut -c2,:3,10,5:7,13: < data.txt
// $ ./cut -f:3,5,8 < data.txt
use std::env;

pub fn command_line() {
    let args: Vec<String> = env::args().collect();
    println!("Program running: {}", args[0]);

    for (i, arg) in args.iter().enumerate().skip(1) {
        println!("Argument {}: {}", i, arg);
    }
}

// enum Range {
//     Nth(u32),
//     NthToEndOfLine(u32),
//     NthtoMth(u32, u32),
//     StartToMth(u32),
// }

// impl Range {
//     fn parse(s: &str) -> Option<Range> {}
//     fn contains(&self, n: usize) -> bool {}
// }

// mod tests {
//     use super::*;

//     #[test]
//     fn test_() {}

//     #[test]
//     fn test() {}
// }
