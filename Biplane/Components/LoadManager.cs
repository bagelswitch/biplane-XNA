using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading;
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
    class LoadManager : DrawableGameComponent
    {
        private SpriteBatch spriteBatch;

        private Texture2D background;

        private bool loading;

        public LoadManager(Game game)
            : base(game)
        {  
        }

        public override void Update(GameTime gameTime)
        {
            this.loading = ((BiplaneGame)Game).gameState.loading;

            if (this.loading)
            {
                BiplaneGame game = ((BiplaneGame)Game);

                int stage = ((((BiplaneGame)Game).gameState.stage - 1) % 3) + 1;

                game.background = game.Content.Load<Texture2D>("Textures\\sky" + stage);

                List<GameObject> objects = game.gameState.objects;

                foreach (GameObject gameObject in objects)
                {
                    if (gameObject is Terrain)
                    {
                        ((Terrain)gameObject).unloadModel();
                        ((Terrain)gameObject).modelName = "Heightmap\\terrain" + stage;
                        ((Terrain)gameObject).loadModel();
                    }
                }

                this.loading = false;
            }

            ((BiplaneGame)Game).gameState.loading = this.loading;

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            BiplaneGame game = ((BiplaneGame)this.Game);

            this.spriteBatch = ((BiplaneGame)Game).spriteBatch;
            // Draw the background image.
            spriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Deferred, SaveStateMode.None);
            Viewport viewport = game.GraphicsDevice.Viewport;
            this.spriteBatch.Draw(game.background,
                new Rectangle(0, 0, viewport.Width, viewport.Height), Color.White);

            Game.GraphicsDevice.RenderState.AlphaBlendEnable = true;

            SpriteFont trueTypeFont = Game.Content.Load<SpriteFont>("ComicSans");
            Vector2 FontOrigin = trueTypeFont.MeasureString("Loading . . .") / 2;
            spriteBatch.DrawString(trueTypeFont, "Loading . . .", new Vector2(400, 300), Color.Red, 0, FontOrigin, 1.0f, SpriteEffects.None, 0.5f);

            this.spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
