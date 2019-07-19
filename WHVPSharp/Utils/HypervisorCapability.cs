using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace WHVPSharp.Utils
{

    public class HypervisorCapability
    {
        #region Consts

        [Flags]
        private enum WhvCapabilityCode
        {
            // Capabilities of the API implementation
            HypervisorPresent = 0x00000000,
            Features = 0x00000001,
            ExtendedVmExits = 0x00000002,
            ExceptionExitBitmap = 0x00000003,
            X64MsrExitBitmap = 0x00000004,

            // Capabilities of the system's processor
            ProcessorVendor = 0x00001000,
            ProcessorFeatures = 0x00001001,
            ProcessorClFlushSize = 0x00001002,
            ProcessorXsaveFeatures = 0x00001003,
            ProcessorClockFrequency = 0x00001004,
            InterruptClockFrequency = 0x00001005,
        };

        [Flags]
        public enum WhvCapabilityFeatures : ulong
        {
            PartialUnmap = 1,
            LocalApicEmulation = 1 << 1,
            Xsave = 1 << 2,
            DirtyPageTracking = 1 << 3,
            SpeculationControl = 1 << 4,
        };

        [Flags]
        public enum WhvExtendedVmExits : ulong
        {
            X64CpuidExit = 1, // WHvRunVpExitReasonX64CPUID supported
            X64MsrExit = 1 << 1, // WHvRunVpExitX64ReasonMSRAccess supported
            ExceptionExit = 1 << 2, // WHvRunVpExitReasonException supported
            X64RdtscExit = 1 << 3, // WHvRunVpExitReasonX64Rdtsc supported
        };


        //
        // Return values for WhvCapabilityCodeProcessorVendor
        //
        [Flags]
        public enum WhvProcessorVendor
        {
            Amd = 0x0000,
            Intel = 0x0001,
            Hygon = 0x0002
        };

        //
        // Return values for WhvCapabilityCodeProcessorFeatures and input buffer for
        // WHvPartitionPropertyCodeProcessorFeatures
        //
        [Flags]
        public enum WhvProcessorFeatures : ulong
        {
            Sse3Support = (ulong)1,
            LahfSahfSupport = (ulong)1 << 1,
            Ssse3Support = (ulong)1 << 2,
            Sse4_1Support = (ulong)1 << 3,
            Sse4_2Support = (ulong)1 << 4,
            Sse4aSupport = (ulong)1 << 5,
            XopSupport = (ulong)1 << 6,
            PopCntSupport = (ulong)1 << 7,
            Cmpxchg16bSupport = (ulong)1 << 8,
            Altmovcr8Support = (ulong)1 << 9,
            LzcntSupport = (ulong)1 << 10,
            MisAlignSseSupport = (ulong)1 << 11,
            MmxExtSupport = (ulong)1 << 12,
            Amd3DNowSupport = (ulong)1 << 13,
            ExtendedAmd3DNowSupport = (ulong)1 << 14,
            Page1GbSupport = (ulong)1 << 15,
            AesSupport = (ulong)1 << 16,
            PclmulqdqSupport = (ulong)1 << 17,
            PcidSupport = (ulong)1 << 18,
            Fma4Support = (ulong)1 << 19,
            F16CSupport = (ulong)1 << 20,
            RdRandSupport = (ulong)1 << 21,
            RdWrFsGsSupport = (ulong)1 << 22,
            SmepSupport = (ulong)1 << 23,
            EnhancedFastStringSupport = (ulong)1 << 24,
            Bmi1Support = (ulong)1 << 25,
            Bmi2Support = (ulong)1 << 26,
            MovbeSupport = (ulong)1 << 28,
            Npiep1Support = (ulong)1 << 29,
            DepX87FPUSaveSupport = (ulong)1 << 30,
            RdSeedSupport = (ulong)1 << 31,
            AdxSupport = (ulong)1 << 32,
            IntelPrefetchSupport = (ulong)1 << 33,
            SmapSupport = (ulong)1 << 34,
            HleSupport = (ulong)1 << 35,
            RtmSupport = (ulong)1 << 36,
            RdtscpSupport = (ulong)1 << 37,
            ClflushoptSupport = (ulong)1 << 38,
            ClwbSupport = (ulong)1 << 39,
            ShaSupport = (ulong)1 << 40,
            X87PointersSavedSupport = (ulong)1 << 41,
            InvpcidSupport = (ulong)1 << 42,
            IbrsSupport = (ulong)1 << 43,
            StibpSupport = (ulong)1 << 44,
            IbpbSupport = (ulong)1 << 45,
            SsbdSupport = (ulong)1 << 47,
            FastShortRepMovSupport = (ulong)1 << 48,
            RdclNo = (ulong)1 << 50,
            IbrsAllSupport = (ulong)1 << 51,
            SsbNo = (ulong)1 << 53,
            RsbANo = (ulong)1 << 54,
        };

        //
        // Return values for WHvCapabilityCodeProcessorXsaveFeatures and input buffer
        // for WHvPartitionPropertyCodeProcessorXsaveFeatures
        //
        [Flags]
        public enum WhvProcessorXsaveFeatures : ulong
        {
            XsaveSupport = (ulong)1,
            XsaveoptSupport = (ulong)1 << 1,
            AvxSupport = (ulong)1 << 2,
            Avx2Support = (ulong)1 << 3,
            FmaSupport = (ulong)1 << 4,
            MpxSupport = (ulong)1 << 5,
            Avx512Support = (ulong)1 << 6,
            Avx512DQSupport = (ulong)1 << 7,
            Avx512CDSupport = (ulong)1 << 8,
            Avx512BWSupport = (ulong)1 << 9,
            Avx512VLSupport = (ulong)1 << 10,
            XsaveCompSupport = (ulong)1 << 11,
            XsaveSupervisorSupport = (ulong)1 << 12,
            Xcr1Support = (ulong)1 << 13,
            Avx512BitalgSupport = (ulong)1 << 14,
            Avx512IfmaSupport = (ulong)1 << 15,
            Avx512VBmiSupport = (ulong)1 << 16,
            Avx512VBmi2Support = (ulong)1 << 17,
            Avx512VnniSupport = (ulong)1 << 18,
            GfniSupport = (ulong)1 << 19,
            VaesSupport = (ulong)1 << 20,
            Avx512VPopcntdqSupport = (ulong)1 << 21,
            VpclmulqdqSupport = (ulong)1 << 22,
        };

        //UINT64 AsUINT64;
        //} WHV_PROCESSOR_XSAVE_FEATURES, * PWHV_PROCESSOR_XSAVE_FEATURES;

        //
        // Return value for WHvCapabilityCodeX64MsrExits and input buffer for
        // WHvPartitionPropertyCodeX64MsrcExits
        //
        [Flags]
        public enum WhvX64MsrExitBitmap : ulong
        {
            UnhandledMsrs = (ulong)1,
            TscMsrWrite = (ulong)1 << 1,
            TscMsrRead = (ulong)1 << 2,
        };

        #endregion

        private const uint WhvEUnknownCapability = 0x80370300;

        [DllImport("WinHvPlatform.dll", SetLastError = true)]
        private static extern uint WHvGetCapability(WhvCapabilityCode CapabilityCode, out ulong CapabilityBuffer, uint CapabilityBufferSizeInBytes, out uint WrittenSizeInBytes);

        private static ulong CheckCapability(WhvCapabilityCode CapabilityCode)
        {
            ulong ret = 0;
            uint size = 0;

            if (WHvGetCapability(CapabilityCode, out ret, 8, out size) == WhvEUnknownCapability)
                throw new NotSupportedException("Unknown the Capability.");

            return ret;
        }

        public static bool IsHypervisorPresent
        {
            get { return CheckCapability(WhvCapabilityCode.HypervisorPresent) != 0; }
        }

        public static WhvCapabilityFeatures CapabilityFeatures
        {
            get { return (WhvCapabilityFeatures)CheckCapability(WhvCapabilityCode.Features); }
        }

        public static WhvExtendedVmExits ExtendedVmExists
        {
            get { return (WhvExtendedVmExits) CheckCapability(WhvCapabilityCode.ExtendedVmExits); }
        }

        public static WhvProcessorVendor ProcessorVendor
        {
            get { return (WhvProcessorVendor) CheckCapability(WhvCapabilityCode.ProcessorVendor); }
        }

        public static WhvProcessorFeatures ProcessorFeatures
        {
            get { return (WhvProcessorFeatures) CheckCapability(WhvCapabilityCode.Features); }
        }

        public static WhvProcessorXsaveFeatures ProcessorXsaveFeatures
        {
            get { return (WhvProcessorXsaveFeatures) CheckCapability(WhvCapabilityCode.ProcessorXsaveFeatures); }
        }
    }
}
