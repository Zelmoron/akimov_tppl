package main

import (
	"context"
	"fmt"
	"time"
)

func producer(ch chan int, done chan bool) {
	for i := 1; i <= 10; i++ {
		ch <- i
		time.Sleep(90 * time.Millisecond)
	}
	done <- true
}

func consumer(ctx context.Context, ch chan int, done chan bool, end chan struct{}) {
	for {
		select {
		case val := <-ch:
			fmt.Printf("Получено: %d\n", val)
		case <-done:
			fmt.Println("Завершение работы")
			end <- struct{}{}
			return
		case <-ctx.Done():
			fmt.Println("Таймаут: превышено время ожидания")
			end <- struct{}{}
			return
		}
	}
}

func main() {
	ch := make(chan int)
	done := make(chan bool)
	end := make(chan struct{})

	ctx, cancel := context.WithTimeout(context.Background(), 1000*time.Millisecond)
	defer cancel()

	go producer(ch, done)
	go consumer(ctx, ch, done, end)

	<-end
}
