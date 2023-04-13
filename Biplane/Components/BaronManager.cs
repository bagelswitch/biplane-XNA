using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Biplane;
using Biplane.Objects;
using Biplane.Objects.Target;
using Biplane.Objects.Weapon;
using Biplane.Service;

namespace Biplane.Components
{
    class BaronManager : GameComponent
    {
        private static Random baronRand = new Random();

        private static List<Baron> deadBarons;

        public BaronManager(Game game)
            : base(game)
        {
            deadBarons = new List<Baron>();
        }

        private static Baron newBaron(Game game, GameTime gameTime)
        {
            Baron baron = null;
            if (deadBarons.Count > 0)
            {
                baron = deadBarons[0];
                deadBarons.Remove(baron);
            }
            else
            {
                baron = new Baron(game);
            }
            baron.setup(gameTime);
            return baron;
        }

        public override void Update(GameTime gameTime)
        {
            BiplaneGame game = ((BiplaneGame)this.Game);

            List<GameObject> objects = ((BiplaneGame)Game).gameState.objects;

            bool foundbaron1 = false;
            bool foundbaron2 = false;
            bool foundDebris = false;

            foreach (GameObject gameObject in objects)
            {
                if (gameObject is Baron)
                {
                    if (!foundbaron1)
                    {
                        foundbaron1 = true;
                        if (!gameObject.destroyed)
                        {
                            if (game.gameState.idleCam == 1 || game.gameState.idleCam == 3)
                            {
                                CameraProvider camService = (CameraProvider)Game.Services.GetService(typeof(CameraProvider));
                                camService.AddCamera(gameObject.position, false);
                            }
                        }
                    }
                    else
                    {
                        foundbaron2 = true;
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
                        deadBarons.Add((Baron)gameObject);
                    }
                }
                else if (gameObject is Debris)
                {
                    foundDebris = true;
                }
            }
            if (((BiplaneGame)Game).gameState.level > 1 && !foundbaron1)// && !foundDebris)
            {
                Baron baron1 = newBaron(Game, gameTime);
                baron1.name = "baron1";
                baron1.team = "red";
                //baron1.modelName = "Models\\baron";
                objects.Add(baron1);
            }
            if (((BiplaneGame)Game).gameState.level > 1 && !foundbaron2)// && !foundDebris)
            {
                Baron baron2 = newBaron(Game, gameTime);
                baron2.name = "baron2";
                baron2.team = "blue";
                //baron2.modelName = "Models\\baron";
                objects.Add(baron2);
            }
            base.Update(gameTime);
        }
    }
}
