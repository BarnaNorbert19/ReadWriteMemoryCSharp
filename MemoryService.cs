public partial class MemoryService
    {
        private enum ProcessAccessTypes : uint
        {
            READ = 0x0010,
            ALL = 0x1F0FFF
        }

        public Process LocalProcess { get; private set; }

        [LibraryImport("kernel32.dll")]
        private static partial nint OpenProcess(ProcessAccessTypes dwDesiredAccess, [MarshalAs(UnmanagedType.Bool)] bool bInheritHandle, int dwProcessId);

        [LibraryImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static partial bool WriteProcessMemory(nint hProcess, nint lpBaseAddress, byte[] lpBuffer, uint nSize, out int lpNumberOfBytesWritten);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool ReadProcessMemory(nint hProcess, nint lpBaseAddress, [Out] byte[] lpBuffer, int dwSize, out int lpNumberOfBytesRead);

        [LibraryImport("kernel32.dll")]
        private static partial int CloseHandle(nint hProcess);


        public MemoryService(string processName)
        {
            LocalProcess = Process.GetProcessesByName(Path.GetFileNameWithoutExtension(processName))[0];
        }

        public byte[] ReadMemory(int address, int numOfBytes, out int bytesRead)
        {
            nint hProc = OpenProcess(ProcessAccessTypes.READ, false, LocalProcess.Id);

            byte[] buffer = new byte[numOfBytes];

            ReadProcessMemory(hProc, new nint(address), buffer, numOfBytes, out bytesRead);

            CloseHandle(hProc);
            return buffer;
        }

        public byte[] ReadMemory<T>(int address, out int bytesRead, ref int finalAddress, params int[] offsets)
        {
            int bytesToRead = SizeHelper.SizeOf(typeof(T));

            nint hProc = OpenProcess(ProcessAccessTypes.READ, false, LocalProcess.Id);

            byte[] buffer = new byte[bytesToRead];

            ReadProcessMemory(hProc, GetBaseAddress(LocalProcess.ProcessName) + new nint(address), buffer, bytesToRead, out bytesRead);
            int read = BitConverter.ToInt32(buffer, 0);

            for (int i = 0; i < offsets.Length; i++)
            {
                finalAddress = read + offsets[i];

                ReadProcessMemory(hProc, finalAddress, buffer, bytesToRead, out bytesRead);
                read = BitConverter.ToInt32(buffer, 0);
            }

            int error = CloseHandle(hProc);

            if (error != 0)
            {
                Console.WriteLine("Error while closing handle");
            }

            return buffer;
        }

        public byte[] ReadMemory<T>(long address, out int bytesRead, ref long finalAddress, params int[] offsets)
        {
            int bytesToRead = SizeHelper.SizeOf(typeof(T));

            nint hProc = OpenProcess(ProcessAccessTypes.READ, false, LocalProcess.Id);

            byte[] buffer = new byte[bytesToRead];

            ReadProcessMemory(hProc, GetBaseAddress(LocalProcess.ProcessName) + new nint(address), buffer, bytesToRead, out bytesRead);
            long read = BitConverter.ToInt64(buffer, 0);

            for (int i = 0; i < offsets.Length; i++)
            {
                finalAddress = read + offsets[i];

                ReadProcessMemory(hProc, new nint(finalAddress), buffer, bytesToRead, out bytesRead);
                read = BitConverter.ToInt64(buffer, 0);
            }

            int error = CloseHandle(hProc);

            if (error != 0)
            {
                Console.WriteLine("Error while closing handle");
            }

            return buffer;
        }
        public void WriteMemory(int address, byte[] bytes)
        {
            nint hProc = OpenProcess(ProcessAccessTypes.ALL, false, LocalProcess.Id);
            WriteProcessMemory(hProc, new nint(address), bytes, (uint)bytes.LongLength, out int numOfBytes);
            int error = CloseHandle(hProc);

            if (error != 0)
            {
                Console.WriteLine("Error while closing handle");
            }
        }

        public void WriteMemory(long address, byte[] bytes)
        {
            nint hProc = OpenProcess(ProcessAccessTypes.ALL, false, LocalProcess.Id);
            WriteProcessMemory(hProc, new nint(address), bytes, (uint)bytes.LongLength, out int numOfBytes);
            int error = CloseHandle(hProc);

            if (error != 0)
            {
                Console.WriteLine("Error while closing handle");
            }
        }

        public nint GetBaseAddress(string moduleName)
        {
            

            for (int i = 0; i < LocalProcess.Modules.Count; i++)
            {
                if (Path.GetFileNameWithoutExtension(LocalProcess.Modules[i].ModuleName) == moduleName)
                {
                    return LocalProcess.Modules[i].BaseAddress;
                }
            }

            return nint.Zero;
        }
    }
