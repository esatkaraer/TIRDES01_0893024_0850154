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
    using System.Text;

    namespace HoneyShip
    {
        class DualBlaster : Blaster
        {
            public DualBlaster(ContentManager content) : base(content) { }
            int bulletDelayer = 15;

            protected override void addShots()
            {
                if (bulletDelayer == 0)
                {
                    if (barrel.Count < 20)
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

                        /*Entity bullet3 = new Entity(bulletAppearance);
                        bullet3.position = shipPosition;
                        bullet3.direction = new Vector2((float)Math.Cos((rotationAngle)), (float)Math.Sin((rotationAngle))) * 4f;
                        bullet3.isVisible = true;
                        barrel.Add(bullet3);

                        Entity bullet4 = new Entity(bulletAppearance);
                        bullet4.position = shipPosition - Vector2.UnitX * 10.0f; ;
                        bullet4.direction = new Vector2((float)Math.Cos((rotationAngle)), (float)Math.Sin((rotationAngle))) * 4f;
                        bullet4.isVisible = true;
                        barrel.Add(bullet4);

                        Entity bullet5 = new Entity(bulletAppearance);
                        bullet5.position = shipPosition - Vector2.UnitX * 20.0f; ;
                        bullet5.direction = new Vector2((float)Math.Cos((rotationAngle)), (float)Math.Sin((rotationAngle))) * 4f;
                        bullet5.isVisible = true;
                        barrel.Add(bullet5);*/
                        //SoundPlayer gunEffect = new SoundPlayer(@"C:\Users\Esat\Desktop\science_fiction_laser_007.wav");
                        //gunEffect.Play();
                    }
                    bulletDelayer = 15;
                }
                bulletDelayer--;
            }
        }
    }

}
