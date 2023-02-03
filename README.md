# testfilewrite

A poor person's simple writing benchmark. It based on options given it will write for N period of X size of bytes continuously and return the number of iterations. By default it will run synchronously and will only use a single writer (i.e. not multi-threaded).

A better solution for benchmarking perf would be use a real benchmarking tool like <https://github.com/microsoft/diskspd>

## raison d'être
trying to simulate the write pattern used by Milliman's Triton product which was doing 79 byte synchronous writes, which was much slower in cloud than on-premise.