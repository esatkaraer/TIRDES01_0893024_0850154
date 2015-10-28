using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
using System.Text;

namespace HoneyShip
{
    class SoloBlaster : Blaster
    {
        public SoloBlaster(ContentManager content) : base(content) { }

        protected override void addShots()
        {
            Entity bullet = new Entity(bulletAppearance);
            bullet.position = shipPosition;
            bullet.direction = new Vector2((float)Math.Cos((rotationAngle)), (float)Math.Sin((rotationAngle))) * 4f;
            bullet.isVisible = true;
            barrel.Add(bullet);
        }
    }
}
