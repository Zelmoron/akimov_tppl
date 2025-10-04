package main

import (
	"fmt"
	"sync"
)

var (
	wg = &sync.WaitGroup{}
	mu = &sync.Mutex{}
)

func Worker(count *int) {
	defer wg.Done()
	mu.Lock()
	*count++
	mu.Unlock()

}

func main() {

	count := 0
	for i := 0; i < 1000; i++ {
		wg.Add(1)
		go Worker(&count)
	}

	wg.Wait()
	fmt.Println(count)

}
