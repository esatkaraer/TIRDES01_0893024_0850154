using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HoneyShip
{
    interface Weapon<Ammunition>
    {
        List<Ammunition> newBullets();
        void pullTrigger();
        void Update(float dt, Vector2 shipPosition, float rotationAngle, Texture2D bulletAppearance);
    }
}
