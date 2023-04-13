using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Biplane.Objects;
using Biplane.Objects.Target;
using Biplane.Objects.Weapon;
using Biplane.Service;

namespace Biplane.Components
{
    class BomberManager : GameComponent
    {

        private static Random bomberRand = new Random();

        private static List<Bomber> deadBombers;

        public BomberManager(Game game)
            : base(game)
        {
            deadBombers = new List<Bomber>();
        }

        private static Bomber newBomber(Game game, GameTime gameTime)
        {
            Bomber bomber = null;
            if (deadBombers.Count > 0)
            {
                bomber = deadBombers[0];
                deadBombers.Remove(bomber);
            }
            else
            {
                bomber = new Bomber(game);
            }
            bomber.setup(gameTime);
            return bomber;
        }

        public override void Update(GameTime gameTime)
        {
            BiplaneGame game = ((BiplaneGame)this.Game);

            List<GameObject> objects = ((BiplaneGame)Game).gameState.objects;

            bool foundbomber1 = false;
            bool foundbomber2 = false;
            bool foundDebris = false;

            foreach (GameObject gameObject in objects)
            {
                if (gameObject is Bomber)
                {
                    if (!foundbomber1)
                    {
                        foundbomber1 = true;
                        if (!gameObject.destroyed)
                        {
                            if (game.gameState.idleCam == 1 || game.gameState.idleCam == 5)
                            {
                                CameraProvider camService = (CameraProvider)Game.Services.GetService(typeof(CameraProvider));
                                camService.AddCamera(gameObject.position, false);
                            }
                        }
                    }
                    else
                    {
                        foundbomber2 = true;
                        if (!gameObject.destroyed)
                        {
                            if (game.gameState.idleCam == 1)
                            {
                                CameraProvider camService = (CameraProvider)Game.Services.GetService(typeof(CameraProvider));
                                camService.AddCamera(gameObject.position, false);
                            }
                        }
                    }

                    if (gameObject.destroyed)
                    {
                        deadBombers.Add((Bomber)gameObject);
                    }
                }
                else if (gameObject is Debris)
                {
                    foundDebris = true;
                }
            }
            if (((BiplaneGame)Game).gameState.level > 2 && !foundbomber1)// && !foundDebris)
            {
                Bomber bomber1 = newBomber(Game, gameTime);
                bomber1.name = "bomber1";
                bomber1.team = "red";
                objects.Add(bomber1);
            }
            if (((BiplaneGame)Game).gameState.level > 2 && !foundbomber2)// && !foundDebris)
            {
                Bomber bomber2 = newBomber(Game, gameTime);
                bomber2.name = "bomber2";
                bomber2.team = "red";
                objects.Add(bomber2);
            }
            base.Update(gameTime);
        }
    }
}
