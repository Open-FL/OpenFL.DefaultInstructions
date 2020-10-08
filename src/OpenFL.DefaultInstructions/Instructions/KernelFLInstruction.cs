using System;
using System.Collections.Generic;
using System.Linq;

using OpenCL.NET.Memory;
using OpenCL.Wrapper;

using OpenFL.Core;
using OpenFL.Core.Buffers;
using OpenFL.Core.DataObjects.ExecutableDataObjects;

using Utility.ADL;
using Utility.FastString;

namespace OpenFL.DefaultInstructions.Instructions
{
    public class KernelFLInstruction : FLInstruction
    {

        /// <summary>
        ///     FL header Count(the offset from 0 where the "user" parameter start)
        /// </summary>
        public const int FL_HEADER_ARG_COUNT = 5;

        private readonly float GenMaxSize;
        private readonly CLKernel Kernel;

        public KernelFLInstruction(float genMaxSize, CLKernel kernel, List<FLInstructionArgument> arguments) :
            base(arguments)
        {
            Kernel = kernel;
            GenMaxSize = genMaxSize;
        }


        public override string ToString()
        {
            return Kernel.Name + " " + Arguments.Unpack(" ");
        }

        private ArgumentResult Compute(FLInstructionArgument arg)
        {
            ArgumentResult ret = new ArgumentResult();
            ret.Type = arg.Type;
            switch (arg.Type)
            {
                case FLInstructionArgumentType.Number:
                    ret.Value = arg.GetValue();
                    break;
                case FLInstructionArgumentType.Name:
                    ret.Value = Parent.Variables.GetVariable(arg.GetValue().ToString());
                    ret.Type = FLInstructionArgumentType.Number; //Translate the Variable to a Number
                    break;
                case FLInstructionArgumentType.Buffer:
                    ret.Value = arg.GetValue();
                    break;
                case FLInstructionArgumentType.Function:
                    ret.Value = ComputeFunction(arg);
                    break;
                default:
                    throw new InvalidOperationException("Can not parse: " + arg.GetValue());
            }

            return ret;
        }

        private FLBuffer ComputeFunction(FLInstructionArgument arg)
        {
            IFunction flFunction = (IFunction) arg.GetValue(); //Process the Function Object

            FLBuffer buffer =
                Root.RegisterUnmanagedBuffer(
                                             new FLBuffer(
                                                          Root.Instance,
                                                          Root.Dimensions.x,
                                                          Root.Dimensions.y,
                                                          Root.Dimensions.z,
                                                          $"{flFunction.Name}_InputBuffer",
                                                          MemoryFlag.ReadWrite,
                                                          flFunction.Modifiers.GetModifiers()
                                                                    .Contains(FLKeywords.OptimizeBufferCreationKeyword)
                                                         )
                                            );

            Logger.Log(LogType.Log, "Storing Current Execution Context", MIN_INSTRUCTION_SEVERITY + 3);
            Root.PushContext(); //Store Dynamic Variables

            Logger.Log(LogType.Log, $"Executing Function: {flFunction.Name}", MIN_INSTRUCTION_SEVERITY + 2);

            Root.ActiveBuffer = buffer;
            flFunction.Process();

            Logger.Log(
                       LogType.Log,
                       $"[{Kernel.Name}]Argument Buffer{Root.ActiveBuffer.DefinedBufferName}",
                       MIN_INSTRUCTION_SEVERITY + 2
                      );

            FLBuffer ret = Root.ActiveBuffer;

            Logger.Log(LogType.Log, "Returning from Function Context", MIN_INSTRUCTION_SEVERITY + 3);
            Root.ReturnFromContext(); //Restore active channels and buffer
            return ret;
        }

        public override void Process()
        {
            Logger.Log(LogType.Log, $"Running CL Kernel: {Kernel.Name}", MIN_INSTRUCTION_SEVERITY);

            ArgumentResult[] results = new ArgumentResult[Arguments.Count];
            Logger.Log(
                       LogType.Log,
                       $"[{Kernel.Name}]Computing Kernel Arguments",
                       MIN_INSTRUCTION_SEVERITY
                      );
            for (int i = 0; i < Arguments.Count; i++)
            {
                results[i] = Compute(Arguments[i]);
            }

            for (int i = 0; i < results.Length; i++)
            {
                Logger.Log(
                           LogType.Log,
                           $"[{Kernel.Name}]Setting Kernel Argument {Kernel.Parameter.First(x => x.Value.Id == i)}",
                           MIN_INSTRUCTION_SEVERITY + 1
                          );
                int kernelArgIndex = i + FL_HEADER_ARG_COUNT;

                ArgumentResult arg = results[i];

                switch (arg.Type)
                {
                    case FLInstructionArgumentType.Number:
                        Kernel.SetArg(kernelArgIndex, arg.Value); //The Value is a Decimal
                        break;
                    case FLInstructionArgumentType.Buffer:
                        FLBuffer bi = (FLBuffer) arg.Value;
                        Logger.Log(
                                   LogType.Log,
                                   $"[{Kernel.Name}]Argument Buffer{bi.DefinedBufferName}",
                                   MIN_INSTRUCTION_SEVERITY + 2
                                  );
                        Kernel.SetBuffer(kernelArgIndex, bi.Buffer);
                        break;
                    case FLInstructionArgumentType.Function:
                        FLBuffer funcBuffer = (FLBuffer) arg.Value;
                        Logger.Log(
                                   LogType.Log,
                                   $"[{Kernel.Name}]Argument Buffer{funcBuffer.DefinedBufferName}",
                                   MIN_INSTRUCTION_SEVERITY + 2
                                  );
                        Kernel.SetBuffer(kernelArgIndex, funcBuffer.Buffer);
                        break;
                    default:
                        throw new InvalidOperationException("Can not parse: " + arg.Value);
                }
            }

            CLAPI.Run(
                      Root.Instance,
                      Kernel,
                      Root.ActiveBuffer.Buffer,
                      Root.Dimensions,
                      GenMaxSize,
                      Root.ActiveChannelBuffer,
                      4
                     );
        }

        private struct ArgumentResult
        {

            public object Value;
            public FLInstructionArgumentType Type;

        }

    }
}