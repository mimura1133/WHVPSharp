using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace WHVPSharp
{
    public partial class WinHvPartition : IDisposable
    {
        #region Defines

        //
        // Flags used by WHvMapGpaRange
        //
        [Flags]
        public enum WhvMapGpaRangeFlags
        {
            None = 0x00000000,
            Read = 0x00000001,
            Write = 0x00000002,
            Execute = 0x00000004,
            TrackDirtyPages = 0x00000008,
        };

        //
        // Flags used by WHvTranslateGva
        //
        [Flags]
        public enum WhvTranslateGvaFlags : uint
        {
            None = 0x00000000,
            ValidateRead = 0x00000001,
            ValidateWrite = 0x00000002,
            ValidateExecute = 0x00000004,
            PrivilegeExempt = 0x00000008,
            SetPageTableBits = 0x00000010
        };

        //
        // Result of an attempt to translate a guest virtual address
        //
        [Flags]
        public enum WhvTranslateGvaResultCode
        {
            Success = 0,

            // Translation failures
            PageNotPresent = 1,
            PrivilegeViolation = 2,
            InvalidPageTableFlags = 3,

            // GPA access failures
            GpaUnmapped = 4,
            GpaNoReadAccess = 5,
            GpaNoWriteAccess = 6,
            GpaIllegalOverlayAccess = 7,
            Intercept = 8
        };

        //
        // Output buffer of WHvTranslateGva
        //

        [StructLayout(LayoutKind.Sequential)]
        public struct WhvTranslateGvaResult
        {
            public WhvTranslateGvaResultCode ResultCode;
            public uint Reserved;
        };

        #endregion

        #region Win32APIs

        [DllImport("WinHvPlatform.dll", SetLastError = true)]
        private static extern uint WHvUnmapGpaRange(IntPtr handle, ulong guestAddress, ulong size);

        [DllImport("WinHvPlatform.dll", SetLastError = true)]
        private static extern uint WHvMapGpaRange(IntPtr handle, IntPtr sourceAddress, ulong guestAddress, ulong size, WhvMapGpaRangeFlags flags);

        [DllImport("WinHvPlatform.dll", SetLastError = true)]
        private static extern uint WHvQueryGpaRangeDirtyBitmap(IntPtr handle, ulong guestAddress, ulong size, out byte[] bitmap, uint bitmapsize);

        #endregion

        #region values

        // Memory mapping table.
        private Dictionary<WinHvMemoryBlock, ulong> _memorymapping = new Dictionary<WinHvMemoryBlock, ulong>();

        #endregion

        #region GPA functions

        // true is success.
        public bool TryMapGpaRange(WinHvMemoryBlock source, ulong guestAddress, WhvMapGpaRangeFlags flag)
        {
            _memorymapping.Add(source, guestAddress);
            return WHvMapGpaRange(this._handle, source.RawPointer, guestAddress, source.Length, flag) == 0;
        }

        public byte[] QueryGpaRange(ulong guestAddress, int size)
        {
            var ret = new byte[size];
            if (WHvQueryGpaRangeDirtyBitmap(this._handle, guestAddress, (ulong)size, out ret, (uint)size) == 0)
                return ret;
            else
                return null;
        }

        public void UnMapGpaRange(ulong guestAddress, ulong size)
        {
            WHvUnmapGpaRange(this._handle, guestAddress, size);
        }

        public void UnMapGpaRange(WinHvMemoryBlock source)
        {
            if (this._memorymapping.ContainsKey(source))
                if (WHvUnmapGpaRange(this._handle, this._memorymapping[source], source.Length) == 0)
                    this._memorymapping.Remove(source);
        }

        #endregion


    }
}
