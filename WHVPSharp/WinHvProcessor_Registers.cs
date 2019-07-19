using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace WHVPSharp
{
    public partial class WinHvProcessor : IDisposable
    {
        #region Defines

        private enum WhvRegisterName : uint
        {
            // X64 General purpose registers
            Rax = 0x00000000,
            Rcx = 0x00000001,
            Rdx = 0x00000002,
            Rbx = 0x00000003,
            Rsp = 0x00000004,
            Rbp = 0x00000005,
            Rsi = 0x00000006,
            Rdi = 0x00000007,
            R8 = 0x00000008,
            R9 = 0x00000009,
            R10 = 0x0000000A,
            R11 = 0x0000000B,
            R12 = 0x0000000C,
            R13 = 0x0000000D,
            R14 = 0x0000000E,
            R15 = 0x0000000F,
            Rip = 0x00000010,
            Rflags = 0x00000011,

            // X64 Segment registers
            Es = 0x00000012,
            Cs = 0x00000013,
            Ss = 0x00000014,
            Ds = 0x00000015,
            Fs = 0x00000016,
            Gs = 0x00000017,
            Ldtr = 0x00000018,
            Tr = 0x00000019,

            // X64 Table registers
            Idtr = 0x0000001A,
            Gdtr = 0x0000001B,

            // X64 Control Registers
            Cr0 = 0x0000001C,
            Cr2 = 0x0000001D,
            Cr3 = 0x0000001E,
            Cr4 = 0x0000001F,
            Cr8 = 0x00000020,

            // X64 Debug Registers
            Dr0 = 0x00000021,
            Dr1 = 0x00000022,
            Dr2 = 0x00000023,
            Dr3 = 0x00000024,
            Dr6 = 0x00000025,
            Dr7 = 0x00000026,

            // X64 Extended Control Registers
            XCr0 = 0x00000027,

            // X64 Floating Point and Vector Registers
            Xmm0 = 0x00001000,
            Xmm1 = 0x00001001,
            Xmm2 = 0x00001002,
            Xmm3 = 0x00001003,
            Xmm4 = 0x00001004,
            Xmm5 = 0x00001005,
            Xmm6 = 0x00001006,
            Xmm7 = 0x00001007,
            Xmm8 = 0x00001008,
            Xmm9 = 0x00001009,
            Xmm10 = 0x0000100A,
            Xmm11 = 0x0000100B,
            Xmm12 = 0x0000100C,
            Xmm13 = 0x0000100D,
            Xmm14 = 0x0000100E,
            Xmm15 = 0x0000100F,
            FpMmx0 = 0x00001010,
            FpMmx1 = 0x00001011,
            FpMmx2 = 0x00001012,
            FpMmx3 = 0x00001013,
            FpMmx4 = 0x00001014,
            FpMmx5 = 0x00001015,
            FpMmx6 = 0x00001016,
            FpMmx7 = 0x00001017,
            FpControlStatus = 0x00001018,
            XmmControlStatus = 0x00001019,

            // X64 MSRs
            Tsc = 0x00002000,
            Efer = 0x00002001,
            KernelGsBase = 0x00002002,
            ApicBase = 0x00002003,
            Pat = 0x00002004,
            SysenterCs = 0x00002005,
            SysenterEip = 0x00002006,
            SysenterEsp = 0x00002007,
            Star = 0x00002008,
            Lstar = 0x00002009,
            Cstar = 0x0000200A,
            Sfmask = 0x0000200B,

            MsrMtrrCap = 0x0000200D,
            MsrMtrrDefType = 0x0000200E,

            MsrMtrrPhysBase0 = 0x00002010,
            MsrMtrrPhysBase1 = 0x00002011,
            MsrMtrrPhysBase2 = 0x00002012,
            MsrMtrrPhysBase3 = 0x00002013,
            MsrMtrrPhysBase4 = 0x00002014,
            MsrMtrrPhysBase5 = 0x00002015,
            MsrMtrrPhysBase6 = 0x00002016,
            MsrMtrrPhysBase7 = 0x00002017,
            MsrMtrrPhysBase8 = 0x00002018,
            MsrMtrrPhysBase9 = 0x00002019,
            MsrMtrrPhysBaseA = 0x0000201A,
            MsrMtrrPhysBaseB = 0x0000201B,
            MsrMtrrPhysBaseC = 0x0000201C,
            MsrMtrrPhysBaseD = 0x0000201D,
            MsrMtrrPhysBaseE = 0x0000201E,
            MsrMtrrPhysBaseF = 0x0000201F,

            MsrMtrrPhysMask0 = 0x00002040,
            MsrMtrrPhysMask1 = 0x00002041,
            MsrMtrrPhysMask2 = 0x00002042,
            MsrMtrrPhysMask3 = 0x00002043,
            MsrMtrrPhysMask4 = 0x00002044,
            MsrMtrrPhysMask5 = 0x00002045,
            MsrMtrrPhysMask6 = 0x00002046,
            MsrMtrrPhysMask7 = 0x00002047,
            MsrMtrrPhysMask8 = 0x00002048,
            MsrMtrrPhysMask9 = 0x00002049,
            MsrMtrrPhysMaskA = 0x0000204A,
            MsrMtrrPhysMaskB = 0x0000204B,
            MsrMtrrPhysMaskC = 0x0000204C,
            MsrMtrrPhysMaskD = 0x0000204D,
            MsrMtrrPhysMaskE = 0x0000204E,
            MsrMtrrPhysMaskF = 0x0000204F,

            MsrMtrrFix64k00000 = 0x00002070,
            MsrMtrrFix16k80000 = 0x00002071,
            MsrMtrrFix16kA0000 = 0x00002072,
            MsrMtrrFix4kC0000 = 0x00002073,
            MsrMtrrFix4kC8000 = 0x00002074,
            MsrMtrrFix4kD0000 = 0x00002075,
            MsrMtrrFix4kD8000 = 0x00002076,
            MsrMtrrFix4kE0000 = 0x00002077,
            MsrMtrrFix4kE8000 = 0x00002078,
            MsrMtrrFix4kF0000 = 0x00002079,
            MsrMtrrFix4kF8000 = 0x0000207A,

            TscAux = 0x0000207B,
            SpecCtrl = 0x00002084,
            PredCmd = 0x00002085,
            TscVirtualOffset = 0x00002087,

            // APIC state (also accessible via WHv(Get/Set)VirtualProcessorInterruptControllerState)
            ApicId = 0x00003002,
            ApicVersion = 0x00003003,

            // Interrupt / Event Registers
            WHvRegisterPendingInterruption = 0x80000000,
            WHvRegisterInterruptState = 0x80000001,
            WHvRegisterPendingEvent = 0x80000002,
            DeliverabilityNotifications = 0x80000004,
            WHvRegisterInternalActivityState = 0x80000005,

        }
        #endregion

        #region Win32APIs

        [DllImport("WinHvPlatform.dll", SetLastError = true)]
        private static extern uint WHvGetVirtualProcessorRegisters(IntPtr handle, uint index, WhvRegisterName[] name, uint count, out ulong registerValue);

        [DllImport("WinHvPlatform.dll", SetLastError = true)]
        private static extern uint WHvSetVirtualProcessorRegisters(IntPtr handle, uint index, WhvRegisterName[] name, uint count, ref ulong registerValue);

        #endregion

        private ulong getRegister(WhvRegisterName name)
        {
            ulong ret;

            WHvGetVirtualProcessorRegisters(this._partitionHandle, this._index, new WhvRegisterName[]{ name }, 1, out ret);
            return ret;
        }

        private void setRegister(WhvRegisterName name, ulong value)
        {
            WHvSetVirtualProcessorRegisters(this._partitionHandle, this._index, new WhvRegisterName[] { name }, 1, ref value);
        }

        #region Registers

        public ulong RAX
        {
            get { return getRegister(WhvRegisterName.Rax); }
            set { setRegister(WhvRegisterName.Rax,value);}
        }

        public ulong RCX
        {
            get { return getRegister(WhvRegisterName.Rcx); }
            set { setRegister(WhvRegisterName.Rcx, value); }
        }

        public ulong RDX
        {
            get { return getRegister(WhvRegisterName.Rdx); }
            set { setRegister(WhvRegisterName.Rdx, value); }
        }

        public ulong RBX
        {
            get { return getRegister(WhvRegisterName.Rbx); }
            set { setRegister(WhvRegisterName.Rbx, value); }
        }

        public ulong RSP
        {
            get { return getRegister(WhvRegisterName.Rsp); }
            set { setRegister(WhvRegisterName.Rsp, value); }
        }

        public ulong RBP
        {
            get { return getRegister(WhvRegisterName.Rbp); }
            set { setRegister(WhvRegisterName.Rbp, value); }
        }

        public ulong RSI
        {
            get { return getRegister(WhvRegisterName.Rsi); }
            set { setRegister(WhvRegisterName.Rsi, value); }
        }

        public ulong RDI
        {
            get { return getRegister(WhvRegisterName.Rdi); }
            set { setRegister(WhvRegisterName.Rdi, value); }
        }

        public ulong R8
        {
            get { return getRegister(WhvRegisterName.R8); }
            set { setRegister(WhvRegisterName.R8, value); }
        }
        public ulong R9
        {
            get { return getRegister(WhvRegisterName.R9); }
            set { setRegister(WhvRegisterName.R9, value); }
        }

        public ulong R10
        {
            get { return getRegister(WhvRegisterName.R10); }
            set { setRegister(WhvRegisterName.R10, value); }
        }

        public ulong R11
        {
            get { return getRegister(WhvRegisterName.R11); }
            set { setRegister(WhvRegisterName.R11, value); }
        }

        public ulong R12
        {
            get { return getRegister(WhvRegisterName.R12); }
            set { setRegister(WhvRegisterName.R12, value); }
        }
        public ulong R13
        {
            get { return getRegister(WhvRegisterName.R13); }
            set { setRegister(WhvRegisterName.R13, value); }
        }

        public ulong R14
        {
            get { return getRegister(WhvRegisterName.R14); }
            set { setRegister(WhvRegisterName.R14, value); }
        }

        public ulong R15
        {
            get { return getRegister(WhvRegisterName.R15); }
            set { setRegister(WhvRegisterName.R15, value); }
        }

        public ulong RIP
        {
            get { return getRegister(WhvRegisterName.Rip); }
            set { setRegister(WhvRegisterName.Rip, value); }
        }
        public ulong Rflags
        {
            get { return getRegister(WhvRegisterName.Rflags); }
            set { setRegister(WhvRegisterName.Rflags, value); }
        }

        public ulong DR0
        {
            get { return getRegister(WhvRegisterName.Dr0); }
            set { setRegister(WhvRegisterName.Dr0, value); }
        }

        public ulong DR1
        {
            get { return getRegister(WhvRegisterName.Dr1); }
            set { setRegister(WhvRegisterName.Dr1, value); }
        }

        public ulong DR2
        {
            get { return getRegister(WhvRegisterName.Dr2); }
            set { setRegister(WhvRegisterName.Dr2, value); }
        }
        public ulong DR3
        {
            get { return getRegister(WhvRegisterName.Dr3); }
            set { setRegister(WhvRegisterName.Dr3, value); }
        }

        public ulong DR6
        {
            get { return getRegister(WhvRegisterName.Dr6); }
            set { setRegister(WhvRegisterName.Dr6, value); }
        }

        public ulong DR7
        {
            get { return getRegister(WhvRegisterName.Dr7); }
            set { setRegister(WhvRegisterName.Dr7, value); }
        }

        public ulong CR0
        {
            get { return getRegister(WhvRegisterName.Cr0); }
            set { setRegister(WhvRegisterName.Cr0, value); }
        }

        public ulong CR2
        {
            get { return getRegister(WhvRegisterName.Cr2); }
            set { setRegister(WhvRegisterName.Cr2, value); }
        }

        public ulong CR3
        {
            get { return getRegister(WhvRegisterName.Cr3); }
            set { setRegister(WhvRegisterName.Cr3, value); }
        }
        public ulong CR4
        {
            get { return getRegister(WhvRegisterName.Cr4); }
            set { setRegister(WhvRegisterName.Cr4, value); }
        }

        public ulong CR8
        {
            get { return getRegister(WhvRegisterName.Cr8); }
            set { setRegister(WhvRegisterName.Cr8, value); }
        }

        public ulong XCR0
        {
            get { return getRegister(WhvRegisterName.XCr0); }
            set { setRegister(WhvRegisterName.XCr0, value); }
        }

        #endregion

    }
}
