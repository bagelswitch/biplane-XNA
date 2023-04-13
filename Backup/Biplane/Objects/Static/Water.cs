using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace Biplane.Objects.Static
{
    class Water : GameObject3DModel
    {
        private static Model model;

        public static float waterLevel = -130500.0f;

        public Water(Game game) : base (game)
        {
            modelName = "Models\\waves";
            
            this.useFXEffects = true;

            this.position = new Vector3(0.0f, waterLevel, 0.0f);

            this.scale = 15.0f;

            float pitch = 0.0f;
            float yaw = 45.0f;
            float roll = 0.0f;

            this.modelOrientation = Quaternion.CreateFromAxisAngle(Vector3.Right, MathHelper.ToRadians(pitch)) * Quaternion.CreateFromAxisAngle(Vector3.Up, MathHelper.ToRadians(yaw)) * Quaternion.CreateFromAxisAngle(Vector3.Backward, MathHelper.ToRadians(roll));
        }

        public override void  update(GameTime gameTime, Vector3 screenVector)
        {
 	        base.update(gameTime, screenVector);
        }
    }
}
