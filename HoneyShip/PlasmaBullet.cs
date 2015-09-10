using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace HoneyShip
{
    public class PlasmaBullet
    {
        private Texture2D plasmaAppearance { get; set; }
        private Vector2 plasmaPosition { get; set; }

        public PlasmaBullet(Texture2D plasmaAppearance, Vector2 plasmaPosition)
        {
            this.plasmaAppearance = plasmaAppearance;
            this.plasmaPosition = plasmaPosition;
        }
    }
}
