using System;
using System.Text;
using WHVPSharp;
using WHVPSharp.Utils;

namespace WHVPSharp_Test
{
    class Program
    {
        static void ShowRegisters(WinHvProcessor processor)
        {
            Console.WriteLine("CURRENT REGISTERS (real mode):");
            Console.WriteLine("AX : {0:x4}, CX : {1:x4}, DX : {2:x4}, BX : {3:x4}", (ushort)processor.RAX, (ushort)processor.RCX, (ushort)processor.RDX, (ushort)processor.RBX);
            Console.WriteLine("SP : {0:x4}, BP : {1:x4}, SI : {2:x4}, DI : {3:x4}", (ushort)processor.RSP, (ushort)processor.RBP, (ushort)processor.RSI, (ushort)processor.RDI);
            Console.WriteLine("IP : {0:x4}, Flags : {1:x4}", (ushort)processor.RIP, processor.Rflags);
        }

        static void Main(string[] args)
        {
            var romBase = 0xF0000;

            Console.WriteLine("FEATURES:");
            Console.WriteLine("ExtendedVmExists : " + HypervisorCapability.ExtendedVmExists);
            Console.WriteLine("ProcessorFeatures : " + HypervisorCapability.ProcessorFeatures);
            Console.WriteLine("ProcessorFeatures : " + HypervisorCapability.ProcessorFeatures);
            Console.WriteLine("ProcessorVendor : " + HypervisorCapability.ProcessorVendor);
            Console.WriteLine("ProcessorXsaveFeatures : " + HypervisorCapability.ProcessorXsaveFeatures);

            Console.WriteLine("-------------------------------------");

            var data = new byte[65536];
            Array.Fill(data, (byte) 0xf4); // FILL the "Halt" commands.


            var code = new byte[]
            {
                0x31, 0xc0,                  // xor eax,eax
                0x66,0xb8,0xfe,0xca,         // mov eax, 0xCAFE
                0xf4                         // halt
            };

            Array.Copy(code, 0, data, 0xFFF0, code.Length);

            var memory = new WinHvMemoryBlock(data, 65536, WinHvMemoryBlock.MemoryProtection.ExecuteReadWrite);

            var partition = new WinHvPartition();
            partition.ProcessorCount = 1;

            partition.Setup();
            partition.TryMapGpaRange(memory, (ulong) romBase,
                WinHvPartition.WhvMapGpaRangeFlags.Execute | WinHvPartition.WhvMapGpaRangeFlags.Read);

            var processor = partition.CreateProcessor(0);

            ShowRegisters(processor);

            processor.RCX = 0xBABE;
            Console.WriteLine("EXECUTE / halt reason : " + processor.Run().ExitReason);
            Console.WriteLine();

            ShowRegisters(processor);
        }
    }
}
