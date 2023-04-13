using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Biplane.Service
{
    interface UITextProvider
    {
        void addText(String text, Vector2 position, Color color, float scale, bool isDebug);
    }
}
