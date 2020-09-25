using System.Collections.Generic;

using OpenFL.Core.DataObjects.ExecutableDataObjects;

using Utility.ADL;
using Utility.FastString;

namespace OpenFL.DefaultInstructions.Instructions
{
    public class PrintLineFLInstruction : FLInstruction
    {

        public PrintLineFLInstruction(List<FLInstructionArgument> arguments) : base(arguments)
        {
        }


        public override void Process()
        {
            string log = "FLSCRIPT:";
            for (int i = 0; i < Arguments.Count; i++)
            {
                if (Arguments[i].Type == FLInstructionArgumentType.Number)
                {
                    log += " " + Arguments[i].GetValue();
                }
                else if (Arguments[i].Type == FLInstructionArgumentType.Name &&
                         Parent.Variables.IsDefined(Arguments[i].GetValue().ToString()))
                {
                    log += " " + Parent.Variables.GetVariable(Arguments[i].GetValue().ToString());
                }
                else
                {
                    log += " " + Arguments[i];
                }
            }

            Logger.Log(LogType.Log, log, 1);
        }

        public override string ToString()
        {
            return Arguments.Unpack(" ");
        }

    }
}