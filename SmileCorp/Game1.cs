﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System;
using System.Text;
using System.IO;
using System.Diagnostics;

namespace SmileCorp
{
    enum GameStates
    {
        Title,
        Game,
        Pause,
        GameOver
    }

    public class Game1 : Game
    {
        #region fields
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private int windowWidth;
        private int windowHeight;

        KeyboardState kbState;
        KeyboardState prevKBState;

        //Fields for gamestates
        private GameStates currentState;
        private GameStates previousState;

        // Map variables 
        private Texture2D testMap;
        private int mapWidth;
        private int mapHeight;

        private Texture2D playerImg; 
        private Player player;
        private GameObject tempCamTarget;
        private Npc npc;
        private CollisionManager collisionManager;
        private Camera camera;

        //private List<Npc> npcs;

        #endregion

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            windowWidth = _graphics.PreferredBackBufferWidth = 1000;
            windowHeight = _graphics.PreferredBackBufferHeight = 1000;

            mapHeight = 2000;
            mapWidth = 2000;

            _graphics.ApplyChanges();
            base.Initialize();
        }
         
        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            playerImg = Content.Load<Texture2D>("angelicaSpriteSheet");
            testMap = Content.Load<Texture2D>("testMap");

            player = new Player(128, 128, new Vector2(700, 700), playerImg);
            npc = new Npc(128, 128, new Vector2(30, 30), playerImg, "TestNpc");
            tempCamTarget = new GameObject(128, 128, new Vector2(0, 0));
            collisionManager = new CollisionManager();
            camera = new Camera();

            currentState = GameStates.Game;
        }

        protected override void Update(GameTime gameTime)
        {

            // Game state loop
            switch (currentState)
            {
                //Title Screen
                case GameStates.Title:

                    previousState = currentState;

                    break;

                //Game Screen --> Where the game is played
                case GameStates.Game:
                    player.Update(gameTime);
                    npc.Update(gameTime);
                    
                    kbState = Keyboard.GetState();

                    // checks to see if the player interacts with any NPCs
                    if (collisionManager.CheckCollision(player, npc, 7) && SingleKeyPress(Keys.E, kbState))
                    {
                        System.Diagnostics.Debug.WriteLine("Collision Detected");
                    }

                    // Checks to see if the player is nearing the edge of the map
                    if (CheckCameraBounds(player.Position))
                    {
                        camera.Follow(tempCamTarget, windowHeight, windowWidth);
                    }
                    else
                    {
                        camera.Follow(player, windowHeight, windowWidth);
                    }

                    prevKBState = kbState;
                    break;

                //Pause Screen
                case GameStates.Pause:
                    previousState = currentState;

                    break;

                //GameOver Screen
                case GameStates.GameOver:

                    break;
            }

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            _spriteBatch.Begin(transformMatrix: camera.Transform); //Starts drawing

            _spriteBatch.Draw(testMap, new Vector2(0, 0), Color.White);

            npc.Draw(_spriteBatch);
            player.Draw(_spriteBatch);

            _spriteBatch.End(); //Ends drawing

            base.Draw(gameTime);
        }

        //Resets the game when on the title screen
        private void GameReset()
        {
            //Resets the players position
        }

        private bool CheckCameraBounds(Vector2 playerPos)
        {

            // If the player is at the x bottom of the map
            if (playerPos.X < windowWidth / 2)
            {
                int xPos = windowWidth / 2;
                tempCamTarget.Position = new Vector2(xPos, playerPos.Y);
                return true;
            }
            // If the player is at the x top of the map
            if (playerPos.X + player.Width > mapWidth - (windowWidth / 2))
            {
                int xPos = mapWidth - (windowWidth / 2) - player.Width;
                tempCamTarget.Position = new Vector2(xPos, playerPos.Y);
                return true;
            }
            // If the player is at the y bottom of the map
            if (playerPos.Y < windowHeight / 2)
            {
                int yPos = windowHeight / 2;
                tempCamTarget.Position = new Vector2(playerPos.X, yPos);
                return true;
            }
            // If the player is at the y top of the map
            if (playerPos.Y + player.Height > mapHeight - (windowHeight / 2))
            {
                int yPos = mapHeight - (windowHeight / 2) - player.Height;
                tempCamTarget.Position = new Vector2(playerPos.X, yPos);
                return true;
            }
            return false;
        }

        // Checking for single input
        public bool SingleKeyPress(Keys key, KeyboardState kbState)
        {
            return kbState.IsKeyDown(key) && !prevKBState.IsKeyDown(key);
        }
    }
}