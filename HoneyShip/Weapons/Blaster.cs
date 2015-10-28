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
    abstract class Blaster : Weapon<Entity>
    {
        int bulletDelayer = 15;
        protected List<Entity> barrel = new List<Entity>(); 

        protected ContentManager Content;
        public Blaster(ContentManager content)
        {
          Content = content;
        }

        public List<Entity> newBullets()
        {
            return barrel;
        }

        public void pullTrigger()
        {
            if (bulletDelayer == 0)
            {
                if (barrel.Count < 20)
                {
                    addShots();
                }
                SoundPlayer shotSound = new SoundPlayer(@"C:\Users\esatk\Desktop\science_fiction_laser_007.wav");
                shotSound.Play();
                bulletDelayer = 15;
            }
            bulletDelayer--;
        }
        protected abstract void addShots();

        protected Vector2 shipPosition;
        protected float rotationAngle;
        protected Texture2D bulletAppearance;
        public void Update(float dt, Vector2 shipPosition,float rotationAngle,Texture2D bulletAppearance)
        {
            this.shipPosition = shipPosition;
            this.bulletAppearance = bulletAppearance;
            this.rotationAngle = rotationAngle;
            barrel = new List<Entity>();
        }
    }
}
