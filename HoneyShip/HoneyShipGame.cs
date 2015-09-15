using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;

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
        Vector2 shipDelta;
        Texture2D bulletAppearance;
        Texture2D asteroidAppearance;
        Vector2 asteroidTopSpawnerPos;

        Song bgMusic;

        private float rotationAngle;

        List<Bullet> bullets = new List<Bullet>();
        //list aanmaken voor alle astroids
        List<Asteroid> asteroidList = new List<Asteroid>();

        int bulletDelayer = 15;
        protected override void Initialize()
        {
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

            bulletAppearance = Content.Load<Texture2D>("plasma.png");

            asteroidAppearance = Content.Load<Texture2D>("asteroid.png");
            asteroidTopSpawnerPos = new Vector2(Window.ClientBounds.Width / 2, 10);

            // = Content.Load<Song>("ascendency.wma");
            //MediaPlayer.Play(bgMusic);

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


            shipDelta = Vector2.Zero;
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
            if (!Window.ClientBounds.Contains(shipPosition))
            {
                Exit();
            }
            Vector2 mouseLoc = new Vector2(ms.Position.X, ms.Position.Y);
            rotationAngle = MathHelper.ToDegrees((float)Math.Atan2(mouseLoc.Y - shipPosition.Y, mouseLoc.X - shipPosition.X));

            if(ms.LeftButton == ButtonState.Pressed)
            {
                shoot();
            }

            spawnAstroids();

            base.Update(gameTime);
        }

        public void shoot()
        {
            if (bulletDelayer == 0)
            {
                if (bullets.Count < 20)
                {
                    Bullet bullet = new Bullet(bulletAppearance);
                    bullet.position = shipPosition;
                    bullet.direction = new Vector2((float)Math.Cos(MathHelper.ToRadians(rotationAngle)), (float)Math.Sin(MathHelper.ToRadians(rotationAngle))) * 4f;
                    bullet.isVisible = true;
                    bullets.Add(bullet);
                }
                bulletDelayer = 15;
            }
            bulletDelayer--;
        }

        public void updateBullets()
        {
            foreach (Bullet b in bullets)
            {
                b.position += b.direction;
                if(!Window.ClientBounds.Contains(b.position))
                {
                    b.isVisible = false;
                }
            }
        }

        public void spawnAstroids()
        {
            //maken van asteroide; denk aan richting en snelheid
            // locatie waar de asteroide plaatsvindt
            Random random = new Random();
            int degree = random.Next(0, 181);
            float asteroidSpeed = (float)random.NextDouble();

            Asteroid asteroid = new Asteroid(asteroidAppearance);
            asteroid.position = asteroidTopSpawnerPos;
            asteroid.direction = new Vector2((float)Math.Cos(MathHelper.ToRadians(degree)), (float)Math.Sin(MathHelper.ToRadians(degree))) * asteroidSpeed;
            asteroid.isVisible = true;
            asteroidList.Add(asteroid);
        }
        public void updateAstroid()
        {
            foreach (Asteroid a in asteroidList)
            {
                a.position += a.direction;
                if(!Window.ClientBounds.Contains(a.position))
                {
                    a.isVisible = false;
                }
            }
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            Vector2 origin = new Vector2();
            origin.X = shipAppearance.Width / 2;
            origin.Y = shipAppearance.Height / 2;

            Vector2 astroidOrigin = new Vector2();
            astroidOrigin.X = asteroidAppearance.Width / 2;
            astroidOrigin.Y = asteroidAppearance.Height / 2;


            float mousePositionAngle = MathHelper.ToRadians(rotationAngle) + MathHelper.PiOver2;
            spriteBatch.Begin();
            spriteBatch.Draw(backgroundAppearance, backgroundPosition, Color.White);
            spriteBatch.Draw(shipAppearance, shipPosition, null, Color.White, mousePositionAngle, origin, 1.0f, SpriteEffects.None, 0f);
            spriteBatch.DrawString(gameFont, "Degrees : " + rotationAngle , new Vector2(10, 10), Color.White);

            foreach(Asteroid a in asteroidList)
            {
                a.draw(spriteBatch);
            }

            updateAstroid();

            for (int i = 0; i < asteroidList.Count; i++)
            {
                if(!asteroidList[i].isVisible)
                {
                    asteroidList.RemoveAt(i);
                }
            }

            foreach (Bullet b in bullets)
            {
                b.draw(spriteBatch);
            }
            updateBullets();
            for (int i = 0; i < bullets.Count; i++)
            {
                if (!bullets[i].isVisible)
                {
                    bullets.RemoveAt(i);
                }
            }
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
