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
using Biplane.ParticleSystem;

namespace Biplane.Objects.Player
{
    class Player1 : Aircraft
    {
        private static Model model;

        public Player1(bool gamepad, bool keyboard, Game game)
            : base(gamepad, keyboard, game)
        {
            this.modelName = "Models\\sopwith";

            this.useXEffects = true;
            this.scale = 3.0f;

            this.debris.Add("debris11");
            this.debris.Add("debris12");
            this.debris.Add("debris13");
            this.debris.Add("debris14");
            this.debris.Add("debris15");
            this.debris.Add("debris16");

            this.name = "player1";

            this.playerIndex = 1;
        }

        public void setup(Vector3 position, Vector3 velocity, GameTime gameTime)
        {
            base.setup();

            this.size = 1250;

            this.position = position;
            this.velocity = new Vector3(0.001f, -0.001f, 0.000f);

            this.fireRate = 1000000;
            this.bombRate = 10000000;
            this.missleRate = 20000000;
            this.lastFire = 0;
            this.lastBomb = 0;
            this.lastMissle = 0;

            this.bombCount = 5;
            this.missleCount = 0;

            this.deltaPow = 0.0f;
            this.deltaYaw = 0.0f;
            this.deltaPitch = 0.0f;
            this.deltaRoll = 0.0f;

            this.power = 600;
            this.ceiling = 200000;
            this.minDis = 10000.0f;

            this.team = "red";
            this.color = Color.Red.PackedValue;

            this.destroyed = false;
            this.damage = 100.0f;
            if (gameTime != null)
            {
                this.createTime = gameTime.TotalRealTime.Ticks;
            }
            else
            {
                this.createTime = 0;
            }
            this.score = 0;

            int chanceRot = rotRandom.Next(100) + 10;
            this.minDis = (minDis * (float)chanceRot / 50.0f);

            if (minDis > (range * 0.75f)) minDis = range * 0.75f;

            float pitch = 0.0f;
            float yaw = -90.0f;
            float roll = 0.0f;

            this.modelOrientation = Quaternion.CreateFromAxisAngle(Vector3.Right, MathHelper.ToRadians(pitch)) * Quaternion.CreateFromAxisAngle(Vector3.Up, MathHelper.ToRadians(yaw)) * Quaternion.CreateFromAxisAngle(Vector3.Backward, MathHelper.ToRadians(roll));
        }
    }
}
