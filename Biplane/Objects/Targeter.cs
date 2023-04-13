using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Input;
using Biplane.Objects.Target;
using Biplane.Objects.Weapon;
using Biplane.Objects.Static;
using Biplane.Objects.Player;
using Biplane.Service;

namespace Biplane.Objects
{
    public abstract class Targeter : GameObject3DModel, Destroyable, GameObjectGenerator, SoundGenerator
    {
        public GameObject target;

        public int playerIndex;
        protected bool useGamePad;
        protected bool useKeyboard;

        protected bool firingGuns;
        protected bool firingBombs;
        protected bool firingMissles;
        protected bool droppingSupplies;

        public int missleCount;
        public int bombCount;

        private static List<Bomb> deadBombs = new List<Bomb>();
        private static List<Bullet> deadBullets = new List<Bullet>();
        private static List<Missle> deadMissles = new List<Missle>();
        private static List<Debris> deadDebris = new List<Debris>();
        private static List<Supplies> deadSupplies = new List<Supplies>();

        private static float maxPitch = 10000.0f;
        private static float minPitch = -10000.0f;
        private static float maxRoll = 10000.0f;
        private static float minRoll = -10000.0f;
        private static float maxYaw = 10000.0f;
        private static float minYaw = -10000.0f;

        // max rotation rate in degrees/sec
        protected float yawRate = 0.15f;
        protected float pitchRate = 0.25f;
        protected float rollRate = 0.35f;

        protected float deltaYaw = 0.0f;
        protected float deltaPitch = 0.0f;
        protected float deltaRoll = 0.0f;
        protected float deltaPow = 0.0f;

        // starting damage (bullets take away 10.0)
        public float damage = 100.0f;

        private static Vector3 cross = Vector3.Zero;
        private Vector3 xzpos = new Vector3();
        private Vector3 targxzpos = new Vector3();
        private Vector3 yawDir = new Vector3();
        private Vector3 yawCDir = new Vector3();

        // max power (at min altitude)
        protected float power = 800.0f;
        // max altitude at which power is produced
        protected float ceiling = 200000;
        // min altitude
        protected float floor = 0;
        // current grount altitude
        protected float ground = 0.0f;

        public Vector3 powerVector = new Vector3(0.001f, 0.001f, 0.001f);
        public Vector3 inertiaVector  = new Vector3(0.0f, 0.0f, 0.0f);
        public Matrix yprt;

        public List<String> debris = new List<String>();
        protected static Random debrisRandom = new Random();

        public Targeter(Game game)
            : base(game)
        {
        }

        public void setup()
        {
            base.setup();
        }

        public abstract void checkTargeters(List<GameObject> targeters);

        public abstract void generateObjects(GameTime gameTime, Vector3 screenVector, List<GameObject> objects);

        public abstract void playSounds(SoundBank soundBank);

        public override void update(GameTime gameTime, Vector3 screenVector)
        {
            base.update(gameTime, screenVector);

            float seconds = gameTime.ElapsedGameTime.Milliseconds / 1000.0f;

            float oriMult = velocity.Length();
            if(oriMult > 1000) oriMult = 1000;
            if(oriMult < 300) oriMult = 300;

            deltaPitch *= oriMult;
            deltaRoll *= oriMult;
            deltaYaw *= oriMult;

            if (deltaPitch > maxPitch) deltaPitch = maxPitch;
            if (deltaPitch < minPitch) deltaPitch = minPitch;
            if (deltaRoll > maxRoll) deltaRoll = maxRoll;
            if (deltaRoll < minRoll) deltaRoll = minRoll;
            if (deltaYaw > maxYaw) deltaYaw = maxYaw;
            if (deltaYaw < minYaw) deltaYaw = minYaw;

            //Quaternion rot = Quaternion.CreateFromAxisAngle(Vector3.Backward, MathHelper.ToRadians(deltaPitch)) * Quaternion.CreateFromAxisAngle(Vector3.Up, MathHelper.ToRadians(deltaYaw)) * Quaternion.CreateFromAxisAngle(Vector3.Right, MathHelper.ToRadians(deltaRoll));
            Quaternion rot = Quaternion.CreateFromAxisAngle(Vector3.Right, MathHelper.ToRadians(deltaPitch)) * Quaternion.CreateFromAxisAngle(Vector3.Up, MathHelper.ToRadians(deltaYaw)) * Quaternion.CreateFromAxisAngle(Vector3.Backward, MathHelper.ToRadians(deltaRoll));
            
            this.modelOrientation *= rot;

            Matrix.CreateFromQuaternion(ref this.modelOrientation, out this.yprt);
            this.currentDirection = new Vector3(0.0f, 0.0f, -1.0f);
            Vector3.Transform(ref this.currentDirection, ref this.yprt, out this.currentDirection);
            currentDirection.Normalize();


            // calculate power
            if (this.deltaPow > 0.0f)
            {
                float enginePow = (this.ceiling - (this.position.Y - this.ground)) / (this.ceiling);
                if (enginePow < 0.0f)
                {
                    enginePow = 0.0f;
                }
                this.powerVector = new Vector3(this.currentDirection.X, this.currentDirection.Y, this.currentDirection.Z) * enginePow * this.deltaPow;
                this.powerVector += (0.93f * this.velocity) + (0.05f * (this.currentDirection*this.velocity.Length()));
            }
            else
            {
                this.powerVector = (0.985f * this.velocity) + (0.01f * (this.currentDirection*this.velocity.Length()));
            }

            if (this is Flyer)
            {
                this.downDirection = new Vector3(0.0f, -1.0f, 0.0f);
                Vector3.Transform(ref this.downDirection, ref this.yprt, out this.downDirection);
                this.downDirection.Normalize();
                this.upDirection = -1.0f * new Vector3(this.downDirection.X, this.downDirection.Y, this.downDirection.Z);
            }
            else
            {
                this.upDirection = Vector3.Up;
                this.downDirection = Vector3.Down;
            }
        }

        public void checkCollisions(List<GameObject> colliders, Vector3 screenVector, GameTime gameTime, List<GameObject> objects)
        {
            float seconds = gameTime.ElapsedGameTime.Milliseconds / 1000.0f;

            foreach (GameObject collider in colliders)
            {
                if (!this.destroyed && !this.Equals(collider) && !collider.destroyed && !(this is Debris && collider is Debris) &&
                        !(this is Bullet) && !(this is Missle) && !(this is Bomb) && !(this is Supplies) &&
                        this.bb.Intersects(collider.bb))
                    {
                        if (this is Zeppelin)
                        {
                            FireAtLocation(this.position, this.velocity);
                        }
                        if (this is Flyer && !(collider.parent == this))
                        {
                            this.velocity += collider.velocity / 10.0f;
                        }
                        if (collider is Bullet && !(collider.parent == this))
                        {
                            this.damage -= 10.0f;
                            collider.score += 10;
                            if (this is Flyer)
                            {
                                this.deltaPitch = ((new Random().Next(4)) - 2) * pitchRate * seconds;
                                this.deltaRoll = ((new Random().Next(4)) - 2) * rollRate * seconds;
                            }
                            collider.destroyed = true;
                        }
                        else if (collider is Bomb)
                        {
                            this.damage -= 100.0f;
                            collider.score += 100;
                            collider.destroyed = true;
                        }
                        else if (collider is Missle && !(collider.parent == this))
                        {
                            this.damage -= 50.0f;
                            collider.score += 100;
                            collider.destroyed = true;
                        }
                        else if (collider is Debris)
                        {
                            this.damage -= 20.0f;
                            collider.score += 50;
                            if (this is Flyer)
                            {
                                this.deltaPitch = ((new Random().Next(4)) - 2) * pitchRate * seconds;
                                this.deltaRoll = ((new Random().Next(4)) - 2) * rollRate * seconds;
                            }
                            collider.destroyed = true;
                        }
                        else if (collider is Supplies && !(this is Bomber))
                        {
                            if (this.damage < 200.0f)
                            {
                                this.damage += 50.0f;
                            }
                            if (this.missleCount < 10)
                            {
                                this.missleCount += 5;
                            }
                            if (this.bombCount < 10)
                            {
                                this.bombCount += 5;
                            }
                            collider.destroyed = true;
                        }
                        else if (!(collider is Supplies) && !(collider is Debris) && !(collider is Missle) && !(collider is Bomb) && !(collider is Bullet) && (collider is Flyer || collider is Sitter))
                        {
                            this.damage -= 50.0f;
                            ((Targeter)collider).damage -= 50.0f;
                            collider.score += 100;
                            this.score += 100;
                        }
                    }
                }
            if (this.damage <= 0.0f)
            {
                this.destroyed = true;
                if (this is Zeppelin)
                {
                    FireAtLocation(this.position, this.velocity);
                }
            }
        }

        public void destroyObject(Vector3 screenVector, GameTime gameTime, List<GameObject> explosions)
        {
            foreach (String modelName in debris)
            {
                Vector3 randV = new Vector3(debrisRandom.Next(800) - 400, debrisRandom.Next(800) - 400, debrisRandom.Next(800) - 400);
                explosions.Add(newDebris(new Vector3(this.position.X, this.position.Y, this.position.Z), new Vector3(this.velocity.X, this.velocity.Y, this.velocity.Z) + randV, this.modelOrientation, gameTime.TotalRealTime.Ticks, modelName, this, game));
            }

            if (this.parent != null)
            {
                this.parent.score += this.score;
            }

            this.firingGuns = false;
        }

        public static void recycle(GameObject gameObject)
        {
            if (gameObject is Debris)
            {
                deadDebris.Add((Debris)gameObject);
            }
            if (gameObject is Bullet)
            {
                deadBullets.Add((Bullet)gameObject);
            }
            if (gameObject is Bomb)
            {
                deadBombs.Add((Bomb)gameObject);
            }
            if (gameObject is Missle)
            {
                deadMissles.Add((Missle)gameObject);
            }
            if (gameObject is Supplies)
            {
                deadSupplies.Add((Supplies)gameObject);
            }
            //gameObject.destroyed = false;
        }

        public void yawToFaceTarget(GameTime gameTime)
        {
            float seconds = gameTime.ElapsedGameTime.Milliseconds / 1000.0f;

            if (this.target != null && !this.target.destroyed)
            {
                xzpos.X = this.position.X;
                xzpos.Y = 0.0f;
                xzpos.Z = this.position.Z;
                targxzpos.X = this.target.position.X;
                targxzpos.Y = 0.0f;
                targxzpos.Z = this.target.position.Z;
                Vector3 direction = (targxzpos - xzpos);
                direction.Normalize();

                yawDir.X = direction.X;
                yawDir.Y = direction.Z;
                yawDir.Z = 0.0f;
                yawCDir.X = currentDirection.X;
                yawCDir.Y = currentDirection.Z;
                yawCDir.Z = 0.0f;

                float angle = angleBetweenVectors(yawCDir, yawDir);

                if (angle > 0.0f)
                {
                    this.deltaYaw = -this.yawRate * seconds;
                }
                else
                {
                    this.deltaYaw = this.yawRate * seconds;
                }

                if (Vector3.Distance(this.position, this.target.position) < 10000.0f)
                {
                    this.deltaYaw *= -1.5f;
                }
            }
        }

        public void yawTowardsHome(GameTime gameTime)
        {
            float seconds = gameTime.ElapsedGameTime.Milliseconds / 1000.0f;

            xzpos.X = this.position.X;
            xzpos.Y = 0.0f;
            xzpos.Z = this.position.Z;
            Vector3 direction = Vector3.Zero - xzpos;
            direction.Normalize();

            yawDir.X = direction.X; 
            yawDir.Y = direction.Z;
            yawDir.Z = 0.0f;
            yawCDir.X = currentDirection.X;
            yawCDir.Y = currentDirection.Z;
            yawCDir.Z = 0.0f;

            float angle = angleBetweenVectors(yawCDir, yawDir);

            if (angle > 0.0f)
            {
                this.deltaYaw = -this.yawRate * seconds;
            }
            else
            {
                this.deltaYaw = this.yawRate * seconds;
            }

        }

        public static float angleBetweenVectors(Vector3 vec1, Vector3 vec2)
        {
            Vector3.Cross(ref vec1, ref vec2, out cross);

            return cross.Z;
        }

        public static Bomb newBomb(Vector3 position, Vector3 velocity, Quaternion orientation, long createTime, GameObject parent, Game game)
        {
            if (deadBombs.Count > 0)
            {
                Bomb bomb = deadBombs[0];
                deadBombs.Remove(bomb);
                bomb.setup(position, velocity, orientation, createTime, parent);
                return bomb;
            }
            else
            {
                Bomb bomb = new Bomb(game);
                bomb.setup(position, velocity, orientation, createTime, parent);
                return bomb;
            }
            
        }

        public static Bullet newBullet(Vector3 position, Vector3 velocity, long createTime, GameObject parent, Game game)
        {
            if (deadBullets.Count > 0)
            {
                Bullet bullet = deadBullets[0];
                deadBullets.Remove(bullet);
                bullet.setup(position, velocity, createTime, parent);
                return bullet;
            }
            else
            {
                Bullet bullet = new Bullet(game);
                bullet.setup(position, velocity, createTime, parent);
                return bullet;
            }
            
        }

        public static Missle newMissle(Vector3 position, Vector3 velocity, Quaternion orientation, long createTime, GameObject parent, Game game)
        {
            if (deadMissles.Count > 0)
            {
                Missle missle = deadMissles[0];
                deadMissles.Remove(missle);
                missle.setup(position, velocity, orientation, createTime, parent);
                return missle;
            }
            else
            {
                Missle missle = new Missle(game);
                missle.setup(position, velocity, orientation, createTime, parent);
                return missle;
            }
        }

        public static Supplies newSupplies(Vector3 position, Vector3 velocity, Quaternion orientation, long createTime, GameObject parent, Game game)
        {
            if (deadSupplies.Count > 0)
            {
                Supplies supplies = deadSupplies[0];
                deadSupplies.Remove(supplies);
                supplies.setup(position, velocity, orientation, createTime, parent);
                return supplies;
            }
            else
            {
                Supplies supplies = new Supplies(game);
                supplies.setup(position, velocity, orientation, createTime, parent);
                return supplies;
            }
        }

        public static Debris newDebris(Vector3 position, Vector3 velocity, Quaternion orientation, long createTime, String modelName, GameObject parent, Game game)
        {
            if (deadDebris.Count > 0)
            {
                Debris debris = deadDebris[0];
                deadDebris.Remove(debris);
                debris.setup(position, velocity, orientation, createTime, modelName, parent);
                return debris;
            }
            else
            {
                Debris debris = new Debris(game);
                debris.setup(position, velocity, orientation, createTime, modelName, parent);
                return debris;
            }
            
        }
    }
}
