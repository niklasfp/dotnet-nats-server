package main

import (
	// #include <stdlib.h>
	// typedef void (*loggerFunc)(const char* msg);
	// void bridge_logger(loggerFunc f, const char* );
	"C"
	"fmt"
	"unsafe"
)

var loggingFunctionPointer C.loggerFunc

//export RegisterLogger
func RegisterLogger(iFunctionPointer C.loggerFunc) {
	loggingFunctionPointer = iFunctionPointer
}

func LogMessage(message string) {
	if loggingFunctionPointer != nil {

		cmsg := C.CString(message)

		defer C.free(unsafe.Pointer(cmsg))

		// this actually calls the registered C function pointer and logs
		C.bridge_logger(loggingFunctionPointer, cmsg)
	}
}

type logWrapper struct {
}

func (lw *logWrapper) Noticef(format string, v ...interface{}) {
	LogMessage(fmt.Sprintf(format, v...))
}

func (lw *logWrapper) Fatalf(format string, v ...interface{}) {
	LogMessage(fmt.Sprintf(format, v...))
}

func (lw *logWrapper) Warnf(format string, v ...interface{}) {
	LogMessage(fmt.Sprintf(format, v...))
}

func (lw *logWrapper) Errorf(format string, v ...interface{}) {
	LogMessage(fmt.Sprintf(format, v...))
}

func (lw *logWrapper) Debugf(format string, v ...interface{}) {
	LogMessage(fmt.Sprintf(format, v...))
}

func (lw *logWrapper) Tracef(format string, v ...interface{}) {
	LogMessage(fmt.Sprintf(format, v...))
}
