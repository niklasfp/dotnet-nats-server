package main

import (
	"C"
	"time"

	"github.com/nats-io/nats-server/v2/server"
)

var (
	instance server.Server
)

//export ShutdownServer
func ShutdownServer() {
	instance.Shutdown()
}

//export StartServer
func StartServer(configFile *C.char) C.int {
	opts := &server.Options{
		NoSigs: true,
		NoLog:  true,
	}

	// Initialize new server with options
	ns, err := server.NewServer(opts)

	if err != nil {
		return -1
	}

	ns.SetLogger(&logWrapper{}, false, false)

	// Start the server via goroutine
	go ns.Start()

	// Wait for up to 10 seconds for server to be ready for connections
	if !ns.ReadyForConnections(10 * time.Second) {
		return -2
	}

	instance = *ns

	return 0
}

func main() {
	StartServer(nil)
}
