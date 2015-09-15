using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using System.IO;
using System.Media;
using System.Runtime.InteropServices;
using System.Text;

namespace HoneyShip
{
    public class HoneyShipGame : Game
    {
        [DllImport("winmm.dll")]
        static extern Int32 mciSendString(string command, StringBuilder buffer, int bufferSize, IntPtr hwndCallback);

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        SpriteFont gameFont;

        float deltaTime;
        float oldGameTime;

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

        float friction = 0.1f;

        private float rotationAngle;

        List<Bullet> bullets = new List<Bullet>();
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

            mciSendString(@"open C:\Users\Esat\Desktop\Ascendency.wav type waveaudio alias Ascendency", null, 0, IntPtr.Zero);
            mciSendString(@"play Ascendency", null, 0, IntPtr.Zero);

   
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
            oldGameTime = gameTime.TotalGameTime.Seconds;
            deltaTime = oldGameTime - gameTime.TotalGameTime.Seconds;

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

                    SoundPlayer gunEffect = new SoundPlayer(@"C:\Users\Esat\Desktop\science_fiction_laser_007.wav");
                    gunEffect.Play();
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

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            Vector2 origin = new Vector2();
            origin.X = shipAppearance.Width / 2;
            origin.Y = shipAppearance.Height / 2;

            float mousePositionAngle = MathHelper.ToRadians(rotationAngle) + MathHelper.PiOver2;
            spriteBatch.Begin();
            spriteBatch.Draw(backgroundAppearance, backgroundPosition, Color.White);
            spriteBatch.Draw(shipAppearance, shipPosition, null, Color.White, mousePositionAngle, origin, 1.0f, SpriteEffects.None, 0f);
            spriteBatch.DrawString(gameFont, "Elapsed Time : " + gameTime.TotalGameTime.Seconds , new Vector2(10, 10), Color.White);
            foreach(Bullet b in bullets)
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
