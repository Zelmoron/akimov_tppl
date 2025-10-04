package main

import (
	"log"
	"sync"
	"time"
)

type Goroutine struct {
	result1 []int
	result2 []int
	mu      sync.Mutex
}

func (g *Goroutine) WithoutGoroutine(number int) {
	if isPrime(number) {
		g.result1 = append(g.result1, number)
	}
}

func (g *Goroutine) WithGoroutine(number int) {
	if isPrime(number) {
		g.mu.Lock()
		g.result2 = append(g.result2, number)
		g.mu.Unlock()
	}
}

func main() {
	w := &Goroutine{
		result1: make([]int, 0),
		result2: make([]int, 0),
	}

	// Без горутин
	now := time.Now()
	for i := 0; i < 1000000; i++ {
		w.WithoutGoroutine(i)
	}
	log.Printf("Без горутин: %d чисел за %v", len(w.result1), time.Since(now))

	// С горутинами
	now = time.Now()
	var wg sync.WaitGroup
	numGoroutines := 500000
	chunkSize := 1000000 / numGoroutines

	for i := 0; i < numGoroutines; i++ {
		wg.Add(1)
		start := i * chunkSize
		end := start + chunkSize

		go func(s, e int) {
			defer wg.Done()
			for num := s; num < e; num++ {
				w.WithGoroutine(num)
			}
		}(start, end)
	}

	wg.Wait()
	log.Printf("С горутинами: %d чисел за %v", len(w.result2), time.Since(now))
}

func isPrime(n int) bool {
	if n < 2 {
		return false
	}
	for i := 2; i*i <= n; i++ {
		if n%i == 0 {
			return false
		}
	}
	return true
}
