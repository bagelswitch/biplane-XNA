using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Biplane.Objects;

namespace Biplane.Service
{
    interface CameraProvider
    {
        void AddCamera(Vector3 position, Boolean force);
    }
}
