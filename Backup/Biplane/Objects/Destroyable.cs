using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Biplane.Objects
{
    interface Destroyable
    {
        void destroyObject(Vector3 screenVector, GameTime gameTime, List<GameObject> objects);

        void checkCollisions(List<GameObject> colliders, Vector3 screenVector, GameTime gameTime, List<GameObject> objects);
    }
}
