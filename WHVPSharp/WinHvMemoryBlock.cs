using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace WHVPSharp
{
    public class WinHvMemoryBlock : IDisposable
    {
        #region Win32APIs

        [Flags]
        private enum AllocationType
        {
            Commit = 0x1000,
            Reserve = 0x2000,
            Decommit = 0x4000,
            Release = 0x8000,
            Reset = 0x80000,
            Physical = 0x400000,
            TopDown = 0x100000,
            WriteWatch = 0x200000,
            LargePages = 0x20000000
        }

        [Flags]
        public enum MemoryProtection
        {
            Execute = 0x10,
            ExecuteRead = 0x20,
            ExecuteReadWrite = 0x40,
            ExecuteWriteCopy = 0x80,
            NoAccess = 0x01,
            ReadOnly = 0x02,
            ReadWrite = 0x04,
            WriteCopy = 0x08,
            GuardModifierflag = 0x100,
            NoCacheModifierflag = 0x200,
            WriteCombineModifierflag = 0x400
        }

        [Flags]
        private enum FreeType
        {
            Decommit = 0x4000,
            Release = 0x8000,
        }


        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr VirtualAlloc(IntPtr lpAddress, uint dwSize, AllocationType lAllocationType, MemoryProtection flProtect);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool VirtualFree(IntPtr lpAddress, uint dwSize, FreeType dwFreeType);

        #endregion

        private IntPtr rawMemoryPtr;
        private uint rawSize;

        public IntPtr RawPointer
        {
            get { return this.rawMemoryPtr; }
        }

        public uint Length
        {
            get { return this.rawSize; }
        }

        public WinHvMemoryBlock(uint size, MemoryProtection protection = MemoryProtection.ExecuteReadWrite)
        {
            rawSize = size;
            rawMemoryPtr = VirtualAlloc(IntPtr.Zero, size, AllocationType.Reserve, protection);
        }

        public WinHvMemoryBlock(byte[] raw, int memorysize, MemoryProtection protection = MemoryProtection.ExecuteReadWrite)
        {
            if (raw.LongLength > int.MaxValue)
            {
                throw new NotSupportedException("memory block is too large.");
            }

            if (raw.LongLength > memorysize)
            {
                throw new NotSupportedException("Need to make memory size larger than raw size");
            }

            rawSize = (uint)memorysize;
            rawMemoryPtr = VirtualAlloc(IntPtr.Zero, rawSize, AllocationType.Commit, protection);
            Marshal.Copy(raw,0,rawMemoryPtr,(int)raw.Length);
        }

        public void Dispose()
        {
            VirtualFree(rawMemoryPtr, rawSize, FreeType.Release);
        }
    }
}
