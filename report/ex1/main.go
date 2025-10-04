package main

import (
	"log"
	"time"
)

func Worker() {
	log.Println("worker start")
}

func main() {
	go Worker()
	time.Sleep(time.Second)
}
