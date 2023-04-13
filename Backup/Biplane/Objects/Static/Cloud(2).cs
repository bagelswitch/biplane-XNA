using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Biplane.Objects.Static 
{
    class Cloud : GameObject2D
    {
        private static Random cloudRand = new Random();

        public Cloud(Color tint)
        {
            this.textureName = "Sprites\\cloud";

            this.texture = texture;

            int X = cloudRand.Next(400000) - 200000;
            int Y = cloudRand.Next(80000) + 20000;

            this.position = new Vector3((float) X, (float) Y, -900.0f);

            this.target = new Rectangle(0, 0, 256, 256);

            this.tint = tint;
        }

        public override void update(GameTime gameTime, Vector3 screenVector)
        {
            this.target.X = (int) screenVector.X;
            this.target.Y = (int) screenVector.Y;
        }
    }
}