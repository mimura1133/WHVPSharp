using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace WHVPSharp
{
    public partial class WinHvProcessor : IDisposable
    {
        #region Defines

        //
        // Reason for a VM exit
        //
        public enum ExitReason : ulong
        {
            None = 0x00000000,

            // Standard exits caused by operations of the virtual processor
            MemoryAccess = 0x00000001,
            X64IoPortAccess = 0x00000002,
            UnrecoverableException = 0x00000004,
            InvalidVpRegisterValue = 0x00000005,
            UnsupportedFeature = 0x00000006,
            X64InterruptWindow = 0x00000007,
            X64Halt = 0x00000008,
            X64ApicEoi = 0x00000009,

            // Additional exits that can be configured through partition properties
            X64MsrAccess = 0x00001000,
            X64Cpuid = 0x00001001,
            Exception = 0x00001002,
            X64Rdtsc = 0x00001003,

            // Exits caused by the host
            Canceled = 0x00002001,
        };

        //
        // Execution state of the virtual processor
        //
        [StructLayout(LayoutKind.Sequential)]
        public struct ExecutionState
        {
            public ushort Cpl;
            public byte Cr0Pe;
            public byte Cr0Am;
            public byte EferLma;
            public byte DebugActive;
            public byte InterruptionPending;
            public byte Reserved0;
            public byte InterruptShadow;
            public byte Reserved1;
        };


        // WHvRunVirtualProcessor output buffer
        [StructLayout(LayoutKind.Sequential)]
        public struct ExitContext
        {
            public ExitReason ExitReason;
            public uint Reserved;
            public VpExitContext VpContext;

            public ulong Details;
        };

        [StructLayout(LayoutKind.Sequential)]
        public struct VpExitContext
        {
            public ExecutionState ExecutionState;
            public byte InstructionLength;
            public byte Cr8;
            public byte Reserved;
            public uint Reserved2;
            public X64SegmentRegister Cs;
            public ulong Rip;
            public ulong Rflags;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct X64SegmentRegister
        {
            public ulong Base;
            public uint Limit;
            public ushort Selector;

            public ushort Attributes;
        };

        #endregion

        #region Win32APIs

        [DllImport("WinHvPlatform.dll", SetLastError = true)]
        private static extern uint WHvRunVirtualProcessor(IntPtr handle, uint index,IntPtr context, out uint size);

        [DllImport("WinHvPlatform.dll", SetLastError = true)]
        private static extern uint WHvCancelRunVirtualProcessor(IntPtr handle, uint index, uint zero);


        #endregion

        public ExitContext Run()
        {
            uint size;

            var memory = Marshal.AllocCoTaskMem(Marshal.SizeOf<ExitContext>());
            WHvRunVirtualProcessor(this._partitionHandle, this._index, memory, out size);
            var ret = Marshal.PtrToStructure<ExitContext>(memory);

            Marshal.FreeCoTaskMem(memory);

            return ret;
        }

        public void Cancel()
        {
            WHvCancelRunVirtualProcessor(this._partitionHandle, this._index, 0);
        }
    }
}
