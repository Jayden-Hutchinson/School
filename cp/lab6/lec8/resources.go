// multiple goroutines sharing a resoure (e.g. 5 printers)
package main

import (
	"fmt"
	"sync"
	"time"
	"math/rand"
)

type Resource struct {
	navail int
	cond *sync.Cond
}

func NewResource(capacity int) *Resource {
  return &Resource{navail: capacity, cond: sync.NewCond(&sync.Mutex{})}
}

func (r *Resource) Acquire() {
	r.cond.L.Lock()
	for r.navail == 0 {
		r.cond.Wait()
	}
  r.navail--
	fmt.Println(r.navail)
	r.cond.L.Unlock()
}

func (r *Resource) Release() {
	r.cond.L.Lock()
	r.navail++
	fmt.Println(r.navail)
	r.cond.Signal()
	r.cond.L.Unlock()
}

func work(min, max int) {
  r := rand.Intn(max - min)
	time.Sleep(time.Duration(min + r) * time.Millisecond)
}

func worker(r *Resource) {
	for i := 0; i < 100; i++ {
		work(200, 500)
		r.Acquire()
		work(500, 1000)
		r.Release()
	}
}

func main() {
	r := NewResource(3)
	go worker(r)
	go worker(r)
	go worker(r)
	go worker(r)

  <- make(chan int)
}

