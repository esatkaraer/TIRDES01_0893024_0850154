﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HoneyShip
{
    public class Entity 
    {
        // Entity is alles wat vaker wordt aangemaakt 
        //en is in dit geval een samengevoegde klasse van Bullet, Astroids en powerup
        public Texture2D appearance;

        public Vector2 position;
        public Vector2 direction;

        public Boolean isVisible;

        public Entity(Texture2D texture)
        {
            appearance = texture;
        }

        public void draw(SpriteBatch spriteBatch)
        {
            Vector2 origin = new Vector2();
            origin.X = appearance.Width / 2;
            origin.Y = appearance.Height / 2;
            spriteBatch.Draw(appearance, position, null, Color.White, 0.0f, origin, 1.0f, SpriteEffects.None, 0.0f);
        }
    }
}
