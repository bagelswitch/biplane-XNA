using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Biplane.Objects;
using Biplane.Objects.Player;
using Biplane.Objects.Weapon;
using Biplane.Objects.Static;
using Biplane.Service;

namespace Biplane.Components
{
    class PlayerManager : GameComponent
    {
        private static Player1 player1;
        private static Player2 player2;

        private static Vector3 p1start = new Vector3(-25000.0f, 10000.0f, 0.0f);
        private static Vector3 p2start = new Vector3(25000.0f, 10000.0f, 0.0f);

        public PlayerManager(Game game)
            : base(game)
        {
            player1 = new Player1(true, true, this.Game);
            player2 = new Player2(true, true, this.Game);
        }

        public override void Update(GameTime gameTime)
        {
            BiplaneGame game = ((BiplaneGame)this.Game);

            List<GameObject> objects = game.gameState.objects;

            bool foundplayer1 = false;
            bool foundplayer2 = false;
            bool founddebris = false;

            foreach (GameObject gameObject in objects)
            {
                if (gameObject is Aircraft && ((Aircraft)gameObject).playerIndex == 1 )//&& !((Aircraft)gameObject).destroyed)
                {
                        foundplayer1 = true;
                        game.gameState.player1pos = gameObject.position;
                        game.gameState.player1 = gameObject;
                        game.gameState.player1score += gameObject.score;
                        gameObject.score = 0;
                        CheckInput(gameTime, gameObject);
                        game.gameState.player1Alive = true;
                        CameraProvider camService = (CameraProvider)Game.Services.GetService(typeof(CameraProvider));
                        if ((game.gameState.idleCam == 1 || game.gameState.idleCam == 2) && !game.gameState.player1Active && !game.gameState.player2Active)
                        {
                            camService.AddCamera(gameObject.position, true);
                        }
                        else if (game.gameState.player1Active)
                        {
                            camService.AddCamera(gameObject.position, true);
                            camService.AddCamera(gameObject.position, true);
                            camService.AddCamera(gameObject.position, true);
                            camService.AddCamera(gameObject.position, true);
                            camService.AddCamera(gameObject.position, true);
                        }
                }
                else if (gameObject is Aircraft && ((Aircraft)gameObject).playerIndex == 2 )//&& !((Aircraft)gameObject).destroyed)
                {
                        foundplayer2 = true;
                        game.gameState.player2pos = gameObject.position;
                        game.gameState.player2 = gameObject;
                        game.gameState.player2score += gameObject.score;
                        gameObject.score = 0;
                        CheckInput(gameTime, gameObject);
                        game.gameState.player2Alive = true;
                        CameraProvider camService = (CameraProvider)Game.Services.GetService(typeof(CameraProvider));
                        if ((game.gameState.idleCam == 1 || game.gameState.idleCam == 2) && !game.gameState.player1Active && !game.gameState.player2Active)
                        {
                            camService.AddCamera(gameObject.position, true);
                        }
                        else if (game.gameState.player2Active)
                        {
                            camService.AddCamera(gameObject.position, true);
                            camService.AddCamera(gameObject.position, true);
                            camService.AddCamera(gameObject.position, true);
                            camService.AddCamera(gameObject.position, true);
                            camService.AddCamera(gameObject.position, true);
                        }
                }
                else if (gameObject is Debris)
                { 
                    if (Math.Abs(gameObject.position.X) < 0)
                    {
                        founddebris = true;
                    }
                }
            }
            if (!foundplayer1 && !founddebris && !game.gameState.player1Alive && gameTime.TotalRealTime.Ticks - player1.createTime > 10000000)
            {
                game.gameState.player1 = player1;
                player1.setup(new Vector3(-25000.0f, Terrain.GetHeight(p1start) + 40000.0f, 0), new Vector3(0.001f, -0.001f, 0.000f), gameTime);
                game.gameState.player1pos = game.gameState.player1.position;
                objects.Add(player1);
                game.gameState.player1Alive = true;
                game.gameState.player1Active = false;
            }
            if (!foundplayer2 && !founddebris && !game.gameState.player2Alive && gameTime.TotalRealTime.Ticks - player2.createTime > 10000000)
            {
                game.gameState.player2 = player2;
                player2.setup(new Vector3(25000.0f, Terrain.GetHeight(p2start) + 40000.0f, 0), new Vector3(0.001f, 0.001f, 0.000f), gameTime);
                game.gameState.player2pos = game.gameState.player2.position;
                objects.Add(player2);
                game.gameState.player2Alive = true;
                game.gameState.player2Active = false;
            }
            base.Update(gameTime);
        }

        private void CheckInput(GameTime gameTime, GameObject gameObject)
        {
            KeyboardState currentState = Keyboard.GetState();

            BiplaneGame game = ((BiplaneGame)this.Game);

            if (((Aircraft)gameObject).playerIndex == 1)
            {
                if (((Aircraft)gameObject).acceptInput(gameTime, GamePad.GetState(PlayerIndex.One), Keyboard.GetState()))
                {
                    game.gameState.player1Active = true;
                }
            }
            else if (((Aircraft)gameObject).playerIndex == 2)
            {
                if (((Aircraft)gameObject).acceptInput(gameTime, GamePad.GetState(PlayerIndex.Two), Keyboard.GetState()))
                {
                    game.gameState.player2Active = true;
                }
            }
        }
    }
}
