using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HoneyShip
{
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
        class DualBlaster : Blaster
        {
            public DualBlaster(ContentManager content) : base(content) { }

            protected override void addShots()
            {
                Entity bullet = new Entity(bulletAppearance);
                bullet.position = shipPosition + Vector2.UnitX * 10.0f;
                bullet.direction = new Vector2((float)Math.Cos((rotationAngle)), (float)Math.Sin((rotationAngle))) * 4f;
                bullet.isVisible = true;
                barrel.Add(bullet);

                Entity bullet2 = new Entity(bulletAppearance);
                bullet2.position = shipPosition - Vector2.UnitX * 10.0f; ;
                bullet2.direction = new Vector2((float)Math.Cos((rotationAngle)), (float)Math.Sin((rotationAngle))) * 4f;
                bullet2.isVisible = true;
                barrel.Add(bullet2);
            }
        }
    }

}
