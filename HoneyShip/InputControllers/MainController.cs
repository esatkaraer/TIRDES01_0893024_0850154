using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HoneyShip
{
    class MainController : InputController
    {
        Vector2 shipDelta;
        KeyboardState ks;
        MouseState ms;

        public bool Quit
        {
            get
            {
                return ks.IsKeyDown(Keys.Escape);
            }
        }

        public Vector2 ShipMovement(float rotationAngle)
        {
                shipDelta = Vector2.Zero;
                if (ks.IsKeyDown(Keys.A))
                {
                    shipDelta += new Vector2((float)Math.Cos((rotationAngle) + MathHelper.PiOver2), (float)Math.Sin((rotationAngle + MathHelper.PiOver2)));
                }
                if (ks.IsKeyDown(Keys.D))
                {
                    shipDelta += new Vector2((float)Math.Cos((rotationAngle) - MathHelper.PiOver2), (float)Math.Sin((rotationAngle - MathHelper.PiOver2)));
                }
                if (ks.IsKeyDown(Keys.W))
                {
                    shipDelta += new Vector2((float)Math.Cos((rotationAngle)), (float)Math.Sin((rotationAngle)));
                }
                if (ks.IsKeyDown(Keys.S))
                {
                    shipDelta += new Vector2(-(float)Math.Cos((rotationAngle)), -(float)Math.Sin((rotationAngle)));
                }
                return shipDelta;
        }

        public bool isShooting
        {
            get
            {
                if(ms.LeftButton == ButtonState.Pressed || ks.IsKeyDown(Keys.Space))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            } 
        }

        public void Update(float dt)
        {
           ks  = Keyboard.GetState();
           ms  = Mouse.GetState();
        }
    }
}
