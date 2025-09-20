use std::io;

fn print_lines() {
    let stdin = io::stdin();
    let mut line = String::new();

    while let Ok(n) = stdin.read_line(&mut line) {
        if n == 0 {
            break;
        }
        print!("{line}");
        line.clear();
    }
}

fn lines() -> io::Result<Vec<String>> {
    let stdin = io::stdin();
    let mut line = String::new();
    let mut v = Vec::new();

    while stdin.read_line(&mut line)? > 0 {
        v.push(line.clone());
        line.clear();
    }
    Ok(v)
}

fn main() {
    for (n, l) in lines().unwrap().into_iter().enumerate() {
        print!("{}: {l}", n + 1);
    }
}

fn stdin_lines() -> io::Result<Vec<String>> {
    io::stdin().lines().collect()
}
