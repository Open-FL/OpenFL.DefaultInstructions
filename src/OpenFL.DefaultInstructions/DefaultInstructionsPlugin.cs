using OpenFL.Core.Instructions;
using OpenFL.Core.Instructions.InstructionCreators;
using OpenFL.DefaultInstructions.Instructions;

using PluginSystem.Core.Pointer;
using PluginSystem.Utility;

namespace OpenFL.DefaultInstructions
{
    public class DefaultInstructionsPlugin : APlugin<FLInstructionSet>
    {

        public override void OnLoad(PluginAssemblyPointer ptr)
        {
            base.OnLoad(ptr);
            PluginHost.AddInstructionWithDefaultCreator<JumpFLInstruction>(
                                                                     "jmp",
                                                                     "X",
                                                                     "Jumps to a Script or Function and returns. The active channels and buffer will not be cleared"
                                                                    );
            PluginHost.AddInstructionWithDefaultCreator<SetActiveFLInstruction>(
                                                                          "setactive",
                                                                          "E|EV|EVV|EVVV|EVVVV|VVVV|VVV|VV|V",
                                                                          "Sets the active buffer and active channel states."
                                                                         );
            PluginHost.AddInstructionWithDefaultCreator<RandomFLInstruction>(
                                                                       "rnd",
                                                                       "|B",
                                                                       "Writes random values to all active channels of the active buffer"
                                                                      );
            PluginHost.AddInstructionWithDefaultCreator<URandomFLInstruction>(
                                                                        "urnd",
                                                                        "|B",
                                                                        "Writes random values to all active channels of the active buffer, the channels of a pixel will have the same color(grayscale)"
                                                                       );

            PluginHost.AddInstructionWithDefaultCreator<PrintLineFLInstruction>(
                                                                          "print",
                                                                          "A|AA|AAA|AAAA|AAAAA|AAAAAA|AAAAAAA|AAAAAAAA|AAAAAAAAA|AAAAAAAAAA|AAAAAAAAAAA|AAAAAAAAAAAA",
                                                                          "Prints text or all kinds of variables to the console."
                                                                         );
            PluginHost.AddInstructionWithDefaultCreator<CPUArrangeFLInstruction>(
                                                                           "arrange",
                                                                           "V|VV|VVV|VVVV",
                                                                           "Swaps the channels based on the arguments provided"
                                                                          );
            PluginHost.AddInstructionWithDefaultCreator<ArraySetFLInstruction>(
                                                                         "arrset",
                                                                         "CVV",
                                                                         "sets the specified value at the specified index."
                                                                        );

        }

    }
}
