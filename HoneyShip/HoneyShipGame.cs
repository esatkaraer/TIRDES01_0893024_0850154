using HoneyShip.HoneyShip;
using HoneyShip.Scripts;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using SlimDX.DirectInput;
using System;
using System.Collections.Generic;
using System.IO;
using System.Media;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace HoneyShip
{
    enum InstructionResult
    {
        Done,
        DoneAndCreateAsteroid,
        Running,
        RunningAndCreateAsteroid
    }
    public class HoneyShipGame : Game
    {
        [DllImport("winmm.dll")]
        static extern Int32 mciSendString(string command, StringBuilder buffer, int bufferSize, IntPtr hwndCallback);

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        SpriteFont gameFont;
        int hitCount = 0;
        Random randomGenerator = new Random();
        List<Weapon<Entity>> weaponsList = new List<Weapon<Entity>>();
        int currentWeaponIndex = 0;
        /*Asteroid spawn script variables
        int gameLogicScriptPC = 0;
        int rndNumberLine1, iLine1;
        float timeToWaitLine3, timeToWaitLine4, timeToWaitLine8, timeToWaitLine7;
        int rndNumberLine5, iLine5;

        Powerup spawn script variables
        int powerUPScriptPC = 0;
        int iLine1PowerUP,rndNumberLine1PowerUP;
        float timeToWaitLine3PowerUP, timeToWaitLine4PowerUP;*/

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

        static Random rand = new Random();
        Instruction astroidSpawnLogic =
          new Repeat(
              new For(0, 10, i =>
                    new Wait(() => i * 0.1f) +
                    new CreateEntity()) +
              new Wait(() => rand.Next(1, 5)) +
              new For(0, 10, i =>
                    new Wait(() => (float)rand.NextDouble() * 1.0f + 0.2f) +
                    new CreateEntity()) +
              new Wait(() => rand.Next(2, 3)));

        Instruction powerupSpawnLogic =
          new Repeat(
              new For(0, 10, i =>
                    new Wait(() => (float)rand.NextDouble() * 1.0f + 5f) +
                    new CreateEntity()) +
              new Wait(() => rand.Next(2, 3)));

        Vector2 backgroundPosition;
        Texture2D backgroundAppearance;
        Vector2 shipPosition;
        Texture2D shipAppearance;
        Texture2D bulletAppearance;
        Texture2D asteroidAppearance;
        Texture2D powerUpAppearance;

        Vector2 asteroidTopSpawnerPos;
        Vector2 asteroidBotSpawnerPos;
        Vector2 asteroidLeftSpawnerPos;
        Vector2 asteroidRightSpawnerPos;

        int screenWidth;
        int screenHeight;

        int score = 0;
        List<Entity> bullets = new List<Entity>();
        //list aanmaken voor alle astroids
        List<Entity> asteroidList = new List<Entity>();
        List<Entity> powerUPs = new List<Entity>();

        Weapon<Entity> currentWeapon;

        protected override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            backgroundPosition = new Vector2(-150.0f,-150.0f);
            screenWidth = Window.ClientBounds.Width;
            screenHeight = Window.ClientBounds.Height;

            shipPosition = new Vector2((float)(screenWidth / 2), (float)(screenHeight / 2));

            backgroundAppearance = Content.Load<Texture2D>("background.jpg");

            shipAppearance = Content.Load<Texture2D>("ship.png");

            bulletAppearance = Content.Load<Texture2D>("plasma.png");

            asteroidAppearance = Content.Load<Texture2D>("asteroid.png");

            powerUpAppearance = Content.Load<Texture2D>("powerup.png");

            weaponsList.Add(new SoloBlaster(Content));
            weaponsList.Add(new DualBlaster(Content));
            weaponsList.Add(new TripleBlaster(Content));
            weaponsList.Add(new QuadBlaster(Content));
            weaponsList.Add(new PentaBlaster(Content));

            currentWeapon = weaponsList[currentWeaponIndex];

            asteroidTopSpawnerPos = new Vector2(Window.ClientBounds.Width / 2, -49);
            asteroidLeftSpawnerPos = new Vector2(-49, Window.ClientBounds.Height / 2);
            asteroidRightSpawnerPos = new Vector2(Window.ClientBounds.Width + 49, Window.ClientBounds.Height / 2);
            asteroidBotSpawnerPos = new Vector2(Window.ClientBounds.Width / 2, Window.ClientBounds.Height + 49);

            //mciSendString(@"open C:\Users\esatk\Desktop\Ascendency.wav type waveaudio alias Ascendency", null, 0, IntPtr.Zero);
            //mciSendString(@"play Ascendency", null, 0, IntPtr.Zero);
         
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
            Vector2 mouseLoc = new Vector2(Microsoft.Xna.Framework.Input.Mouse.GetState().Position.X, Microsoft.Xna.Framework.Input.Mouse.GetState().Position.Y);
            rotationAngle = (float)Math.Atan2(mouseLoc.Y - shipPosition.Y, mouseLoc.X - shipPosition.X);
            mousePositionAngle = rotationAngle;

            shipPosition += input.ShipMovement(rotationAngle) * 2.0f;

            //Geef aan wapen door waar schippositie is , welke hoek hij moet schieten en hoe kogel eruit ziet.
            currentWeapon.Update(deltaTime, shipPosition,rotationAngle,bulletAppearance);

            if(input.isShooting)
            {
                    currentWeapon.pullTrigger();
            }
            //alle bullets die zijn aangemaakt in de wapen. zetten we in de lijst van bullets die al geschoten.
            bullets.AddRange(currentWeapon.newBullets());

            if (!Window.ClientBounds.Contains(shipPosition))
            {
                Exit();
            }
            //selecteer random spawner ( top left right bottom )
            int currentSpawner = randomGenerator.Next(0, 4);

            switch (astroidSpawnLogic.Execute(deltaTime))
            {
                case InstructionResult.DoneAndCreateAsteroid:
                    createAstroid(currentSpawner);
                    break;
                case InstructionResult.RunningAndCreateAsteroid:
                    createAstroid(currentSpawner);
                    break;
            }

            switch (powerupSpawnLogic.Execute(deltaTime))
            {
                case InstructionResult.DoneAndCreateAsteroid:
                    if (powerUPs.Count < 1)
                    {
                        createPowerUP();
                    }
                    break;
                case InstructionResult.RunningAndCreateAsteroid:
                    if (powerUPs.Count < 1)
                    {
                        createPowerUP();
                    }
                    break;
            }
            
            base.Update(gameTime);
        }

        /*public void shoot()
        {

        }
        */
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

        /* public void spawnAstroids()
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
         */

        /*public void spawnPowerUP()
        {
            switch (powerUPScriptPC)
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
                        timeToWaitLine4PowerUP = (float)(randomGenerator.NextDouble() * 2.0 + 10.0);
                    }
                    break;
                case 2:
                    //maak powerup aan
                    if(powerUPs.Count < 1)
                    {
                        createPowerUP();
                    }
                    timeToWaitLine4 -= deltaTime;
                    powerUPScriptPC = 3;
                    timeToWaitLine3PowerUP = (float)(randomGenerator.NextDouble() * 4.0 + 0.5);
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
            }
        }*/

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
            
            float asteroidSpeed = (float)randomGenerator.NextDouble() + 0.3f;

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
            // powerups in een list entity
            foreach (Entity up in powerUPs)
            {
                // powerups de direction aangeven waar die heen moet
                up.position += up.direction;
                Rectangle offSetRec = new Rectangle(Window.ClientBounds.X - 50, Window.ClientBounds.Y - 50, Window.ClientBounds.Width + 100, Window.ClientBounds.Height + 100);

                // powerups 
                if (!offSetRec.Contains(up.position))
                {
                    up.isVisible = false;
                }
                //checken of onze schip en een powerup elkaar "raken"
                if (Vector2.Distance(up.position, shipPosition) < 20.0f)
                {
                    //als weapons niet beste is dan maken we wapen beter.
                    if (currentWeaponIndex < 4)
                    {
                        currentWeaponIndex++;
                    }
                    currentWeapon = weaponsList[currentWeaponIndex];
                    up.isVisible = false;
                    SoundPlayer powerupSound = new SoundPlayer(@"C:\Users\esatk\Desktop\powerup.wav");
                    powerupSound.Play();
                }
            }
        }

        protected override void Draw(GameTime gameTime)
        {
            // achtergrond plaats geven
            GraphicsDevice.Clear(Color.CornflowerBlue);
            Vector2 origin = new Vector2();

            // plaats geven van de ship zelf
            origin.X = shipAppearance.Width / 2;
            origin.Y = shipAppearance.Height / 2;

            // plaats geven van de asteroid zelf
            Vector2 astroidOrigin = new Vector2();
            astroidOrigin.X = asteroidAppearance.Width / 2;
            astroidOrigin.Y = asteroidAppearance.Height / 2;

            // achtergrond tekenen postbode aangeven en laten bezorgen
            spriteBatch.Begin();
            spriteBatch.Draw(backgroundAppearance, backgroundPosition, Color.White);
            spriteBatch.Draw(shipAppearance, shipPosition, null, Color.White, mousePositionAngle + MathHelper.PiOver2, origin, 1.0f, SpriteEffects.None, 0f);
            spriteBatch.DrawString(gameFont, "Score : " + score, new Vector2(10, 10), Color.White);
            spriteBatch.DrawString(gameFont, "Current Weapon : " + currentWeapon.GetType().Name, new Vector2(10, screenHeight - 20), Color.White);
            
            // powerups op het scherm tekenen
            foreach (Entity pu in powerUPs)
            {
                pu.draw(spriteBatch);
            }

            // powerup laten verwijderen voorbij de kader
            updatePowerUP();
            for (int i = 0; i < powerUPs.Count; i++)
            {
                if (!powerUPs[i].isVisible)
                {
                    powerUPs.RemoveAt(i);
                }
            }

            // alle asteroid in de list zetten die zichtbaar zijn op het scherm
            foreach(Entity a in asteroidList)
            {
                a.draw(spriteBatch);
            }

            updateAstroid();

            // asteroid laten verwijderen voorbij de kader
            for (int i = 0; i < asteroidList.Count; i++)
            {
                if(!asteroidList[i].isVisible)
                {
                    asteroidList.RemoveAt(i);
                }
            }

            // teken bullet en bullet in een list entity
            foreach (Entity b in bullets)
            {
                b.draw(spriteBatch);
            }

            // bullet verwijderen als die niet in het scherm aanwezig is
            updateBullets();
            for (int i = 0; i < bullets.Count; i++)
            {
                if (!bullets[i].isVisible)
                {
                    bullets.RemoveAt(i);
                }
            }

            // postbode taak is gedaan
            spriteBatch.End();

            // uitvoeren gameTime() en teken het
            base.Draw(gameTime);
        }
    }
}
