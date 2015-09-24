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
        int aantalXGeraakt = 0;
        Random randomGenerator = new Random();
        int gameLogicScriptPC = 0;
        int rndNumberLine1, iLine1;
        float timeToWaitLine3, timeToWaitLine4, timeToWaitLine8, timeToWaitLine7;
        int rndNumberLine5, iLine5;

        float deltaTime;

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
        Vector2 asteroidBotSpawnerPos;
        Vector2 asteroidLeftSpawnerPos;
        Vector2 asteroidRightSpawnerPos;

        Song bgMusic;

        float friction = 0.1f;

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

            asteroidTopSpawnerPos = new Vector2(Window.ClientBounds.Width / 2, -49);
            asteroidLeftSpawnerPos = new Vector2(-49, Window.ClientBounds.Height / 2);
            asteroidRightSpawnerPos = new Vector2(Window.ClientBounds.Width + 49, Window.ClientBounds.Height / 2);
            asteroidBotSpawnerPos = new Vector2(Window.ClientBounds.Width / 2, Window.ClientBounds.Height + 49);

            // = Content.Load<Song>("ascendency.wma");
            //MediaPlayer.Play(bgMusic);

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
            deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

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

                    SoundPlayer gunEffect = new SoundPlayer(@"C:\Users\RHEA\Desktop\science_fiction_laser_007.wav");
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

        public void spawnAstroids()
        {
            int currentSpawner = randomGenerator.Next(0, 4);
            switch (gameLogicScriptPC)
            {
                case 0:
                    if (true)
                    {
                        gameLogicScriptPC = 1;
                        iLine1 = 1;
                        rndNumberLine1 = randomGenerator.Next(20, 100);
                    }
                    else
                        gameLogicScriptPC = 9;
                    break;
                case 1:
                    if (iLine1 <= rndNumberLine1)
                        gameLogicScriptPC = 2;
                    else
                    {
                        gameLogicScriptPC = 4;
                        timeToWaitLine4 = (float)(randomGenerator.NextDouble() * 3.0 + 6.0);
                    }
                    break;
                case 2:
                    createAstroid(currentSpawner);
                    gameLogicScriptPC = 3;
                    timeToWaitLine3 = (float)(randomGenerator.NextDouble() * 0.3 + 0.1);
                    break;
                case 3:
                    timeToWaitLine3 -= deltaTime;
                    if (timeToWaitLine3 > 0.0f)
                        gameLogicScriptPC = 3;
                    else
                    {
                        gameLogicScriptPC = 1;
                        iLine1++;
                    }
                    break;
                case 4:
                    timeToWaitLine4 -= deltaTime;
                    if (timeToWaitLine4 > 0.0f)
                        gameLogicScriptPC = 4;
                    else
                    {
                        gameLogicScriptPC = 5;
                        iLine5 = 1;
                        rndNumberLine5 = randomGenerator.Next(10, 30);
                    }
                    break;
                case 5:
                    if (iLine5 <= rndNumberLine5)
                    {
                        gameLogicScriptPC = 6;
                    }
                    else
                    {
                        gameLogicScriptPC = 8;
                        timeToWaitLine8 = (float)(randomGenerator.NextDouble() * 2.0 + 5.0);
                    }
                    break;
                case 6:
                    createAstroid(currentSpawner);
                    gameLogicScriptPC = 7;
                    timeToWaitLine7 = (float)(randomGenerator.NextDouble() * 1.5 + 0.5);
                    break;
                case 7:
                    timeToWaitLine7 -= deltaTime;
                    if (timeToWaitLine7 > 0)
                        gameLogicScriptPC = 7;
                    else
                    {
                        gameLogicScriptPC = 5;
                        iLine5++;
                    }
                    break;
                case 8:
                    timeToWaitLine8 -= deltaTime;
                    if (timeToWaitLine8 > 0.0f)
                        gameLogicScriptPC = 8;
                    else
                    {
                        gameLogicScriptPC = 0;
                    }
                    break;
                default:
                    break;
            }
            //maken van asteroide; denk aan richting en snelheid
            // locatie waar de asteroide plaatsvindt
        }

        public void createAstroid(int spawnerID)
        {
            Vector2 spawnerLocation = Vector2.Zero;
            int degree = 0;
            switch(spawnerID)
            {
                case 0:
                    spawnerLocation = asteroidTopSpawnerPos;
                    degree = randomGenerator.Next(0, 181);
                    break;
                case 1:
                    spawnerLocation = asteroidBotSpawnerPos;
                    degree = randomGenerator.Next(180, 360);
                    break;
                case 2:
                    spawnerLocation = asteroidLeftSpawnerPos;
                    degree = randomGenerator.Next(-90, 90);
                    break;
                case 3:
                    spawnerLocation = asteroidRightSpawnerPos;
                    degree = randomGenerator.Next(-180, 180);
                    break;

            }
            
            float asteroidSpeed = (float)randomGenerator.NextDouble();

            Asteroid asteroid = new Asteroid(asteroidAppearance);
            asteroid.position = spawnerLocation;
            asteroid.direction = new Vector2((float)Math.Cos(MathHelper.ToRadians(degree)), (float)Math.Sin(MathHelper.ToRadians(degree))) * asteroidSpeed;
            asteroid.isVisible = true;
            asteroidList.Add(asteroid);
        }

        public void updateAstroid()
        {
            foreach (Asteroid a in asteroidList)
            {
                a.position += a.direction;
                Rectangle offSetRec = new Rectangle(Window.ClientBounds.X -50,Window.ClientBounds.Y - 50,Window.ClientBounds.Width + 100,Window.ClientBounds.Height + 100);
                if (!offSetRec.Contains(a.position))
                {
                    a.isVisible = false;
                }
                foreach(Bullet b in bullets)
                {
                    if(Vector2.Distance(b.position,a.position) < 20.0f)
                    {
                        a.isVisible = false;
                        b.isVisible = false;
                    }
                }
                if (Vector2.Distance(a.position, shipPosition) < 20.0f)
                {
                    aantalXGeraakt++;
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
            spriteBatch.DrawString(gameFont, "Aantal x geraakt : " + aantalXGeraakt, new Vector2(10, 10), Color.White);

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
