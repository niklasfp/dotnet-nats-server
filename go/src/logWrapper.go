package main

import (
	// #include <stdlib.h>
	// typedef void (*loggerFunc)(char* msg, int level);
	// void bridge_logger(loggerFunc f, char*, int level);
	"C"
	"fmt"
)
import (
	"log"
	"unsafe"
)

// Logs messages on the registered logger using dotnet log levels.
type logWrapper struct {
	FunctionPointer C.loggerFunc
}

func (lw *logWrapper) Tracef(format string, v ...interface{}) {
	lw.LogMessage(fmt.Sprintf(format, v...), 0) // LogLevel.Trace
}

func (lw *logWrapper) Debugf(format string, v ...interface{}) {
	lw.LogMessage(fmt.Sprintf(format, v...), 1) // LogLevel.Debug
}

func (lw *logWrapper) Noticef(format string, v ...interface{}) {
	lw.LogMessage(fmt.Sprintf(format, v...), 2) // LogLevel.Information
}

func (lw *logWrapper) Fatalf(format string, v ...interface{}) {
	lw.LogMessage(fmt.Sprintf(format, v...), 5) // LogLevel.Critical
}

func (lw *logWrapper) Warnf(format string, v ...interface{}) {
	lw.LogMessage(fmt.Sprintf(format, v...), 4) // LogLevel.Warning
}

func (lw *logWrapper) Errorf(format string, v ...interface{}) {
	lw.LogMessage(fmt.Sprintf(format, v...), 4) // LogLevel.Error
}

func (lw *logWrapper) LogMessage(message string, level int32) {

	if lw.FunctionPointer != nil {

		cmsg := C.CString(message)
		defer C.free(unsafe.Pointer(cmsg))

		// this actually calls the registered C function pointer and logs
		C.bridge_logger(lw.FunctionPointer, cmsg, C.int(level))

	} else {
		log.Printf("[%d] %s", level, message)
	}
}
