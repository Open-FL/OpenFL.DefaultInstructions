using System;
using System.Collections.Generic;

using OpenCL.NET.Memory;

using OpenFL.Core.Buffers;
using OpenFL.Core.DataObjects.ExecutableDataObjects;

using Utility.ADL;
using Utility.FastString;

namespace OpenFL.DefaultInstructions.Instructions
{
    public class SetActiveFLInstruction : FLInstruction
    {

        public SetActiveFLInstruction(List<FLInstructionArgument> arguments) : base(arguments)
        {
        }

        public override void Process()
        {
            byte[] newFlags = new byte[4];
            for (int i = 0; i < Arguments.Count; i++)
            {
                if (i == 0)
                {
                    if (Arguments[i].Type == FLInstructionArgumentType.Buffer)
                    {
                        FLBuffer obj = (FLBuffer) Arguments[i].GetValue();
                        Logger.Log(
                                   LogType.Log,
                                   "Setting Active Buffer: " + obj.DefinedBufferName,
                                   MIN_INSTRUCTION_SEVERITY
                                  );

                        Root.ActiveBuffer = obj;
                        continue;
                    }


                    if (Arguments[i].Type == FLInstructionArgumentType.Function)
                    {
                        FLBuffer buffer =
                            Root.RegisterUnmanagedBuffer(
                                                         new FLBuffer(
                                                                      Root.Instance,
                                                                      Root.Dimensions.x,
                                                                      Root.Dimensions.y,
                                                                      Root.Dimensions.z,
                                                                      "FunctionInputBuffer_Registered"
                                                                     )
                                                        );
                        IFunction source = (IFunction) Arguments[i].GetValue();


                        Logger.Log(LogType.Log, "Storing Current Execution Context", MIN_INSTRUCTION_SEVERITY + 3);
                        Root.PushContext();

                        Logger.Log(LogType.Log, $"Executing Function: {source.Name}", MIN_INSTRUCTION_SEVERITY + 2);

                        Root.ActiveBuffer = buffer;
                        source.Process();

                        //Root.Run(Root.Instance, buffer, true, source);

                        FLBuffer output = Root.ActiveBuffer;

                        Logger.Log(LogType.Log, "Returning from Function Context", MIN_INSTRUCTION_SEVERITY + 3);
                        Root.ReturnFromContext();

                        Root.ActiveBuffer = output;

                        continue;
                    }
                }

                if (Arguments[i].Type == FLInstructionArgumentType.Number)
                {
                    int channel = (int) Convert.ChangeType(Arguments[i].GetValue(), typeof(int));
                    Logger.Log(LogType.Log, "Setting Active Channel: " + channel, MIN_INSTRUCTION_SEVERITY);
                    newFlags[channel] = 1;
                }
                else
                {
                    throw new InvalidOperationException("Invalid Channel ID");
                }
            }

            Root.ActiveChannels = newFlags;
        }

        public override string ToString()
        {
            return "setactive " + Arguments.Unpack(" ");
        }

    }
}