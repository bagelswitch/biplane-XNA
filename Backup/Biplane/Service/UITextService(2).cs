using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Biplane.Objects;
using Biplane.Objects.UI;
using Biplane.Objects.Player;

namespace Biplane.Service
{
    class UITextService : DrawableGameComponent, UITextProvider 
    {
        private List<GameObject2D> UIobjects;
        private List<UIText> textList;
        private List<UIText> debugList;

        private static SpriteBatch spriteBatch;

        private static SpriteFont guiFont;
        private static SpriteFont debugFont;

        private static string health = "Health: ";
        private static string p1health;
        private static string p2health;
        private static float p1healthval = 0.0f;
        private static float p2healthval = 0.0f;
        private static string score = "Score: ";
        private static string p1score;
        private static string p2score;
        private static int p1scoreval = -1;
        private static int p2scoreval = -1;
        private static string percent = "%";
        private static string missles = "Msl: ";
        private static string p1weapons;
        private static int p1misslesval = -1;
        private static int p2misslesval = -1;
        private static string bombs = "Bmb: ";
        private static string p2weapons;
        private static int p1bombsval = -1;
        private static int p2bombsval = -1;
        private static string space = "  ";

        private static StringBuilder sb;

        private class UIText {
            public string text;
            public Vector2 position;
            public Color color;
            public float scale;
            public bool isDebug;
        }

        public UITextService(Game game)
            : base(game)
        {
            UIobjects = new List<GameObject2D>();
            UIobjects.Add(new UIBacking(new Rectangle(0, 500, 250, 75), Color.White));
            UIobjects.Add(new UIBacking(new Rectangle(550, 500, 250, 75), Color.White));
            UIobjects.Add(new Logo(new Rectangle(50, 50, 150, 50), Color.White));

            textList = new List<UIText>();
            addText("", new Vector2(27, 502), Color.Black, 0.8f, false);
            addText("", new Vector2(25, 500), Color.Red, 0.8f, false);
            addText("", new Vector2(27, 522), Color.Black, 0.8f, false);
            addText("", new Vector2(25, 520), Color.Red, 0.8f, false);
            addText("", new Vector2(27, 542), Color.Black, 0.8f, false);
            addText("", new Vector2(25, 540), Color.Red, 0.8f, false);
            addText("", new Vector2(577, 502), Color.Black, 0.8f, false);
            addText("", new Vector2(575, 500), Color.Blue, 0.8f, false);
            addText("", new Vector2(577, 522), Color.Black, 0.8f, false);
            addText("", new Vector2(575, 520), Color.Blue, 0.8f, false);
            addText("", new Vector2(577, 542), Color.Black, 0.8f, false);
            addText("", new Vector2(575, 540), Color.Blue, 0.8f, false);

            sb = new StringBuilder();

            debugList = new List<UIText>();
        }

        public void addText(string text, Vector2 position, Color color, float scale, bool isDebug)
        {
            UIText uit = new UIText();
            uit.text = text;
            uit.position = position;
            uit.color = color;
            uit.scale = scale;
            uit.isDebug = isDebug;
            if (isDebug)
            {
                debugList.Add(uit);
            }
            else
            {
                textList.Add(uit);
            }
        }

        public override void Update(GameTime gameTime)
        {
            BiplaneGame game = ((BiplaneGame)Game);

            if (((Aircraft)game.gameState.player1).damage != p1healthval)
            {
                p1healthval = ((Aircraft)game.gameState.player1).damage;
                p1health = sb.Append(health).Append(p1healthval).Append(percent).ToString();
                textList[0].text = p1health;
                textList[1].text = p1health;
                sb.Remove(0, sb.Length);
            }
            if (game.gameState.player1score != p1scoreval)
            {
                p1scoreval = game.gameState.player1score;
                p1score = sb.Append(score).Append(p1scoreval * 347).ToString();
                textList[2].text = p1score;
                textList[3].text = p1score;
                sb.Remove(0, sb.Length);
            }
            if (((Aircraft)game.gameState.player1).missleCount != p1misslesval || ((Aircraft)game.gameState.player1).bombCount != p1bombsval)
            {
                p1misslesval = ((Aircraft)game.gameState.player1).missleCount;
                p1bombsval = ((Aircraft)game.gameState.player1).bombCount;
                p1weapons = sb.Append(missles).Append(p1misslesval).Append(space).Append(bombs).Append(p1bombsval).ToString();
                textList[4].text = p1weapons;
                textList[5].text = p1weapons;
                sb.Remove(0, sb.Length);
            }
            if (((Aircraft)game.gameState.player2).damage != p2healthval)
            {
                p2healthval = ((Aircraft)game.gameState.player2).damage;
                p2health = sb.Append(health).Append(p2healthval).Append(percent).ToString();
                textList[6].text = p2health;
                textList[7].text = p2health;
                sb.Remove(0, sb.Length);
            }
            if (game.gameState.player2score != p2scoreval)
            {
                p2scoreval = game.gameState.player2score;
                p2score = sb.Append(score).Append(p2scoreval * 347).ToString();
                textList[8].text = p2score;
                textList[9].text = p2score;
                sb.Remove(0, sb.Length);
            }
            if (((Aircraft)game.gameState.player2).missleCount != p2misslesval || ((Aircraft)game.gameState.player2).bombCount != p2bombsval)
            {
                p2misslesval = ((Aircraft)game.gameState.player2).missleCount;
                p2bombsval = ((Aircraft)game.gameState.player2).bombCount;
                p2weapons = sb.Append(missles).Append(p2misslesval).Append(space).Append(bombs).Append(p2bombsval).ToString();
                textList[10].text = p2weapons;
                textList[11].text = p2weapons;
                sb.Remove(0, sb.Length);
            }


            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
                if (guiFont == null || debugFont == null)
                {
                    guiFont = Game.Content.Load<SpriteFont>("LCD");
                    debugFont = Game.Content.Load<SpriteFont>("LCD");
                }

                List<GameObject> objects = ((BiplaneGame)Game).gameState.objects;

                spriteBatch = ((BiplaneGame)Game).spriteBatch;

                spriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Deferred, SaveStateMode.None);

                // draw logo and other static UI elements
                foreach (GameObject gameObject in UIobjects)
                {
                    if (((GameObject2D)gameObject).texture == null)
                    {
                        ((GameObject2D)gameObject).texture = Game.Content.Load<Texture2D>(((GameObject2D)gameObject).textureName);
                    }
                    ((GameObject2D)gameObject).draw(spriteBatch);
                }

                int debugCount = 0;

                foreach (UIText uit in textList)
                {
                    //Vector2 FontOrigin = guiFont.MeasureString(uit.text) / 2;
                    spriteBatch.DrawString(guiFont, uit.text, uit.position, uit.color);//, 0, FontOrigin, uit.scale, SpriteEffects.None, 0.5f);
                }
                /* foreach (UIText uit in debugList)
                {
                    //Vector2 FontOrigin = debugFont.MeasureString(uit.text) / 2;
                    spriteBatch.DrawString(debugFont, uit.text, new Vector2(600, 40 - 20 * debugCount), uit.color);//, 0, FontOrigin, uit.scale, SpriteEffects.None, 0.5f);
                    debugCount++;
                } */
                spriteBatch.End();

                //debugList.Clear();

            base.Draw(gameTime);
        }
    }
}
