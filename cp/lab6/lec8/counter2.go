package main

import (
	"fmt"
	"sync"
)

type Counter struct {
	value int
	mutex sync.Mutex
}

func NewCounter(start int) *Counter {
  c := new(Counter)
	c.value = start
	c.mutex = sync.Mutex{} // not really necessary
  return c
}

/* alternative version
func NewCounter(start int) *Counter {
	return &Counter{value: start}
}
*/

func (c *Counter) Inc() {
	c.mutex.Lock()
	c.value++
	c.mutex.Unlock()
}

func inc(c *Counter, wg *sync.WaitGroup) {
	defer wg.Done()
	for i := 0; i < 1000000; i++ {
		c.Inc()
	}
}

func main() {
	var wg sync.WaitGroup
	c := NewCounter(0)
	wg.Add(2)
	go inc(c, &wg)
	go inc(c, &wg)

	wg.Wait()
	fmt.Println(c.value)
}
