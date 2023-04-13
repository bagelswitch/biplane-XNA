using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Biplane.Objects
{
    abstract class GameObject2D : GameObject
    {
        public Texture2D texture;
        public String textureName;

        protected Rectangle target;
        protected Color tint;

        public void draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(this.texture, this.target, this.tint);
        }

        public override void update(GameTime gameTime, Vector3 screenVector)
        {
            base.update(gameTime, screenVector);
        }
    }
}
