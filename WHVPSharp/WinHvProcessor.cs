using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace WHVPSharp
{
    public partial class WinHvProcessor : IDisposable
    {
        #region Win32APIs

        [DllImport("WinHvPlatform.dll", SetLastError = true)]
        private static extern uint WHvCreateVirtualProcessor(IntPtr handle, uint index, uint reserve);

        [DllImport("WinHvPlatform.dll", SetLastError = true)]
        private static extern uint WHvDeleteVirtualProcessor(IntPtr handle, uint index);

        #endregion

        #region Values

        private IntPtr _partitionHandle;
        private uint _index;

        #endregion

        internal WinHvProcessor(IntPtr handle, uint index)
        {
            this._partitionHandle = handle;
            this._index = index;

            if(WHvCreateVirtualProcessor(this._partitionHandle, this._index, 0) != 0)
                throw new NotSupportedException();


        }

        public void Dispose()
        {
            WHvDeleteVirtualProcessor(this._partitionHandle, this._index);
        }


    }
}
