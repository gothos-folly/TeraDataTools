using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using SharpDisasm;
using SharpDisasm.Udis86;

namespace DataCenterUnpack
{
    class KeyScanner
    {
        private IEnumerable<IList<Instruction>> GroupByFunction(IEnumerable<Instruction> instructions)
        {
            var instructionsInFunction = new List<Instruction>();
            foreach (var instruction in instructions)
            {
                if (instruction.Mnemonic == ud_mnemonic_code.UD_Iint3)
                {
                    if (instructionsInFunction.Any())
                    {
                        yield return instructionsInFunction.ToArray();
                    }
                    instructionsInFunction.Clear();
                }
                else
                {
                    instructionsInFunction.Add(instruction);
                }
            }
            yield return instructionsInFunction.ToArray();
        }

        static readonly Regex regex = new Regex(@"^mov dword \[ebp.*],.+$");

        public IEnumerable<Instruction> Disassemble(Disassembler disassembler)
        {
            disassembler.Reset();
            Instruction instruction;
            while ((instruction = disassembler.NextInstruction()) != null)
                yield return instruction;
        }

        private static string Stringify(byte[] bytes)
        {
            var chars = new char[bytes.Length];
            for (int i = 0; i < bytes.Length; i++)
                chars[i] = (char)bytes[i];
            return new string(chars);
        }

        private static byte[] GetBytes(uint i)
        {
            var bytes = new byte[4];
            bytes[0] = (byte)(i >> 0);
            bytes[1] = (byte)(i >> 8);
            bytes[2] = (byte)(i >> 16);
            bytes[3] = (byte)(i >> 24);
            return bytes;
        }

        public static List<Tuple<string, string, int>> Find()
        {
            var candidates = new List<Tuple<string, string, int>>();

            var process = Process.GetProcessesByName("tera").SingleOrDefault();
            if (process == null)
                throw new ApplicationException("Tera doesn't run");
            using (var memoryScanner = new MemoryScanner(process))
            {
                var memoryRegions = memoryScanner.MemoryRegions();
                var relevantRegions = memoryRegions.Where(x => x.State == MemoryScanner.PageState.Commit && x.Protect == MemoryScanner.PageFlags.ExecuteReadWrite);
                foreach (var memoryRegion in relevantRegions)
                {
                    var data = memoryScanner.ReadMemory(memoryRegion.BaseAddress, memoryRegion.RegionSize);
                    //data = data.Skip(0x012F6F46 - 0x00401000).ToArray();
                    var dataSlice = new byte[300];
                    var s = Stringify(data);
                    var index = 0;// 0x016F6EFC - 0x00401000;
                    while ((index = s.IndexOf("\x00CC\x00CC\x00CC\x00CC\x00CC", index, StringComparison.Ordinal)) >= 0)
                    {
                        index++;
                        while (data[index] == 0xCC)
                            index++;
                        Array.Copy(data, index, dataSlice, 0, Math.Min(data.Length - index, dataSlice.Length));
                        var disasm = new Disassembler(dataSlice, ArchitectureMode.x86_32, (ulong)memoryRegion.BaseAddress + (uint)index, true);
                        try
                        {
                            var instructions = disasm.Disassemble().TakeWhile(x => x.Mnemonic != ud_mnemonic_code.UD_Iint3);

                            var movs = new List<Instruction>();
                            foreach (var instruction in instructions)
                            {
                                if (instruction.Mnemonic == ud_mnemonic_code.UD_Imov)
                                    movs.Add(instruction);
                                else
                                {
                                    var matches = movs.Where(x => regex.IsMatch(x.ToString())).ToList();
                                    if (matches.Count == 8)
                                    {
                                        var keyIv = string.Join(" ", matches.Select(x => x.Operands[1].Value).Select(x => BitConverter.ToString(GetBytes((uint)x)).Replace("-", "")));
                                        var interestingChars = keyIv.Count(c => !"0F ".Contains(c));
                                        var key = keyIv.Substring(0, 32 + 3);
                                        var iv = keyIv.Substring(32 + 4, 32 + 3);

                                        candidates.Add(Tuple.Create(key, iv, interestingChars));
                                    }
                                    movs.Clear();
                                }
                            }
                        }
                        catch (IndexOutOfRangeException)
                        {
                        }
                    }
                }
            }
            var candidatesByQuality = candidates.OrderByDescending(t => t.Item3).Where(t => t.Item3 >= 32).ToList();
            return candidatesByQuality;
        }
    }
}
