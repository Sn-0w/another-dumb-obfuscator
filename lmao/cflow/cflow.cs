﻿using dnlib.DotNet;
using dnlib.DotNet.Emit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lmao.cflow
{
    internal class cflow
    {
        public static void cflowgo(ModuleDefMD md)
        {
            foreach (var tDef in md.Types)
            {
                if (tDef == md.GlobalType) continue;
                foreach (var mDef in tDef.Methods)
                {
                    if (mDef.Name.StartsWith("get_") || mDef.Name.StartsWith("set_")) continue;
                    if (!mDef.HasBody || mDef.IsConstructor) continue;
                    mDef.Body.SimplifyBranches();
                    sussy(mDef);
                }
            }
        }

        public static void sussy(MethodDef method)
        {
            method.Body.SimplifyMacros(method.Parameters);
            var blocks = BlockParser.ParseMethod(method);
            blocks = Randomize(blocks);
            method.Body.Instructions.Clear();
            var local = new Local(method.Module.CorLibTypes.Int32);
            method.Body.Variables.Add(local);
            var target = Instruction.Create(OpCodes.Nop);
            var instr = Instruction.Create(OpCodes.Br, target);
            foreach (var instruction in Calc(0))
                method.Body.Instructions.Add(instruction);
            method.Body.Instructions.Add(Instruction.Create(OpCodes.Stloc, local));
            method.Body.Instructions.Add(Instruction.Create(OpCodes.Br, instr));
            method.Body.Instructions.Add(target);
            foreach (var block in blocks.Where(block => block != blocks.Single(x => x.Number == blocks.Count - 1)))
            {
                method.Body.Instructions.Add(Instruction.Create(OpCodes.Ldloc, local));
                foreach (var instruction in Calc(block.Number))
                    method.Body.Instructions.Add(instruction);
                method.Body.Instructions.Add(Instruction.Create(OpCodes.Ceq));
                var instruction4 = Instruction.Create(OpCodes.Nop);
                method.Body.Instructions.Add(Instruction.Create(OpCodes.Brfalse, instruction4));

                foreach (var instruction in block.Instructions)
                    method.Body.Instructions.Add(instruction);

                foreach (var instruction in Calc(block.Number + 1))
                    method.Body.Instructions.Add(instruction);

                method.Body.Instructions.Add(Instruction.Create(OpCodes.Stloc, local));
                method.Body.Instructions.Add(instruction4);
            }
            method.Body.Instructions.Add(Instruction.Create(OpCodes.Ldloc, local));
            foreach (var instruction in Calc(blocks.Count - 1))
                method.Body.Instructions.Add(instruction);
            method.Body.Instructions.Add(Instruction.Create(OpCodes.Ceq));
            method.Body.Instructions.Add(Instruction.Create(OpCodes.Brfalse, instr));
            method.Body.Instructions.Add(Instruction.Create(OpCodes.Br, blocks.Single(x => x.Number == blocks.Count - 1).Instructions[0]));
            method.Body.Instructions.Add(instr);

            foreach (var lastBlock in blocks.Single(x => x.Number == blocks.Count - 1).Instructions)
                method.Body.Instructions.Add(lastBlock);
        }

        public static Random rnd = new Random();

        public static List<Block> Randomize(List<Block> input)
        {
            var ret = new List<Block>();
            foreach (var group in input)
                ret.Insert(rnd.Next(0, ret.Count), group);
            return ret;
        }

        public static List<Instruction> Calc(int value)
        {
            var instructions = new List<Instruction> { Instruction.Create(OpCodes.Ldc_I4, value) };
            return instructions;
        }

        public void AddJump(IList<Instruction> instrs, Instruction target)
        {
            instrs.Add(Instruction.Create(OpCodes.Br, target));
        }
    }

    public static class jumpcflw
    {
        public static Random rand = new Random();

        public static void yeet(ModuleDefMD module)
        {
            foreach (var type in module.Types)
            {
                foreach (var method in type.Methods.ToArray())
                {
                    if (!method.HasBody || !method.Body.HasInstructions || method.Body.HasExceptionHandlers) continue;
                    for (var i = 0; i < method.Body.Instructions.Count - 2; i++)
                    {
                        var inst = method.Body.Instructions[i + 1];
                        method.Body.Instructions.Insert(i + 1, Instruction.Create(OpCodes.Ldstr, "FloppaᅠGamingᅠ<OBFUSCATOR>ᅠ" + rand.Next(0, 1000000000)));
                        method.Body.Instructions.Insert(i + 1, Instruction.Create(OpCodes.Br_S, inst));
                        i += 2;
                    }
                }
            }
        }
    }
}
