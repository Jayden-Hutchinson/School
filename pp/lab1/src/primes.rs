use std::collections::HashMap;

pub fn primes(m: usize, n: usize) -> Vec<usize> {
    let mut p: Vec<usize> = sieve(n);

    if m >= n {
        panic!("m > n");
    }

    p = p.iter().cloned().filter(|&x| x >= m && x <= n).collect();

    // PRINT TEST
    let mut groups: HashMap<String, Vec<usize>> = HashMap::new();

    for prime in &p {
        let mut digits: Vec<char> = prime.to_string().chars().collect();
        digits.sort_unstable();
        let key: String = digits.into_iter().collect();

        groups.entry(key).or_default().push(*prime);
    }
    let largest_group_size = groups.values().map(|v| v.len()).max().unwrap_or(0);

    println!("Total 6 digit primes: {}", p.len());
    println!("Largest Permutation Size: {}", largest_group_size);

    p
}

fn sieve(n: usize) -> Vec<usize> {
    if n < 2 {
        return Vec::new();
    }

    let mut is_prime: Vec<bool> = vec![true; n + 1];
    is_prime[0] = false;
    is_prime[1] = false;

    let sqrt_n = (n as f64).sqrt() as usize;

    for i in 2..=sqrt_n {
        if is_prime[i] {
            let mut multiple = i * i;
            while multiple <= n {
                is_prime[multiple] = false;
                multiple += i;
            }
        }
    }

    is_prime
        .iter()
        .enumerate()
        .filter_map(|(index, &prime)| if prime { Some(index) } else { None })
        .collect()
}
