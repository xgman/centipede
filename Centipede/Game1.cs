﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Centipede
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        // saved so they don't have to be loaded every time sprites are created
        public static Texture2D pointSprite;
        public static Texture2D bulletSprite;
        public static Texture2D playerSprite;
        public static Texture2D mushroomSprite;
        public static Texture2D scoreSprite;
        public static Texture2D wurmHead;

        static Random random = new Random();

        Player player;
        Mushroom mushroom;
        MushroomGrid mushroomGrid;
        Wurmhead wurmhead;
        Score score;

        static List<Bullet> bullets = new List<Bullet>();
        static List<Wurmhead> wurmheads = new List<Wurmhead>();

        static int scoreValue = 0;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            graphics.PreferredBackBufferWidth = GameConstants.WindowWidth;
            graphics.PreferredBackBufferHeight = GameConstants.WindowHeight;
            IsMouseVisible = false;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // create a new SpriteBatch, which can be used to draw textures
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // load sprites
            pointSprite = Content.Load<Texture2D>(@"graphics\point");
            bulletSprite = Content.Load<Texture2D>(@"graphics\bullet");
            playerSprite = Content.Load<Texture2D>(@"graphics\player");
            mushroomSprite = Content.Load<Texture2D>(@"graphics\mushroom");
            scoreSprite = Content.Load<Texture2D>(@"graphics\score");
            wurmHead = Content.Load<Texture2D>(@"graphics\head2");

            // add initial game objects
            player = new Player(spriteBatch, playerSprite, 24, 24,
                new Vector2(GameConstants.WindowWidth / 2 - 12, GameConstants.WindowHeight - 12));

            wurmheads.Add(new Wurmhead(spriteBatch, wurmHead, 24, 24, new Vector2(random.Next(0, 31) * 24, 24), random.Next(1, 8)));
            mushroom = new Mushroom(spriteBatch, mushroomSprite, 24, 24, Vector2.Zero);
            mushroomGrid = new MushroomGrid(spriteBatch, mushroom);
            score = new Score(spriteBatch, scoreSprite, 21, 21, Vector2.Zero);

            Mouse.SetPosition(player.Rectangle.Center.X, player.Rectangle.Center.Y);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            

            Console.WriteLine(wurmheads.Count);
            if(wurmheads.Count == 0) wurmheads.Add(new Wurmhead(spriteBatch, wurmHead,
                24, 24, new Vector2(random.Next(0, 31) * 24, 24), random.Next(1, 8)));

            if (Keyboard.GetState().IsKeyDown(Keys.Escape)) { Exit(); }

            MouseState mouseState = Mouse.GetState();

            foreach(Wurmhead wurmhead in wurmheads)
            {
                if(player.Rectangle.Contains(wurmhead.Rectangle)) player.Active = false;
            }
           

            foreach (Wurmhead wurmhead in wurmheads)
            {
                wurmhead.Update(gameTime);
            }

            player.Update(gameTime, mouseState, mushroomGrid);

            foreach (Bullet bullet in bullets)
            {
                bullet.Update(gameTime);
            }

            foreach (Bullet bullet in bullets)
            {

                    for (int count = 0; count < MushroomGrid.mushrooms.Count; count++)
                    {
                        if (bullet.Rectangle.Intersects(MushroomGrid.mushrooms[count].Rectangle))
                        {
                            bullet.Active = false;
                            MushroomGrid.mushrooms[count].Shot();
                            if (mushroomGrid.Mushrooms[count].AnimationState == 5) MushroomGrid.mushrooms.RemoveAt(count);


                        }
                    }
                
            }

            foreach(Bullet bullet in bullets)
            {
                foreach(Wurmhead wurmhead in wurmheads)
                {
                    if(bullet.Rectangle.Intersects(wurmhead.Rectangle))
                    {
                        AddScore(300);
                        bullet.Active = false;
                        wurmhead.active = false;
                        
                    } 
                }
            }

            

            // clean out inactive bullets
            for (int i = bullets.Count - 1; i >= 0; i--)
            {
                if (!bullets[i].Active)
                {
                    bullets.RemoveAt(i);
                }
            }

            // clean out inactive wurmheads
            
            for (int i = wurmheads.Count - 1; i >= 0; i--)
            {
                if (!wurmheads[i].Active)
                {
                    wurmheads.RemoveAt(i);
                }
            }
            



            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            spriteBatch.Begin(SpriteSortMode.FrontToBack);

            player.Draw();
            mushroomGrid.Draw();
            foreach (Bullet bullet in bullets)
            {
               bullet.Draw();
            }
            foreach (Wurmhead wurmhead in wurmheads)
            {
                wurmhead.Draw();
            }

            score.Draw(scoreValue);

            spriteBatch.End();
            base.Draw(gameTime);
        }
        public static void AddBullet(Bullet bullet)
        {
            bullets.Add(bullet);

        }
        public static void AddScore(int score)
        {
            scoreValue += score;
        }
    }
}
