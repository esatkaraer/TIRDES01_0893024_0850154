using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HoneyShip.Scripts
{
    class CreateEntity : Instruction
    {
        public override InstructionResult Execute(float dt)
        {
            return InstructionResult.DoneAndCreateAsteroid;
        }

        public override Instruction Reset()
        {
            return new CreateEntity();
        }
    }
}