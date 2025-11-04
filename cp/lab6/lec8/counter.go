package main

import (
	"fmt"
	"time"
	"sync"
)

var counter int
var mutex sync.Mutex // zero value is unlocked mutex

func inc() {
	for i := 0; i < 1000000; i++ {
		mutex.Lock()
		counter++
		mutex.Unlock()
	}
}

func main() {
	go inc()
	go inc()

	time.Sleep(2 * time.Second)
	fmt.Println(counter)
}
