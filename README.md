# RobotNES

Yet another Nintendo Entertainment System emulator written in C#.

Project skeleton only with partially implemented bus and memory systems. The basics of the CPU emulation without most of the instruction emulation.

## Partially implemented

- System bus
- Cartridge loading (ines 1.0 format)
- System memory with mirroring
- CPU emulation architecture

Currently working on the instruction set implementation.

18 opcodes are implemented out of 151:
![](https://geps.dev/progress/12)