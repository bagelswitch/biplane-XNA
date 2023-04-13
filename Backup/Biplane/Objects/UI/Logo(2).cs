using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Biplane.Objects.UI 
{
    class Logo : GameObject2D
    {
        public Logo(Rectangle target, Color tint)
        {
            this.textureName = "Sprites\\logo";

            this.texture = texture;
            this.target = target;
            this.tint = tint;
        }

        public override void update(GameTime gameTime, Vector3 screenVector)
        {
            // do nothing
        }
    }
}
