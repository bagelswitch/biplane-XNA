using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Heightmap;

namespace Biplane.Objects.Static
{
    class Terrain : GameObject3DModel
    {
        private static HeightMapInfo heightMapInfo;

        public Terrain(Game game) : base (game)
        {
            modelName = "Heightmap\\terrain1";

            this.position = new Vector3(0.0f, 0.0f, 0.0f);
        }

        public static float GetHeight(Vector3 position)
        {
            float ground = 0.0f;

            if (heightMapInfo != null)
            {
                if (heightMapInfo.IsOnHeightmap(position))
                {
                    ground = heightMapInfo.GetHeight(position);
                }
            }

            if (ground < Water.waterLevel) ground = Water.waterLevel;

            return ground;
        }

        public void unloadModel()
        {
            model = null;
        }

        public override void loadModel()
        {
            if (this.model == null)
            {
                this.model = game.Content.Load<Model>(this.modelName);

                // The terrain processor attached a HeightMapInfo to the terrain model's
                // Tag. We'll save that to a member variable now, and use it to
                // calculate the terrain's heights later.
                heightMapInfo = model.Tag as HeightMapInfo;
                if (heightMapInfo == null)
                {
                    string message = "The terrain model did not have a HeightMapInfo " +
                        "object attached. Are you sure you are using the " +
                        "TerrainProcessor?";
                    throw new InvalidOperationException(message);
                }
            }
        }
    }
}
