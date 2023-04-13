using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Biplane.Objects;
using Biplane.Objects.UI;
using Biplane.Objects.Target;
using Biplane.Objects.Static;
using Biplane.Objects.Player;
using Biplane.Objects.Weapon;
using Biplane.Service;

namespace Biplane.Components
{
    class GameObjectManager : DrawableGameComponent
    {
        private SpriteBatch spriteBatch;

        public GameObjectManager(Game game)
            : base(game)
        {
            List <GameObject> objects = ((BiplaneGame)game).gameState.objects;

            objects.Add(new Terrain(game));
            objects.Add(new Water(game));
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

                BiplaneGame game = ((BiplaneGame)Game);

                List<GameObject> objects = game.gameState.objects;

                Array oldObjects = objects.ToArray();

                int gameObjectCount = 0;

                foreach (GameObject gameObject in oldObjects)
                {
                    gameObjectCount++;

                    Vector3 screenVector = Game.GraphicsDevice.Viewport.Project(gameObject.position, ((BiplaneGame)this.Game).gameState.projectionMatrix, ((BiplaneGame)this.Game).gameState.viewMatrix, Matrix.CreateTranslation(0, 0, 0));
                    gameObject.update(gameTime, screenVector);

                    if (!((BiplaneGame)Game).gameState.zMotion)
                    {
                        gameObject.position.Z = 0;
                    }

                    if (gameObject is GameObjectGenerator)
                    {
                        ((GameObjectGenerator)gameObject).generateObjects(gameTime, screenVector, objects);
                    }
                    if (gameObject is Destroyable)
                    {
                        if (gameObject.destroyed)
                        {
                            objects.Remove(gameObject);
                            ((Destroyable)gameObject).destroyObject(screenVector, gameTime, objects);
                            if (gameObject is Aircraft && ((Aircraft)gameObject).playerIndex == 1)
                            {
                                ((BiplaneGame)this.Game).gameState.player1score += gameObject.score;
                                ((BiplaneGame)this.Game).gameState.player1Alive = false;
                            }
                            else if (gameObject is Aircraft && ((Aircraft)gameObject).playerIndex == 2)
                            {
                                ((BiplaneGame)this.Game).gameState.player2score += gameObject.score;
                                ((BiplaneGame)this.Game).gameState.player2Alive = false;
                            }
                            gameObject.parent = null;
                            if (gameObject is Targeter)
                            {
                                ((Targeter)gameObject).target = null;
                                Targeter.recycle(gameObject);
                            }
                        }
                        else
                        {
                            ((Destroyable)gameObject).checkCollisions(objects, screenVector, gameTime, objects);
                        }
                    }
                    if (gameObject is Targeter && !gameObject.destroyed)
                    {
                        ((Targeter)gameObject).checkTargeters(objects);
                    }
                    if (gameObject is SoundGenerator)
                    {
                        ((SoundGenerator)gameObject).playSounds(((BiplaneGame)this.Game).soundBank);
                    }
                }

                /*UITextProvider uitService = (UITextProvider)Game.Services.GetService(typeof(UITextProvider));
                String text = "gameObject count: " + gameObjectCount;
                uitService.addText(gameTime, text, new Vector2(450, 100), Color.Green, 0.5f, true);*/
            
        }

        public override void Draw(GameTime gameTime)
        {
                BiplaneGame game = ((BiplaneGame)this.Game);

                List<GameObject> objects = ((BiplaneGame)this.Game).gameState.objects;

                this.spriteBatch = ((BiplaneGame)Game).spriteBatch;

                // Draw the background image.
                this.spriteBatch.Begin(SpriteBlendMode.None, SpriteSortMode.Deferred, SaveStateMode.SaveState);
                Viewport viewport = game.GraphicsDevice.Viewport;
                this.spriteBatch.Draw(game.background,
                    new Rectangle(0, 0, viewport.Width, viewport.Height), Color.White);
                this.spriteBatch.End();

                // draw 3D models and primitives
                foreach (GameObject gameObject in objects)
                {
                    if (gameObject is GameObject3DModel)
                    {
                        ((GameObject3DModel)gameObject).draw(game.gameState.viewMatrix, game.gameState.projectionMatrix);
                    }
                }

            base.Draw(gameTime);
        }
    }
}
