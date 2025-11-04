package main

import (
	"bufio"
	"fmt"
	"log"
	"os"
	"strconv"
)

func main() {
	if len(os.Args) == 1 {
		return
	}
	capacity, err := strconv.Atoi(os.Args[1])
	if err != nil {
		log.Fatal("invalid capacity")
	}

	s := bufio.NewScanner(os.Stdin)
	s.Split(bufio.ScanBytes)

	n := 0
	for s.Scan() {
		switch s.Text() {
		case "+":
			n++
		case "-":
			n--
		}
		if n < 0 || n > capacity {
			fmt.Println("Nothing ever works! What do you expect?")
		}
	}
}
