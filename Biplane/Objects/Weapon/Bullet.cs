using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;

namespace Biplane.Objects.Weapon
{
    public class Bullet : Flyer
    {
        private static Model model;

        public Bullet(Game game) : base(game)
        {
            this.hasShadow = false;

            modelName = "Models\\pea_proj";
        }

        public void setup(Vector3 position, Vector3 velocity, long createTime, GameObject parent)
        {
            base.setup();

            this.parent = parent;

            power = 0.0f;
            ceiling = 500000;

            this.size = 100;

            this.lift = 0.15f;

            this.drag = 0.25f;

            this.maxAge = 8000000;
            this.createTime = createTime;

            modelName = "Models\\pea_proj";

            this.playerIndex = 0;
            this.useGamePad = false;
            this.useKeyboard = false;
            this.position = position;
            this.velocity = velocity;

            float pitch = 0.0f;
            float yaw = 0.0f;
            float roll = 0.0f;

            this.modelOrientation = Quaternion.CreateFromAxisAngle(Vector3.Backward, MathHelper.ToRadians(pitch)) * Quaternion.CreateFromAxisAngle(Vector3.Up, MathHelper.ToRadians(yaw)) * Quaternion.CreateFromAxisAngle(Vector3.Right, MathHelper.ToRadians(roll));

        }

        public override void generateObjects(GameTime gameTime, Vector3 screenVector, List<GameObject> objects)
        {
            // do nothing
        }

        public override void playSounds(SoundBank soundBank)
        {
            // do nothing
        }

        public override void update(GameTime gameTime, Vector3 screenVector)
        {
            ((BiplaneGame)game).tracerParticles.AddParticle(this.position, this.velocity);
            base.update(gameTime, screenVector);
        }

    }
}
