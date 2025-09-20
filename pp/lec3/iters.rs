fn main() {
    let mut v = vec![1, 2, 3];

    for x in &v {  // use IntoIterator::into_iter()
        println!("{x}");
    }

    for x in &mut v {
        *x *= 2;
    }
    println!("{v:?}");

    for x in v {
        println!("{x}");
    }
}

// Vec has 3 iterators
// into_iter(), iter(), iter_mut()

// IntoIterator trait specifies into_iter() method
// Vec implements IntoIterator by having into_iter() call its into_iter()
// &Vec                                              call iter() 
// &mut Vec                                          call iter_mut()

