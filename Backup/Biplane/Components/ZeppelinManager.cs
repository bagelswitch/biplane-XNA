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
    class ZeppelinManager : GameComponent
    {

        private static Random baronRand = new Random();

        private static List<Zeppelin> deadZeppelins;

        public ZeppelinManager(Game game)
            : base(game)
        {
            deadZeppelins = new List<Zeppelin>();
        }

        private static Zeppelin newZeppelin(Game game, GameTime gameTime)
        {
            Zeppelin zeppelin = null;
            if (deadZeppelins.Count > 0)
            {
                zeppelin = deadZeppelins[0];
                deadZeppelins.Remove(zeppelin);
            }
            else
            {
                zeppelin = new Zeppelin(game);
            }
            zeppelin.setup(gameTime);
            return zeppelin;
        }

        public override void Update(GameTime gameTime)
        {
            BiplaneGame game = ((BiplaneGame)this.Game);

            List<GameObject> objects = ((BiplaneGame)this.Game).gameState.objects;

            int zeppelinCount = 0;
            bool foundDebris = false;

            foreach (GameObject gameObject in objects)
            {
                if (gameObject is Zeppelin)
                {
                    zeppelinCount += 1;
                    if (!gameObject.destroyed)
                    {
                        if (game.gameState.idleCam == 1 || (game.gameState.idleCam == 4 && zeppelinCount == 1))
                        {
                            CameraProvider camService = (CameraProvider)Game.Services.GetService(typeof(CameraProvider));
                            camService.AddCamera(gameObject.position, false);
                        }
                    }

                    if (gameObject.destroyed)
                    {
                        deadZeppelins.Add((Zeppelin)gameObject);
                    }
                }
                else if (gameObject is Debris)
                {
                    foundDebris = true;
                }
            }
            while (((BiplaneGame)this.Game).gameState.level > 2 && zeppelinCount < 4)// && !foundDebris)
            {
                Zeppelin zeppelin = newZeppelin(Game, gameTime);
                objects.Add(zeppelin);
                zeppelin.team = "blue";
                zeppelinCount++;
            }
            base.Update(gameTime);
        }
    }
}
