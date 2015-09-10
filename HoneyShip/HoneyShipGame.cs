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
        SpriteFont gameFont;
        public HoneyShipGame()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        Vector2 backgroundPosition;
        Texture2D backgroundAppearance;

        Vector2 shipPosition;
        Texture2D shipAppearance;

        Texture2D plasmaAppearance;

        private float rotationAngle;

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            backgroundPosition = new Vector2(-150.0f,-150.0f);
            backgroundAppearance = Content.Load<Texture2D>("background.jpg");

            shipPosition = new Vector2(300.0f, 200.0f);
            shipAppearance = Content.Load<Texture2D>("ship.png");

            plasmaAppearance = Content.Load<Texture2D>("plasmaSmall.png");

            gameFont = Content.Load<SpriteFont>("spaceFont");

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

            if(ms.LeftButton == ButtonState.Pressed)
            {
                //if left button is clicked : shooooooot :D

            }

            shipPosition += shipDelta * 2.0f;

            Vector2 mouseLoc = new Vector2(ms.Position.X, ms.Position.Y);
            rotationAngle = MathHelper.ToDegrees((float)Math.Atan2(mouseLoc.Y - shipPosition.Y, mouseLoc.X - shipPosition.X));

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
            spriteBatch.Draw(shipAppearance, shipPosition, null, Color.White, MathHelper.ToRadians(rotationAngle) + MathHelper.PiOver2, origin, 1.0f, SpriteEffects.None, 0f);
            spriteBatch.DrawString(gameFont, "Degrees : " + rotationAngle , new Vector2(10, 10), Color.White);

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
