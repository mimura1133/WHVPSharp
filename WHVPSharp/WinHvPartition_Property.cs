using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace WHVPSharp
{
    public partial class WinHvPartition : IDisposable
    {

        #region Defines

        [Flags]
        private enum WhvPartitionPropertyCode : uint
        {
            ExtendedVmExits = 0x00000001,
            ExceptionExitBitmap = 0x00000002,
            SeparateSecurityDomain = 0x00000003,
            NestedVirtualization = 0x00000004,
            X64MsrExitBitmap = 0x00000005,

            ProcessorFeatures = 0x00001001,
            ProcessorClFlushSize = 0x00001002,
            CpuidExitList = 0x00001003,
            CpuidResultList = 0x00001004,
            LocalApicEmulationMode = 0x00001005,
            ProcessorXsaveFeatures = 0x00001006,
            ProcessorClockFrequency = 0x00001007,
            InterruptClockFrequency = 0x00001008,

            ProcessorCount = 0x00001fff
        };

        #endregion

        #region Win32APIs

        [DllImport("WinHvPlatform.dll", SetLastError = true)]
        private static extern uint WHvGetPartitionProperty(IntPtr handle, WhvPartitionPropertyCode code, IntPtr buf, uint size, out uint writtensize);

        [DllImport("WinHvPlatform.dll", SetLastError = true)]
        private static extern uint WHvSetPartitionProperty(IntPtr handle, WhvPartitionPropertyCode code, IntPtr buf, uint size);


        #endregion

        public int ProcessorCount
        {
            get
            {
                var p = Marshal.AllocHGlobal(4);
                uint size;
                WHvGetPartitionProperty(this._handle, WhvPartitionPropertyCode.ProcessorCount, p, 4, out size);

                var ret = Marshal.ReadInt32(p);
                Marshal.FreeHGlobal(p);
                return ret;
            }
            set
            {
                var p = Marshal.AllocHGlobal(4);
                Marshal.WriteInt32(p,(int)value);

                WHvSetPartitionProperty(this._handle, WhvPartitionPropertyCode.ProcessorCount, p, 4);
                Marshal.FreeHGlobal(p);
            }
        }

        public long ProcessorClockFrequency
        {
            get
            {
                var p = Marshal.AllocHGlobal(8);
                uint size;
                WHvGetPartitionProperty(this._handle, WhvPartitionPropertyCode.ProcessorClockFrequency, p, 8, out size);

                var ret = Marshal.ReadInt64(p);
                Marshal.FreeHGlobal(p);
                return ret;
            }
            set
            {
                var p = Marshal.AllocHGlobal(8);
                Marshal.WriteInt32(p, (int)value);

                WHvSetPartitionProperty(this._handle, WhvPartitionPropertyCode.ProcessorClockFrequency, p, 8);
                Marshal.FreeHGlobal(p);
            }
        }

        public bool NestedVirtualization
        {
            get
            {
                var p = Marshal.AllocHGlobal(4);
                uint size;
                WHvGetPartitionProperty(this._handle, WhvPartitionPropertyCode.NestedVirtualization, p, 4, out size);

                var ret = Marshal.ReadInt32(p);
                Marshal.FreeHGlobal(p);
                return ret == 1;
            }
            set
            {
                var p = Marshal.AllocHGlobal(4);
                Marshal.WriteInt32(p, value ? 1 : 0);

                WHvSetPartitionProperty(this._handle, WhvPartitionPropertyCode.NestedVirtualization, p, 4);
                Marshal.FreeHGlobal(p);
            }
        }
    }
}
