using System;
using System.Collections.Generic;

using OpenFL.Core.DataObjects.ExecutableDataObjects;

using Utility.FastString;

namespace OpenFL.DefaultInstructions.Instructions
{
    public abstract class ArrangeFLInstruction : FLInstruction
    {

        protected ArrangeFLInstruction(List<FLInstructionArgument> arguments) : base(arguments)
        {
        }

        public override void Process()
        {
            byte[] newOrder = new byte[Root.ActiveChannels.Length];
            for (int i = 0; i < newOrder.Length; i++)
            {
                if (i >= Arguments.Count)
                {
                    newOrder[i] = (byte) i;
                }
                else
                {
                    if (Arguments[i].Type == FLInstructionArgumentType.Number)
                    {
                        byte channel = (byte) Convert.ChangeType(Arguments[i].GetValue(), typeof(byte));
                        newOrder[i] = channel;
                    }
                    else
                    {
                        throw new InvalidOperationException("Invalid Channel ID");
                    }
                }
            }

            Arrange(newOrder);
        }

        protected abstract void Arrange(byte[] newOrder);

        public override string ToString()
        {
            return "arrange " + Arguments.Unpack(" ");
        }

    }
}