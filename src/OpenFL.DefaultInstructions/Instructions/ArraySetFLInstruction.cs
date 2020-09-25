using System.Collections.Generic;

using OpenFL.Core.Buffers;
using OpenFL.Core.DataObjects.ExecutableDataObjects;

using Utility.FastString;

namespace OpenFL.DefaultInstructions.Instructions
{
    public class ArraySetFLInstruction : FLInstruction
    {

        public ArraySetFLInstruction(List<FLInstructionArgument> arguments) : base(arguments)
        {
        }


        public override void Process()
        {
            IEditableBuffer buffer = (IEditableBuffer) Arguments[0].GetValue();

            int index;
            if (Arguments[1].Type == FLInstructionArgumentType.Name &&
                Parent.Variables.IsDefined(Arguments[1].GetValue().ToString()))
            {
                index = (int) Parent.Variables.GetVariable(Arguments[1].GetValue().ToString());
            }
            else
            {
                index = (int) Arguments[1].GetValue();
            }

            byte value;
            if (Arguments[2].Type == FLInstructionArgumentType.Name &&
                Parent.Variables.IsDefined(Arguments[2].GetValue().ToString()))
            {
                value = (byte) Parent.Variables.GetVariable(Arguments[2].GetValue().ToString());
            }
            else
            {
                object o = Arguments[2].GetValue();
                value = (byte) (decimal) o;
            }

            byte[] bytes = buffer.GetData();
            bytes[index] = value;
            buffer.SetData(bytes);
        }

        public override string ToString()
        {
            return "arrset " + Arguments.Unpack(" ");
        }

    }
}