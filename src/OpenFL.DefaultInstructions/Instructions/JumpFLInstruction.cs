using System;
using System.Collections.Generic;

using OpenFL.Core.DataObjects.ExecutableDataObjects;

using Utility.ADL;
using Utility.FastString;

namespace OpenFL.DefaultInstructions.Instructions
{
    public class JumpFLInstruction : FLInstruction
    {

        private readonly int ArgumentOffset;

        public JumpFLInstruction(List<FLInstructionArgument> arguments) : base(arguments)
        {
        }

        public JumpFLInstruction(int argOffset, List<FLInstructionArgument> arguments) : this(arguments)
        {
            ArgumentOffset = argOffset;
        }


        public override void Process()
        {
            if (Arguments.Count != ArgumentOffset + 1 ||
                Arguments[ArgumentOffset].Type != FLInstructionArgumentType.Function)
            {
                throw new InvalidOperationException("Jump instruction needs to point to a valid function.");
            }

            IFunction obj = (IFunction) Arguments[ArgumentOffset].GetValue();

            Logger.Log(LogType.Log, "Jumping to " + obj.Name, MIN_INSTRUCTION_SEVERITY);

            obj.Process();
        }

        public override string ToString()
        {
            return "jmp " + Arguments.Unpack(" ");
        }

    }
}