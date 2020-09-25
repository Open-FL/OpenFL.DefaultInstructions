using System.Collections.Generic;
using System.Linq;

using OpenCL.Wrapper;

using OpenFL.Core.DataObjects.ExecutableDataObjects;

namespace OpenFL.DefaultInstructions.Instructions
{
    public class CPUArrangeFLInstruction : ArrangeFLInstruction
    {

        public CPUArrangeFLInstruction(List<FLInstructionArgument> arguments) : base(arguments)
        {
        }

        protected override void Arrange(byte[] newOrder)
        {
            byte[] bytes =
                CLAPI.ReadBuffer<byte>(Root.Instance, Root.ActiveBuffer.Buffer, (int) Root.ActiveBuffer.Size);
            for (int i = 0; i < bytes.Length; i++)
            {
                if (i % Root.ActiveChannels.Length == 0)
                {
                    byte[] channelValues = new byte[Root.ActiveChannels.Length];
                    for (int j = 0; j < channelValues.Length; j++)
                    {
                        channelValues[j] = bytes[i + j];
                    }

                    channelValues = SwapChannel(channelValues, newOrder);

                    for (int j = 0; j < channelValues.Length; j++)
                    {
                        bytes[i + j] = channelValues[j];
                    }
                }
            }

            byte[] test = bytes.Reverse().Take(50).Reverse().ToArray();

            CLAPI.WriteToBuffer(Root.Instance, Root.ActiveBuffer.Buffer, bytes);
        }

        private byte[] SwapChannel(byte[] channelValues, byte[] newOrder)
        {
            byte[] ret = new byte[channelValues.Length];
            for (int i = 0; i < channelValues.Length; i++)
            {
                if (i < newOrder.Length)
                {
                    ret[i] = channelValues[newOrder[i]];
                }
                else
                {
                    ret[i] = channelValues[i];
                }
            }

            return ret;
        }

    }
}