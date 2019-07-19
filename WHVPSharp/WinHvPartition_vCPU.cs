using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace WHVPSharp
{
    public partial class WinHvPartition : IDisposable
    {

        public WinHvProcessor CreateProcessor(uint index)
        {
            try
            {
                var ret = new WinHvProcessor(this._handle, index);
                return ret;
            }
            catch
            {

            }

            return null;
        }

    }
}
