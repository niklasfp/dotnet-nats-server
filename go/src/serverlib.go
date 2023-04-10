package main

import (
	// #include <stdint.h> // for uintptr_t
	// typedef void (*loggerFunc)(char* msg, int level);
	"C"
	"flag"
	"log"
	"runtime/cgo"
	"strings"
	"time"

	"github.com/nats-io/nats-server/v2/server"
)
import "fmt"

//export CreateServer
func CreateServer(arguments *C.char, loggerFuncPtr C.loggerFunc) C.uintptr_t {
	libname := "nats-serverlib"

	var (
		opts *server.Options
	)

	if arguments == nil {
		log.Printf("%s: %s", libname, "Missing arguments.")
		return 0
	}

	args := strings.Split(C.GoString(arguments), "`")

	if len(args) > 0 && args[0] != "" {

		// Create a FlagSet
		fs := flag.NewFlagSet(libname, flag.ContinueOnError)

		var err error
		// Configure the options from the flags/config file
		opts, err = server.ConfigureOptions(fs, args[0:],
			nil,
			nil,
			nil)

		if err != nil {
			log.Printf("%s: %s", libname, err)
			return 0
		} else if opts.CheckConfig {
			log.Printf("%s: configuration file %s is valid\n", libname, opts.ConfigFile)
			return 0
		}

	} else {
		opts = &server.Options{}
	}

	opts.NoSigs = true // We dont want signals like ctrl+c and such to be enabled in embedded mode.
	opts.NoLog = true  // Also we register our own logger, so no need for that to be enabled.

	// Initialize new server with options
	ns, err := server.NewServer(opts)

	if err != nil {
		log.Printf("%s: %s", libname, err)
		return 0
	}

	ns.SetLoggerV2(&logWrapper{FunctionPointer: loggerFuncPtr}, false, false, false)

	handle := C.uintptr_t(cgo.NewHandle(ns))

	return handle
}

//export StartServer
func StartServer(handle C.uintptr_t) int {

	h := cgo.Handle(handle)
	ns := h.Value().(*server.Server)

	// Start the server via goroutine
	go ns.Start()

	// Wait for up to 10 seconds for server to be ready for connections
	if !ns.ReadyForConnections(10 * time.Second) {
		return -1
	}

	return 0
}

//export ShutdownServer
func ShutdownServer(handle C.uintptr_t) int {

	h := cgo.Handle(handle)
	ns := h.Value().(*server.Server)

	ns.Shutdown()

	return 0
}

//export FreeServer
func FreeServer(handle C.uintptr_t) {
	h := cgo.Handle(handle)
	h.Delete()
}

func main() {
	println("Starting nast-serverlib")
	//cargs := C.CString("-js")

	cargs := C.CString("")
	nsHandle := CreateServer(cargs, nil)

	StartServer(nsHandle)

	println("Press <enter> to shutdown server")
	fmt.Scanln()

	ShutdownServer(nsHandle)

	println("Press <enter> to exit")
	fmt.Scanln()
}
