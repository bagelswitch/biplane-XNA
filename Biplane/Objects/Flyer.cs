using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using Biplane;
using Biplane.Service;
using Biplane.Objects.Target;
using Biplane.Objects.Weapon;
using Biplane.Objects.Player;
using Biplane.Objects.Static;

namespace Biplane.Objects
{
    public abstract class Flyer : Targeter
    {
        Matrix matRotZ30 = Matrix.CreateRotationZ(MathHelper.ToRadians(-45.0f));

        protected static Random rotRandom = new Random();

        // default values here, specific subclasses should set these in their constructors

        // gravity constant
        protected float gravity = 150.0f;
        // lift coefficient
        protected float lift = 0.75f;
        // drag coefficient
        protected float drag = 0.25f;

        Vector3 liftVector = new Vector3(0.0f, 0.0f, 0.0f);
        Vector3 dragVector = new Vector3(0.0f, 0.0f, 0.0f);
        Vector3 gravityVector = new Vector3(0.0f, 0.0f, 0.0f);

        protected long fireRate =    1000000;
        protected long bombRate =   20000000;
        protected long missleRate = 30000000;
        protected long suppliesRate = 50000000;

        protected long lastFire = 0;
        protected long lastBomb = 0;
        protected long lastMissle = 0;
        protected long lastSupplies = 0;

        protected float range = 15000.0f;
        protected float minDis = 5000.0f;

        protected bool controlsReversed = false;

        public Flyer(Game game)
            : base(game)
        {
            this.hasShadow = true;
        }

        public void setup()
        {
            base.setup();
        }

        public override void update(GameTime gameTime, Vector3 screenVector)
        {
            base.update(gameTime, screenVector);

            float seconds = gameTime.ElapsedGameTime.Milliseconds / 1000.0f;

            // calculate lift
            this.liftVector = new Vector3(this.upDirection.X, this.upDirection.Y, this.upDirection.Z) * (Math.Abs(this.velocity.X) + Math.Abs(this.velocity.Z)) * lift * seconds;

            // calculate drag
            this.dragVector = new Vector3(this.velocity.X, this.velocity.Y, this.velocity.Z) * -1 * drag * seconds;

            // calculate gravity
            gravityVector = new Vector3(0.0f, -1.0f, 0.0f) * gravity * seconds;

            this.velocity = this.powerVector + this.liftVector + this.dragVector + this.gravityVector;

            this.position += this.velocity;

            this.ground = Terrain.GetHeight(this.position);

            if (this.position.Y <= ground || Math.Abs(this.position.X) > 500000.0f)
            {
                this.velocity.Y = 0.0f;
                this.destroyed = true;
            }
        }

        public override void generateObjects(GameTime gameTime, Vector3 screenVector, List<GameObject> weapons)
        {
            if (this.firingGuns && (gameTime.TotalRealTime.Ticks - this.lastFire) > this.fireRate)
            {
                this.lastFire = gameTime.TotalRealTime.Ticks;
                weapons.Add(newBullet(this.position + (currentDirection * this.size * 1.5f), (this.velocity) + (currentDirection * 500.0f), gameTime.TotalRealTime.Ticks, this, game));
            }
            if (this.firingBombs && this.bombCount > 0 && (gameTime.TotalRealTime.Ticks - this.lastBomb) > this.bombRate)
            {
                this.lastBomb = gameTime.TotalRealTime.Ticks;
                weapons.Add(newBomb(this.position + (downDirection * this.size * 1.5f), (this.velocity), this.modelOrientation, gameTime.TotalRealTime.Ticks, this, game));
                this.firingBombs = false;
                this.bombCount--;
            }
            if (this.firingMissles && this.missleCount > 0 && (gameTime.TotalRealTime.Ticks - this.lastMissle) > this.missleRate)
            {
                this.lastMissle = gameTime.TotalRealTime.Ticks;
                weapons.Add(newMissle(this.position + (downDirection * this.size * 1.5f), (this.velocity) + (currentDirection * 150.0f), this.modelOrientation, gameTime.TotalRealTime.Ticks, this, game));
                this.firingMissles = false;
                this.missleCount--;
            }
            if (this.droppingSupplies && (gameTime.TotalRealTime.Ticks - this.lastSupplies) > this.suppliesRate)
            {
                this.lastSupplies = gameTime.TotalRealTime.Ticks;
                Supplies supplies = newSupplies(this.position + (currentDirection * this.size * -1.5f), (this.velocity), this.modelOrientation, gameTime.TotalRealTime.Ticks, this, game);
                weapons.Add(supplies);
                this.droppingSupplies = false;
            }

            if (this.damage <= 50 && !(this is Debris))
            {
                ((BiplaneGame)game).projectileTrailParticles.AddParticle(this.position, Vector3.Zero);
            }
            else if (this.damage <= 30 && !(this is Debris))
            {
                ((BiplaneGame)game).smokePlumeParticles.AddParticle(this.position, Vector3.Zero);
            }
        }

        public override void checkTargeters(List<GameObject> targeters)
        {
            foreach (GameObject targeter in targeters)
            {
                if (targeter != null && targeter is Targeter && !(targeter.team.Equals(this.team)) && !(this is Debris) && !(this is Bomb) && !(this is Missle) && !(this is Bullet) && !this.Equals(targeter) && !this.Equals(targeter.parent))
                {
                    if (((Targeter)targeter).target == null || ((Targeter)targeter).target.destroyed)
                    {
                        ((Targeter)targeter).target = this;
                    } 
                    else
                    {
                        if (Vector3.Distance(targeter.position, ((Targeter)targeter).target.position) > Vector3.Distance(targeter.position, this.position))
                        {
                            if (!(((Targeter)targeter).target is Sitter && ((Targeter)targeter).bombCount > 0) || this is Aircraft || this is Supplies)// || this.target == targeter)
                            {
                                ((Targeter)targeter).target = this;
                            }
                        }
                    }
                }
            }
        }

        public override void playSounds(SoundBank soundBank)
        {
            if (destroyed)
            {
                soundBank.PlayCue("targexpl");
            }
        }

        public void turnTowardsHome(GameTime gameTime)
        {
            float seconds = gameTime.ElapsedGameTime.Milliseconds / 1000.0f;

            LookAtYawToRoll(this.currentDirection + new Vector3(this.position.X, this.position.Y + 100000.0f, this.position.Z), this.pitchRate * seconds * this.velocity.Length() / 25.0f);
            keepUpright();

            this.deltaPow = this.power * seconds;
        }

        public void turnAwayFromHome(GameTime gameTime)
        {
            float seconds = gameTime.ElapsedGameTime.Milliseconds / 1000.0f;

            LookAtYawToRoll(this.currentDirection + new Vector3(this.position.X, this.ground, this.position.Z), this.pitchRate * seconds * this.velocity.Length() / 25.0f);
            keepUpright();

            this.deltaPow = this.power * seconds;
        }

        public void turnAboveTarget(GameTime gameTime)
        {
            float seconds = gameTime.ElapsedGameTime.Milliseconds / 1000.0f;

            Vector3 ip = new Vector3(this.target.position.X, this.target.position.Y + 20000.0f, this.target.position.Z);
            LookAtYawToRoll(ip, this.pitchRate * seconds * this.velocity.Length() / 50.0f);
            keepUpright();

            this.deltaPow = this.power * seconds;
        }

        public void turnTowardsTarget(GameTime gameTime)
        {
            float seconds = gameTime.ElapsedGameTime.Milliseconds / 1000.0f;

            LookAtYawToRoll(this.target.position, this.pitchRate * seconds * this.velocity.Length() / 50.0f);
            keepUpright();

            this.deltaPow = this.power * seconds;
        }

        public void turnAwayFromTarget(GameTime gameTime)
        {
            float seconds = gameTime.ElapsedGameTime.Milliseconds / 1000.0f;

            LookAtYawToRoll(this.position + (this.position - this.target.position), this.pitchRate * seconds * this.velocity.Length() / 50.0f);
            keepUpright();

            this.deltaPow = this.power * seconds;
        }

        public void turnInCircles(GameTime gameTime)
        {
            float seconds = gameTime.ElapsedGameTime.Milliseconds / 1000.0f;

            RollRight(this.rollRate * seconds * this.velocity.Length() / 50.0f);
            PitchUp(this.pitchRate * seconds * this.velocity.Length() / 50.0f);
            keepUpright();

            this.deltaPow = this.power * seconds;
        }

        public void keepUpright()
        {
            if (Math.Abs(this.currentDirection.Y) > 0.5f)
            {
                ConstrainRoll(Math.Abs((1.0f - this.currentDirection.Y) / 1000.0f));
            }
            if (Math.Abs(this.currentDirection.Z) > 0.5f)
            {
                ConstrainYaw(Math.Abs(this.currentDirection.Z / 100.0f));
            }
        }
    }
}
