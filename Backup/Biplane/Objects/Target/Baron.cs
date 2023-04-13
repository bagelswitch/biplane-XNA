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
    class Baron : Flyer
    {
        private static Random rotRand = new Random();

        private static Dictionary<string, Model> models = new Dictionary<string, Model>();

        public Baron(Game game) : base(game)
        {
            this.debris.Add("debris31");
            this.debris.Add("debris32");
            this.debris.Add("debris33");
            this.debris.Add("debris34");
            this.debris.Add("debris35");
            this.debris.Add("debris36");

            modelName = "Models\\baron";
        }

        public void setup(GameTime gameTime)
        {
            base.setup();

            this.maxAge = 500000000;

            this.target = target;

            this.size = 1250;

            this.power = 600.0f;

            this.damage = 100.0f;

            this.fireRate = 2000000;
            this.bombRate = 40000000;
            this.missleRate = 60000000;

            this.bombCount = 5;
            this.missleCount = 0;

            this.range = 10000.0f;
            this.minDis = 5000.0f;

            modelName = "Models\\baron";

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

            this.name = "baron";

            this.createTime = gameTime.TotalRealTime.Ticks;

            this.destroyed = false;

            // randomize the flight envelope a bit
            int chanceRot = rotRand.Next(100) + 10;
            this.minDis = (minDis * (float)chanceRot / 50.0f);
            this.pitchRate = this.pitchRate * ((rotRand.Next(20) + 80.0f) / 100.0f);
            this.power = this.power * ((rotRand.Next(20) + 80.0f) / 100.0f);

            if (minDis > (range * 0.75f)) minDis = range * 0.75f;

            float pitch = 0.0f;
            float yaw = -90.0f;
            float roll = 0.0f;

            this.modelOrientation = Quaternion.CreateFromAxisAngle(Vector3.Right, MathHelper.ToRadians(pitch)) * Quaternion.CreateFromAxisAngle(Vector3.Up, MathHelper.ToRadians(yaw)) * Quaternion.CreateFromAxisAngle(Vector3.Backward, MathHelper.ToRadians(roll));

        }

        public override void loadModel()
        {
            if (!models.ContainsKey(this.modelName))
            {
                Model newmodel = game.Content.Load<Model>(modelName);
                models.Add(this.modelName, newmodel);
            }
            this.model = models[this.modelName];
        }

        public override void generateObjects(GameTime gameTime, Vector3 screenVector, List<GameObject> objects)
        {
            if (this.target != null && !(this.target is Supplies))
            {
                // how far away is the Baron's target?
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

                // he's on your 12, fox 1
                if (Math.Abs(angleFromTarget) <= 0.1f && distance < (range * 2))
                {
                    this.firingMissles = true;
                }

                // directly below you! bombs away!
                if (angleFromTarget >= 0.7f && distance < (range * 3))
                {
                    this.firingBombs = true;
                }
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

            if ((Math.Abs(this.ground - this.position.Y) < 15000))
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
                bool samedir = Math.Abs(MathHelper.ToDegrees((float) Math.Acos(Vector3.Dot(this.target.velocity, this.currentDirection)))) < 45;
                if (((distance < minDis && !samedir) || (distance < minDis / 2)) && !(this.target is Supplies))
                {
                    turnAwayFromTarget(gameTime);
                    this.target = null;
                }
                else
                {
                    if (this.target is Flyer || this.bombCount < 1)
                    {
                        turnTowardsTarget(gameTime);
                    }
                    else
                    {
                        turnAboveTarget(gameTime);
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
