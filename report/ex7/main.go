package main

import (
	"log"
	"sync"
)

const GOROUTINE = 3

var wg = &sync.WaitGroup{}

type Checker struct {
	number int
	id     int
}

func Worker(ch chan<- *Checker, id int) {
	defer wg.Done()
	for i := 0; i < 3; i++ {
		ch <- &Checker{number: i, id: id}
	}

}

func main() {
	ch := make(chan *Checker)

	for i := 0; i < GOROUTINE; i++ {
		wg.Add(1)
		go Worker(ch, i)
	}

	go func() {
		wg.Wait()
		close(ch)
	}()

	for v := range ch {
		log.Println(v.number, v.id)
	}
}
