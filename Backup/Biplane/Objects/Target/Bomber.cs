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

namespace Biplane.Objects.Target
{
    class Bomber : Flyer
    {
        private static Random rotRand = new Random();

        private static Model model;

        public Bomber(Game game) : base(game)
        {
            this.debris.Add("debris11");
            this.debris.Add("debris11");
            this.debris.Add("debris12");
            this.debris.Add("debris12");
            this.debris.Add("debris13");
            this.debris.Add("debris14");
            this.debris.Add("debris15");
            this.debris.Add("debris16");

            modelName = "Models\\bomber";
        }

        public void setup(GameTime gameTime)
        {
            base.setup();

            this.maxAge = 500000000;

            this.target = null;

            this.size = 2000;

            this.damage = 150.0f;

            this.power = 400.0f;

            this.ceiling = 350000;

            this.pitchRate = 0.15f;

            this.fireRate = 2000000;
            this.bombRate = 30000000;

            this.bombCount = 100;
            this.missleCount = 0;

            this.range = 10000.0f;
            this.minDis = 5000.0f;

            modelName = "Models\\bomber";

            this.name = "bomber";

            this.useXEffects = true;
            this.scale = 3.0f;

            this.playerIndex = 0;
            this.useGamePad = false;
            this.useKeyboard = false;

            int randCenter = (rotRand.Next(20) - 10);
            if (randCenter == 0) randCenter = 1;
            int center = randCenter * 20000;
            int randZCenter = (rotRand.Next(40000) - 20000);
            this.position = new Vector3((float)center, (rotRand.Next(3) - 3) * 10000.0f, (float)randZCenter);

            this.velocity = new Vector3(0.001f, 0.001f, 0.000f);

            this.createTime = gameTime.TotalRealTime.Ticks;

            this.destroyed = false;

            this.name = "bomber";

            // randomize the flight envelope a bit
            int chanceRot = rotRand.Next(100) + 20;
            this.minDis = (minDis * (float)chanceRot / 50.0f);
            this.pitchRate = this.pitchRate * ((rotRand.Next(20) + 80.0f) / 100.0f);
            this.power = this.power * ((rotRand.Next(20) + 80.0f) / 100.0f);

            float pitch = 0.0f;
            float yaw = -90.0f;
            float roll = 0.0f;

            this.modelOrientation = Quaternion.CreateFromAxisAngle(Vector3.Right, MathHelper.ToRadians(pitch)) * Quaternion.CreateFromAxisAngle(Vector3.Up, MathHelper.ToRadians(yaw)) * Quaternion.CreateFromAxisAngle(Vector3.Backward, MathHelper.ToRadians(roll));

        }

        public override void generateObjects(GameTime gameTime, Vector3 screenVector, List<GameObject> objects)
        {
            if (this.target != null)
            {
                // how far away is the target?
                float distance = Vector3.Distance(this.target.position, this.position);

                // OK, so this doesn't really give the angle, but we can tell above/below/ahead from the Vector cross-product
                Vector3 direction = (this.target.position - this.position);
                direction.Normalize();
                float angleFromTarget = Vector3.Cross(direction, currentDirection).Z;

                // too close for missles, going winchester
                if (Math.Abs(angleFromTarget) <= 0.3f && distance < range)
                {
                    this.firingGuns = true;
                }
                else
                {
                    this.firingGuns = false;
                }

                // directly below you! bombs away!
                if (angleFromTarget >= 0.7f && distance < (range * 3))
                {
                    this.firingBombs = true;
                }
            }

            if (this.position.Y > -100000.0f)
            {
                this.droppingSupplies = true;
            }

            base.generateObjects(gameTime, screenVector, objects);
        }

        public override void update(GameTime gameTime, Vector3 screenVector)
        {
            base.update(gameTime, screenVector);

            this.deltaPitch = 0.0f;
            this.deltaYaw = 0.0f;
            this.deltaRoll = 0.0f;
            this.deltaPow = 0.0f;

            if ((Math.Abs(this.ground - this.position.Y) < 20000))
            {
                turnTowardsHome(gameTime);
            }
            else if (this.velocity.Length() < 100)
            {
                turnAwayFromHome(gameTime);
            }
            else if (this.target != null && !this.target.destroyed)
            {
                float distance = Vector3.Distance(this.target.position, this.position);
                if (distance < minDis)
                {
                    turnAwayFromTarget(gameTime);
                    this.target = null;
                }
                else
                {
                    if ((Math.Abs(this.ground - this.position.Y) < 100000) && this.bombCount > 0)
                    {
                        turnAboveTarget(gameTime);
                    }
                    else
                    {
                        turnTowardsTarget(gameTime);
                    }
                }
            }
            else
            {
                turnInCircles(gameTime);
            }
        }
    }
}
