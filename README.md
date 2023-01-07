# ReadWriteMemoryCSharp

Simple classes to read and write memory in C#

### 64 bit
In case you want to write to a 64 bit process change the platform target to x64 and use 8 bit types

## Usage
```csharp
MemoryService memory = new("process_name");
long address = 0;
var read = memory.ReadMemory<long>(0x05475D28, out int bytes, ref address, 0x38, 0x30, 0x48, 0xF0, 0x2A8, 0xF0, 0xF30);
var test = BitConverter.ToSingle(read);
memory.WriteMemory(address, BitConverter.GetBytes(2000f));
```
