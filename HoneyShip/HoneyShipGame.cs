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
        int hitCount = 0;
        Random randomGenerator = new Random();

        //Asteroid spawn script variables
        int gameLogicScriptPC = 0;
        int rndNumberLine1, iLine1;
        float timeToWaitLine3, timeToWaitLine4, timeToWaitLine8, timeToWaitLine7;
        int rndNumberLine5, iLine5;

        //Powerup spawn script variables
        int powerUPScriptPC = 0;
        int iLine1PowerUP,rndNumberLine1PowerUP;
        float timeToWaitLine3PowerUP, timeToWaitLine4PowerUP;

        float deltaTime;
        float rotationAngle;
        float mousePositionAngle;

        InputController input =
           new MainController();

        public HoneyShipGame()
        {
            graphics = new GraphicsDeviceManager(this);
            //graphics.IsFullScreen = true;
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            
        }

        Vector2 backgroundPosition;
        Texture2D backgroundAppearance;
        Vector2 shipPosition;
        Texture2D shipAppearance;
        Texture2D bulletAppearance;
        Texture2D asteroidAppearance;
        Vector2 asteroidTopSpawnerPos;
        Vector2 asteroidBotSpawnerPos;
        Vector2 asteroidLeftSpawnerPos;
        Vector2 asteroidRightSpawnerPos;

        Texture2D powerUpAppearance;

        Song bgMusic;

        float friction = 0.1f;
        int score = 0;
        int oldScore = 250;
        List<Entity> bullets = new List<Entity>();
        //list aanmaken voor alle astroids
        List<Entity> asteroidList = new List<Entity>();
        List<Entity> powerUPs = new List<Entity>();
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
            shipPosition = new Vector2(300.0f, 200.0f);
            backgroundAppearance = Content.Load<Texture2D>("background.jpg");

            shipAppearance = Content.Load<Texture2D>("ship.png");

            bulletAppearance = Content.Load<Texture2D>("plasma.png");

            asteroidAppearance = Content.Load<Texture2D>("asteroid.png");

            powerUpAppearance = Content.Load<Texture2D>("powerup.png");

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
            deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            input.Update(deltaTime);
            Vector2 mouseLoc = new Vector2(Mouse.GetState().Position.X, Mouse.GetState().Position.Y);
            rotationAngle = (float)Math.Atan2(mouseLoc.Y - shipPosition.Y, mouseLoc.X - shipPosition.X);
            mousePositionAngle = rotationAngle;

            shipPosition += input.ShipMovement(rotationAngle) * 2.0f;

            if(input.isShooting)
            {
                shoot();
            }

            if (!Window.ClientBounds.Contains(shipPosition))
            {
                Exit();
            }


            spawnAstroids();
            spawnPowerUP();
            base.Update(gameTime);
        }

        public void shoot()
        {
            if (bulletDelayer == 0)
            {
                if (bullets.Count < 20)
                {
                    Entity bullet = new Entity(bulletAppearance);
                    bullet.position = shipPosition;
                    bullet.direction = new Vector2((float)Math.Cos((rotationAngle)), (float)Math.Sin((rotationAngle))) * 4f;
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
            foreach (Entity b in bullets)
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

        public void spawnPowerUP()
        {
            if(score >= (oldScore * 2))
            {
                oldScore = score;
                createPowerUP();
            }
            /*switch (powerUPScriptPC)
            {
                case 0:
                    if (true)
                    {
                        //for(int iLine1PowerUP = 1)
                        powerUPScriptPC = 1;
                        iLine1PowerUP = 1;
                        rndNumberLine1PowerUP = randomGenerator.Next(10,30 );
                    }
                    else
                        powerUPScriptPC = 5;
                    break;
                case 1:
                    //for(int iLinePowerUP = 1; iLine1PowerUP <= rndNumberLine1PowerUP;)
                    if (iLine1PowerUP <= rndNumberLine1PowerUP)
                        powerUPScriptPC = 2;
                    else
                    {
                        powerUPScriptPC = 4;
                        timeToWaitLine4PowerUP = (float)(randomGenerator.NextDouble() * 4.0 + 5.0);
                    }
                    break;
                case 2:
                    //maak powerup aan
                    createPowerUP();
                    timeToWaitLine4 -= deltaTime;
                    powerUPScriptPC = 3;
                    timeToWaitLine3PowerUP = (float)(randomGenerator.NextDouble() * 2.0 + 0.5);
                    break;
                case 3:
                    timeToWaitLine3PowerUP -= deltaTime;
                    if (timeToWaitLine3PowerUP > 0.0f)
                        powerUPScriptPC = 3;
                    else
                    {
                        powerUPScriptPC = 1;
                        //for(int iLinePowerUP = 1; iLine1PowerUP <= rndNumberLine1PowerUP;iLine1PowerUP++)
                        iLine1PowerUP++;
                    }
                    break;
                case 4:
                    timeToWaitLine4 -= deltaTime;
                    if (timeToWaitLine4PowerUP > 0.0f)
                        powerUPScriptPC = 4;
                    else
                    {
                        powerUPScriptPC = 0;
                    }
                    break;
            }*/
        }

        public void createPowerUP()
        {
            Entity powerUP = new Entity(powerUpAppearance);
            powerUP.position = asteroidTopSpawnerPos;
            int degree = randomGenerator.Next(0, 181);
            float powerUPSpeed = (float)randomGenerator.NextDouble();
            powerUP.direction = new Vector2((float)Math.Cos((degree)), (float)Math.Sin((degree))) * (powerUPSpeed + 0.5f);
            powerUP.isVisible = true;
            powerUPs.Add(powerUP);
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

            Entity asteroid = new Entity(asteroidAppearance);
            asteroid.position = spawnerLocation;
            asteroid.direction = new Vector2((float)Math.Cos((degree)), (float)Math.Sin((degree))) * asteroidSpeed;
            asteroid.isVisible = true;
            asteroidList.Add(asteroid);
        }

        public void updateAstroid()
        {
            foreach (Entity a in asteroidList)
            {
                a.position += a.direction;
                Rectangle offSetRec = new Rectangle(Window.ClientBounds.X -50,Window.ClientBounds.Y - 50,Window.ClientBounds.Width + 100,Window.ClientBounds.Height + 100);
                if (!offSetRec.Contains(a.position))
                {
                    a.isVisible = false;
                }
                foreach(Entity b in bullets)
                {
                    if(Vector2.Distance(b.position,a.position) < 20.0f)
                    {
                        a.isVisible = false;
                        b.isVisible = false;
                        score += 50;
                    }
                }
                if (Vector2.Distance(a.position, shipPosition) < 20.0f)
                {
                    hitCount++;
                    a.isVisible = false;
                }
            }
        }

        public void updatePowerUP()
        {
            foreach (Entity up in powerUPs)
            {
                up.position += up.direction;
                Rectangle offSetRec = new Rectangle(Window.ClientBounds.X - 50, Window.ClientBounds.Y - 50, Window.ClientBounds.Width + 100, Window.ClientBounds.Height + 100);
                if (!offSetRec.Contains(up.position))
                {
                    up.isVisible = false;
                }
                
                if (Vector2.Distance(up.position, shipPosition) < 20.0f)
                {
                    // TODO : powerup weapon
                    up.isVisible = false;
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


            spriteBatch.Begin();
            spriteBatch.Draw(backgroundAppearance, backgroundPosition, Color.White);
            spriteBatch.Draw(shipAppearance, shipPosition, null, Color.White, mousePositionAngle + MathHelper.PiOver2, origin, 1.0f, SpriteEffects.None, 0f);
            spriteBatch.DrawString(gameFont, "Score : " + score, new Vector2(10, 10), Color.White);
            
            foreach (Entity pu in powerUPs)
            {
                pu.draw(spriteBatch);
            }

            updatePowerUP();
            for (int i = 0; i < powerUPs.Count; i++)
            {
                if (!powerUPs[i].isVisible)
                {
                    powerUPs.RemoveAt(i);
                }
            }
            foreach(Entity a in asteroidList)
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

            foreach (Entity b in bullets)
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
