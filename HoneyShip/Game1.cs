using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace HoneyShip
{
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        Vector2 backgroundPosition;
        Texture2D backgroundAppearance;

        Vector2 shipPosition;
        Texture2D shipAppearance;

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
            // TODO: use this.Content to load your game content here
        }

        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        protected override void Update(GameTime gameTime)
        {
            KeyboardState ks = Keyboard.GetState();
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
            shipPosition += shipDelta * 2.0f;
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();
            spriteBatch.Draw(backgroundAppearance, backgroundPosition, Color.White);
            spriteBatch.Draw(shipAppearance, shipPosition, Color.Gold);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
