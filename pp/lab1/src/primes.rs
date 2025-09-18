pub fn primes(m: usize, n: usize) -> Vec<usize> {
    let mut p: Vec<usize> = sieve(n);

    if m >= n {
        panic!("m > n");
    }

    p = p.iter().cloned().filter(|&x| x >= m && x <= n).collect();

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
