using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HoneyShip.Scripts
{
    abstract class Instruction
    {
        public abstract InstructionResult Execute(float dt);
        public abstract Instruction Reset();

        static public Instruction operator +(Instruction A, Instruction B)
        {
            return new Semicolon(A, B);
        }
    }
}