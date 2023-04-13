using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Biplane.Objects.Weapon;
using Biplane.Service;

namespace Biplane.Objects.Weapon
{
    public class Supplies : Flyer
    {
        private static Random rotRand = new Random();

        private static Model model;

        public Supplies(Game game) : base(game)
        {
            modelName = "Models\\parachute";
            scale = 2.5f;
            useXEffects = true;
        }

        public void setup(Vector3 position, Vector3 velocity, Quaternion orientation, long createTime, GameObject parent)
        {
            base.setup();

            this.parent = parent;

            this.size = 3000;

            this.range = 0.0f;

            this.power = 0.0f;

            this.ceiling = 500000.0f;

            this.lift = 0.00f;

            this.drag = 2.00f;

            this.damage = 500.0f;

            this.maxAge = 100000000;
            this.createTime = createTime;

            this.pitchRate = 0.10f;
            this.yawRate = 0.10f;
            this.rollRate = 0.10f;

            this.minDis = 0;

            this.playerIndex = 0;
            this.useGamePad = false;
            this.useKeyboard = false;
            this.position = position;
            this.velocity = velocity;

            float pitch = 0.0f;
            float yaw = 0.0f;
            float roll = 90.0f;

            this.modelOrientation = Quaternion.CreateFromAxisAngle(Vector3.Backward, MathHelper.ToRadians(pitch)) * Quaternion.CreateFromAxisAngle(Vector3.Up, MathHelper.ToRadians(yaw)) * Quaternion.CreateFromAxisAngle(Vector3.Right, MathHelper.ToRadians(roll));

        }

        public override void generateObjects(GameTime gameTime, Vector3 screenVector, List<GameObject> objects)
        {
            base.generateObjects(gameTime, screenVector, objects);
        }

        public override void update(GameTime gameTime, Vector3 screenVector)
        {
            base.update(gameTime, screenVector);

            float seconds = gameTime.TotalRealTime.Seconds;

            this.deltaRoll = (float) Math.Abs(Math.Sin(seconds)) / 1000.0f;
            this.deltaYaw = (float)Math.Sign(Math.Sin(seconds)) / 1000.0f;
            keepUpright();
        }
    }
}
