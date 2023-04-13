using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Biplane.Objects.UI 
{
    class UIBacking : GameObject2D
    {
        public UIBacking(Rectangle target, Color tint)
        {
            this.textureName = "Textures\\uibacking";

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
