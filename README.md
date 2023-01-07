# ReadWriteMemoryCSharp

Simple classes to read and write memory in C#

### 64 bit
In case you want to write to a 64 bit process change the platform target to x64 and use 8 bit types

## Usage
```csharp
MemoryService memory = new("process_name"); //Create instance, give the process name, extension will be stripped off
long address = 0; //the currect dynamic address if there is at least an offset, otherwise (using static addresses or something else) there's no need for this variable
var read = memory.ReadMemory<long>(0x05475D28, out int bytes, ref address, 0x38, 0x30, 0x48, 0xF0, 0x2A8, 0xF0, 0xF30); //read address value
var test = BitConverter.ToSingle(read); //convert byte[] to float or whatever you want
memory.WriteMemory(address, BitConverter.GetBytes(2000f)); //write to a address
```
