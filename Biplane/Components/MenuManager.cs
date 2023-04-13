using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Biplane.Objects;
using Biplane.Objects.UI;
using Biplane.Objects.Player;

namespace Biplane.Components 
{
    class MenuManager : DrawableGameComponent
    {
        private ArrayList UIobjects;

        private int selectedItem;
        private bool selectionMade;
        private long selectionTime;

        private SpriteBatch spriteBatch;

        private class UIText
        {
            public String text;
            public Vector2 position;
            public Color color;
            public float scale;

            public UIText(String text, Vector2 position, Color color, float scale)
            {
                this.text = text;
                this.position = position;
                this.color = color;
                this.scale = scale;
            }
        }

        private Queue textList = new Queue();

        public MenuManager(Game game)
            : base(game)
        {
            selectedItem = 0;
            this.selectionTime = 0;
            UIobjects = new ArrayList();
            UIobjects.Add(new UIBacking(new Rectangle(225, 50, 350, 500), Color.White));
            UIobjects.Add(new Logo(new Rectangle(250, 75, 300, 100), Color.White));
        }

        public override void Update(GameTime gameTime)
        {
                String text = "Resume Game";
                textList.Enqueue(new UIText(text, new Vector2(400, 225), Color.Red, 1.2f));

                text = "Exit Game";
                textList.Enqueue(new UIText(text, new Vector2(400, 275), Color.Red, 1.2f));

                text = "Current Stage: " + ((BiplaneGame)Game).gameState.stage;
                textList.Enqueue(new UIText(text, new Vector2(400, 325), Color.Red, 1.2f));

                text = "Current Level: " + ((BiplaneGame)Game).gameState.level;
                textList.Enqueue(new UIText(text, new Vector2(400, 375), Color.Red, 1.2f));

                text = "Z-move On: " + ((BiplaneGame)Game).gameState.zMotion;
                textList.Enqueue(new UIText(text, new Vector2(400, 425), Color.Red, 1.2f));

                text = "Idle Cam: " + ((BiplaneGame)Game).gameState.idleCam;
                textList.Enqueue(new UIText(text, new Vector2(400, 475), Color.Red, 1.2f));

                if ((gameTime.TotalRealTime.Ticks - this.selectionTime) > 1500000)
                {
                    selectionMade = acceptInput(gameTime, GamePad.GetState(PlayerIndex.One), Keyboard.GetState());
                    if (selectionMade)
                    {
                        if (selectedItem == 0)
                        {
                            ((BiplaneGame)Game).gameState.paused = false;
                            ((BiplaneGame)Game).gameState.loading = true;
                            ((BiplaneGame)Game).gameState.player1Active = false;
                            ((BiplaneGame)Game).gameState.player2Active = false;
                        }
                        else if (selectedItem == 1)
                        {
                            ((BiplaneGame)Game).Exit();
                        }
                        else if (selectedItem == 2)
                        {
                            ((BiplaneGame)Game).gameState.stage += 1;
                            if (((BiplaneGame)Game).gameState.stage > 3) ((BiplaneGame)Game).gameState.stage = 1;
                        }
                        else if (selectedItem == 3)
                        {
                            ((BiplaneGame)Game).gameState.level += 1;
                            if (((BiplaneGame)Game).gameState.level > 3) ((BiplaneGame)Game).gameState.level = 1;
                        }
                        else if (selectedItem == 4)
                        {
                            ((BiplaneGame)Game).gameState.zMotion = !((BiplaneGame)Game).gameState.zMotion;
                        }
                        else if (selectedItem == 5)
                        {
                            ((BiplaneGame)Game).gameState.idleCam += 1;
                            if (((BiplaneGame)Game).gameState.idleCam > 5) ((BiplaneGame)Game).gameState.idleCam = 1;
                        }
                    }
                    if (selectedItem > 5)
                    {
                        selectedItem = 0;
                    }
                    if (selectedItem < 0)
                    {
                        selectedItem = 5;
                    }
                }
                else
                {
                    selectionMade = false;
                }
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
                this.spriteBatch = ((BiplaneGame)Game).spriteBatch;
                // Draw the background image.
                this.spriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Deferred, SaveStateMode.SaveState);
                Viewport viewport = Game.GraphicsDevice.Viewport;
                this.spriteBatch.Draw(((BiplaneGame)Game).background,
                    new Rectangle(0, 0, viewport.Width, viewport.Height), Color.White);

                // draw logo and other static UI elements
                foreach (GameObject gameObject in UIobjects)
                {
                    if (((GameObject2D)gameObject).texture == null)
                    {
                        ((GameObject2D)gameObject).texture = Game.Content.Load<Texture2D>(((GameObject2D)gameObject).textureName);
                    }
                    ((GameObject2D)gameObject).draw(this.spriteBatch);
                }

                int i = 0;
                while (textList.Count > 0)
                {
                    UIText uit = (UIText)textList.Dequeue();

                    Color color = Color.Blue;
                    float scale = 1.2f;
                    if (selectedItem == i)
                    {
                        color = Color.White;
                        scale = 1.5f;
                    }

                    Vector2 shadowPos = new Vector2(uit.position.X - 2, uit.position.Y - 2);

                    SpriteFont trueTypeFont = Game.Content.Load<SpriteFont>("ComicSans");
                    Vector2 FontOrigin = trueTypeFont.MeasureString(uit.text) / 2;
                    spriteBatch.DrawString(trueTypeFont, uit.text, shadowPos, Color.Black, 0, FontOrigin, scale, SpriteEffects.None, 0.5f);
                    spriteBatch.DrawString(trueTypeFont, uit.text, uit.position, color, 0, FontOrigin, scale, SpriteEffects.None, 0.5f);
                    i++;
                }

                this.spriteBatch.End();

            base.Draw(gameTime);
        }

        public bool acceptInput(GameTime gameTime, GamePadState gs, KeyboardState ks)
        {
            bool gotInput = false;

            float seconds = gameTime.ElapsedGameTime.Milliseconds / 1000.0f;
            if (ks.IsKeyDown(Keys.Up))
            {
                this.selectedItem -= 1;
                this.selectionTime = gameTime.TotalRealTime.Ticks;
            }
            if (ks.IsKeyDown(Keys.Down))
            {
                this.selectedItem += 1;
                this.selectionTime = gameTime.TotalRealTime.Ticks;
            }
            if (ks.IsKeyDown(Keys.Space))
            {
                gotInput = true;
                this.selectionTime = gameTime.TotalRealTime.Ticks;
            }
            if (gs.IsConnected)
            {
                if (gs.ThumbSticks.Left.Y > 0.0f)
                {
                    this.selectedItem -= 1;
                    this.selectionTime = gameTime.TotalRealTime.Ticks;
                }
                if (gs.ThumbSticks.Left.Y < 0.0f)
                {
                    this.selectedItem += 1;
                    this.selectionTime = gameTime.TotalRealTime.Ticks;
                }
                if (gs.Buttons.A.Equals(ButtonState.Pressed))
                {
                    gotInput = true;
                    this.selectionTime = gameTime.TotalRealTime.Ticks;
                }
            }

            return gotInput;
        }
    }
}
