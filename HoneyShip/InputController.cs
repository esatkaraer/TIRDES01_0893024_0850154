using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HoneyShip
{
    interface InputController
    {
        bool Quit { get; }
        Vector2 ShipMovement(float rotationAngle);
        bool isShooting { get; }

        void Update(float dt);
    }
}
