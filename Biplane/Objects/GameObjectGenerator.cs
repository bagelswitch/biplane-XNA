using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using Microsoft.Xna.Framework;

namespace Biplane.Objects
{
    interface GameObjectGenerator
    {
        void generateObjects(GameTime gameTime, Vector3 screenVector, List<GameObject> objects);
    }
}
