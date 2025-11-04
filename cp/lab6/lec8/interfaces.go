package main

import "fmt"

type I interface {
	f() int
}

type S1 struct {}

func (s S1) f() int { return 1 }  // value receiver

type S2 struct {}

func (s *S2) f() int { return 2 }  // pointer receiver

func main() {
	var i1 I = S1{}
	var i2 I = &S1{}
	// var _ I = S2{} // pointer receiver is more restrictive
	var _ I = &S2{}
	
	x := i1.(S1) // type assertion
  fmt.Println(x)
	if x, ok := i2.(S1); ok {
		fmt.Println(x)
	}
}
