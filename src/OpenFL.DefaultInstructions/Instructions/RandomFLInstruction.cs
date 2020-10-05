﻿using System.Collections.Generic;

using OpenCL.Wrapper;

using OpenFL.Core.Buffers;
using OpenFL.Core.DataObjects.ExecutableDataObjects;
using OpenFL.Core.Exceptions;

using Utility.ADL;
using Utility.FastString;

namespace OpenFL.DefaultInstructions.Instructions
{
    public class RandomFLInstruction : FLInstruction
    {

        public RandomFLInstruction(List<FLInstructionArgument> arguments) : base(arguments)
        {
        }


        public override void Process()
        {
            if (Arguments.Count == 0)
            {
                Logger.Log(
                           LogType.Log,
                           "Writing Random Data to Active Buffer:" + Root.ActiveBuffer.DefinedBufferName,
                           MIN_INSTRUCTION_SEVERITY
                          );
                CLAPI.WriteRandom(
                                  Root.Instance,
                                  Root.ActiveBuffer.Buffer,
                                  RandomInstructionHelper.Randombytesource,
                                  Root.ActiveChannels,
                                  false
                                 );
            }

            for (int i = 0; i < Arguments.Count; i++)
            {
                FLInstructionArgument obj = Arguments[i];

                if (obj.Type != FLInstructionArgumentType.Buffer)
                {
                    throw
                        new FLInvalidArgumentType("Argument: " + obj.GetValue(), "MemoyBuffer/Image");
                }

                FLBuffer func = (FLBuffer) obj.GetValue();

                Logger.Log(
                           LogType.Log,
                           "Writing Random Data to Active Buffer:" + func.DefinedBufferName,
                           MIN_INSTRUCTION_SEVERITY
                          );

                CLAPI.WriteRandom(
                                  Root.Instance,
                                  func.Buffer,
                                  RandomInstructionHelper.Randombytesource,
                                  Root.ActiveChannels,
                                  false
                                 );
            }
        }


        public static FLBuffer ComputeRnd(bool isArray, int size, bool initializeOnStart)
        {
            LazyLoadingFLBuffer info;
            if (!isArray)
            {
                info = new LazyLoadingFLBuffer(
                                               root =>
                                               {
                                                   FLBuffer buf = new FLBuffer(
                                                                               root.Instance,
                                                                               CLAPI.CreateRandom(
                                                                                    root.InputSize,
                                                                                    new byte[] { 1, 1, 1, 1 },
                                                                                    RandomInstructionHelper
                                                                                        .Randombytesource,
                                                                                    false
                                                                                   ),
                                                                               root.Dimensions.x,
                                                                               root.Dimensions.y,
                                                                               root.Dimensions.z,
                                                                               "RandomBuffer"
                                                                              );
                                                   buf.SetRoot(root);
                                                   return buf;
                                               },
                                               initializeOnStart
                                              );
            }
            else
            {
                info = new LazyLoadingFLBuffer(
                                               root =>
                                               {
                                                   FLBuffer buf = new FLBuffer(
                                                                               root.Instance,
                                                                               CLAPI.CreateRandom(
                                                                                    size,
                                                                                    new byte[] { 1, 1, 1, 1 },
                                                                                    RandomInstructionHelper
                                                                                        .Randombytesource,
                                                                                    false
                                                                                   ),
                                                                               size,
                                                                               1,
                                                                               1,
                                                                               "RandomBuffer"
                                                                              );
                                                   buf.SetRoot(root);
                                                   return buf;
                                               },
                                               initializeOnStart
                                              );
            }

            return info;
        }

        public override string ToString()
        {
            return "rnd " + Arguments.Unpack(" ");
        }

    }
}