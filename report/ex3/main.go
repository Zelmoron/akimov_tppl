package main

import (
	"log"
	"sync"
)

func Worker(wg *sync.WaitGroup) {
	defer wg.Done()
	for i := 0; i < 100000; i++ {
		log.Println("worker", i)
	}
}
func main() {
	wg := &sync.WaitGroup{}
	wg.Add(2)
	go Worker(wg)

	wg.Wait()
}
