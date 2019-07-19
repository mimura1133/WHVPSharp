using System;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Runtime.InteropServices;
using System.Text;

namespace WHVPSharp
{
    public partial class WinHvPartition : IDisposable
    {
        #region Win32APIs

        [DllImport("WinHvPlatform.dll", SetLastError = true)]
        private static extern uint WHvCreatePartition(out IntPtr handle);

        [DllImport("WinHvPlatform.dll", SetLastError = true)]
        private static extern uint WHvDeletePartition(IntPtr handle);

        [DllImport("WinHvPlatform.dll", SetLastError = true)]
        private static extern uint WHvSetupPartition(IntPtr handle);

        #endregion

        #region Values

        // Partition Handle.
        private IntPtr _handle;

        #endregion


        #region Partitions

        public WinHvPartition()
        {
            if(WHvCreatePartition(out _handle) != 0)
                throw new NotSupportedException();
        }

        public bool Setup()
        {
            return WHvSetupPartition(this._handle) == 0;
        }

        #endregion

        public void Dispose()
        {
            WHvDeletePartition(this._handle);
        }
    }
}
