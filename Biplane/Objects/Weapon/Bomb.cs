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
    public class Bomb : Flyer
    {
        private static Model model;

        public Bomb(Game game) : base(game)
        {
            this.debris.Add("pea_proj");
            this.debris.Add("pea_proj");
            this.debris.Add("pea_proj");
            this.debris.Add("pea_proj");
            this.debris.Add("pea_proj");
            this.debris.Add("pea_proj");
            this.debris.Add("pea_proj");
            this.debris.Add("pea_proj");
            this.debris.Add("pea_proj");
            this.debris.Add("pea_proj");
            this.debris.Add("pea_proj");
            this.debris.Add("pea_proj");

            modelName = "Models\\bomb";
        }

        public void setup(Vector3 position, Vector3 velocity, Quaternion orientation, long createTime, GameObject parent)
        {
            base.setup();

            this.parent = parent;

            this.size = 500;

            this.damage = 10.0f;

            this.lift = 0.15f;

            this.drag = 0.25f;

            this.maxAge = 50000000;
            this.createTime = createTime;

            modelName = "Models\\bomb";

            this.playerIndex = 0;
            this.useGamePad = false;
            this.useKeyboard = false;
            this.position = position;
            this.velocity = velocity;

            this.modelOrientation = orientation;
        }

        public override void generateObjects(GameTime gameTime, Vector3 screenVector, List<GameObject> objects)
        {
            // do nothing
        }

        public override void update(GameTime gameTime, Vector3 screenVector)
        {
            base.update(gameTime, screenVector);

            float seconds = gameTime.ElapsedGameTime.Milliseconds / 1000.0f;

            LookAt((Vector3.Down * 1000000.0f) - this.position, 0.01f);
        }

        public override void playSounds(SoundBank soundBank)
        {
            if (destroyed)
            {
                soundBank.PlayCue("bombexpl");
            }
        }
    }
}
