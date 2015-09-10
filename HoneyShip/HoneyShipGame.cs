using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace HoneyShip
{
    public class HoneyShipGame : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        public HoneyShipGame()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        Vector2 backgroundPosition;
        Texture2D backgroundAppearance;

        Vector2 shipPosition;
        Texture2D shipAppearance;

        Vector2 plasmaPosition;
        Texture2D plasmaAppearance;

        bool isShooting;
        private float RotationAngle;
        int count = 0;

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            backgroundPosition = new Vector2(-150.0f, -150.0f);
            backgroundAppearance = Content.Load<Texture2D>("background.jpg");

            shipPosition = new Vector2(300.0f, 200.0f);
            shipAppearance = Content.Load<Texture2D>("ship.png");

            plasmaAppearance = Content.Load<Texture2D>("plasmaSmall.png");

            isShooting = false;
            // TODO: use this.Content to load your game content here
        }

        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        protected override void Update(GameTime gameTime)
        {
            KeyboardState ks = Keyboard.GetState();
            MouseState ms = Mouse.GetState();

            if (ks.IsKeyDown(Keys.Escape))
                Exit();

            if(isShooting)
            {
                plasmaPosition += new Vector2(0.0f, -1.0f) * 3.0f;
            }
           if(plasmaPosition.Y < 0)
            {
                isShooting = false;
            }

            var shipDelta = Vector2.Zero;

            if(ks.IsKeyDown(Keys.A))
            {
                shipDelta += new Vector2(-1.0f, 0.0f);
            }
            if (ks.IsKeyDown(Keys.D))
            {
                shipDelta += new Vector2(1.0f, 0.0f);
            } 
            if (ks.IsKeyDown(Keys.W))
            {
                shipDelta += new Vector2(0.0f, -1.0f);
            } 
            if (ks.IsKeyDown(Keys.S))
            {
                shipDelta += new Vector2(0.0f, 1.0f);
            }

            shipPosition += shipDelta * 2.0f;

            Vector2 mouseLoc = new Vector2(ms.Position.X, ms.Position.Y); 

            Vector2 direction = (shipPosition) - mouseLoc;
            RotationAngle = (float)(Math.Atan2(direction.Y, direction.X)); 

            //float circle = MathHelper.Pi * 2;
            //RotationAngle = count % circle;
            //count++;

            if(ks.IsKeyDown(Keys.Space))
            {
                if(!isShooting)
                {
                    isShooting = true;
                    plasmaPosition = new Vector2(shipPosition.X + 25,shipPosition.Y);
                }
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            Vector2 origin = new Vector2();
            origin.X = shipAppearance.Width / 2;
            origin.Y = shipAppearance.Height / 2;

            spriteBatch.Begin();
            spriteBatch.Draw(backgroundAppearance, backgroundPosition, Color.White);
            spriteBatch.Draw(shipAppearance, shipPosition, null, Color.White, RotationAngle, origin, 1.0f, SpriteEffects.None, 0f); 

            if (isShooting)
            {
                spriteBatch.Draw(plasmaAppearance, plasmaPosition, null, Color.White, RotationAngle, origin, 1.0f, SpriteEffects.None, 0f);
            }
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
