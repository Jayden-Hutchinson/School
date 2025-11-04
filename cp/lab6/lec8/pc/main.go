package main

import (
	"rust/pc/utils"
)

func consumer(b *utils.Buffer) {
	for {
		b.Get()
	}
}

func producer(b *utils.Buffer) {
	for {
		b.Put()
	}
}

func main() {
	b := utils.NewBuffer(100)
  go consumer(b)
  go consumer(b)
  go consumer(b)
	go producer(b)
	go producer(b)

	<-make(chan int)
}
