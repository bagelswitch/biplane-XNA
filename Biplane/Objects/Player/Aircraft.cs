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
using Biplane.Components;

namespace Biplane.Objects.Player
{
    abstract class Aircraft : Flyer
    {
        private Cue engineSound = null;
        private Cue gunsSound = null;

        protected uint color = 0xffffffff;

        public Aircraft(bool gamepad, bool keyboard, Game game) : base (game)
        {
            this.useGamePad = gamepad;
            this.useKeyboard = keyboard;

            this.useXEffects = true;
        }

        public bool acceptInput(GameTime gameTime, GamePadState gs, KeyboardState ks)
        {
            bool gotInput = false;

            float inputPow = 0.0f;
            float inputYaw = 0.0f;
            float inputPitch = 0.0f;
            float inputRoll = 0.0f;

            float seconds = gameTime.ElapsedGameTime.Milliseconds / 1000.0f;
            if (this.useKeyboard && this.playerIndex == 1)
            {
                if (ks.IsKeyDown(Keys.Up))
                {
                    gotInput = true;
                    inputPitch = this.pitchRate * seconds;
                }
                if (ks.IsKeyDown(Keys.Down))
                {
                    gotInput = true;
                    inputPitch = -this.pitchRate * seconds;
                }
                if (!ks.IsKeyDown(Keys.Up) && !ks.IsKeyDown(Keys.Down))
                {
                    inputPitch = 0.0f;
                }

                if (ks.IsKeyDown(Keys.Left))
                {
                    gotInput = true;
                    inputRoll = -this.rollRate * seconds;
                }
                if (ks.IsKeyDown(Keys.Right))
                {
                    gotInput = true;
                    inputRoll = this.rollRate * seconds;
                }
                if (!ks.IsKeyDown(Keys.Left) && !ks.IsKeyDown(Keys.Right))
                {
                    inputRoll = 0.0f;
                }

                if (ks.IsKeyDown(Keys.X))
                {
                    gotInput = true;
                    inputPow = this.power * seconds;
                }
                else
                {
                    inputPow = 0.0f;
                }
                if (ks.IsKeyDown(Keys.Space))
                {
                    gotInput = true;
                    this.firingGuns = true;
                }
                else
                {
                    this.firingGuns = false;
                }
                if (ks.IsKeyDown(Keys.B))
                {
                    gotInput = true;
                    this.firingBombs = true;
                }
                else
                {
                    this.firingBombs = false;
                }
                if (ks.IsKeyDown(Keys.V))
                {
                    gotInput = true;
                    this.firingMissles = true;
                }
                else
                {
                    this.firingMissles = false;
                }
            }
            if (this.useGamePad && gs.IsConnected && this.playerIndex != 0)
            {
                inputPitch = this.pitchRate * seconds * gs.ThumbSticks.Left.Y;
                inputRoll = -1 * this.rollRate * seconds * gs.ThumbSticks.Left.X;
                inputPow = this.power * seconds * gs.Triggers.Right;
                if (inputPitch != 0.0f || inputRoll != 0.0f || inputPow != 0.0f)
                {
                    gotInput = true;
                }
                if (gs.Buttons.A.Equals(ButtonState.Pressed))
                {
                    gotInput = true;
                    this.firingGuns = true;
                }
                else
                {
                    this.firingGuns = false;
                }
                if (gs.Buttons.B.Equals(ButtonState.Pressed))
                {
                    gotInput = true;
                    this.firingBombs = true;
                }
                else
                {
                    this.firingBombs = false;
                }
                if (gs.Buttons.X.Equals(ButtonState.Pressed))
                {
                    gotInput = true;
                    this.firingMissles = true;
                }
                else
                {
                    this.firingMissles = false;
                }
            }

            inputPitch *= -1;
            inputRoll *= -1;

            if ((((BiplaneGame)this.game).gameState.player1Active && this is Player1) || (((BiplaneGame)this.game).gameState.player2Active && this is Player2)) 
            {
                this.deltaPitch = inputPitch;
                this.deltaYaw = inputYaw;
                this.deltaRoll = inputRoll;
                this.deltaPow = inputPow;
            }

            return gotInput;
        }

        public override void playSounds(SoundBank soundBank) {
            if (this.deltaPow > 0.0f && !this.destroyed)
            {
                if (engineSound == null)
                {
                    engineSound = soundBank.GetCue("planengi");
                    if (!engineSound.IsPlaying)
                    {
                        engineSound.Play();
                    }
                }

                else if (engineSound.IsPaused)
                {
                    engineSound.Resume();
                }
            }
            else
            {
                if (engineSound != null && engineSound.IsPlaying)
                {
                    engineSound.Pause();
                }
            }

            if (firingGuns && !this.destroyed)
            {
                if (gunsSound == null)
                {
                    gunsSound = soundBank.GetCue("planfire");
                    if (!gunsSound.IsPlaying)
                    {
                        gunsSound.Play();
                    }
                }

                else if (gunsSound.IsPaused)
                {
                    gunsSound.Resume();
                }
            }
            else
            {
                if (gunsSound != null && gunsSound.IsPlaying)
                {
                    gunsSound.Pause();
                }
            }

            base.playSounds(soundBank);
        }

        public override void update(GameTime gameTime, Vector3 screenVector)
        {
            base.update(gameTime, screenVector);

            if ((!((BiplaneGame)this.game).gameState.player1Active && this is Player1) ||
                (!((BiplaneGame)this.game).gameState.player2Active && this is Player2))
            {

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

            if (this.target != null)
            {
                DebugLineManager.DebugLine(this.position + ((this.target.position - this.position) * 0.2f), this.target.position + ((this.position - this.target.position) * 0.2f), this.color);
            }
        }

        public override void generateObjects(GameTime gameTime, Vector3 screenVector, List<GameObject> objects)
        {
            if (!((BiplaneGame)this.game).gameState.player1Active && !((BiplaneGame)this.game).gameState.player2Active)
            {
                if (this.target != null && !(this.target is Supplies))
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

                    // he's on your 12, fox 1
                    if (Math.Abs(angleFromTarget) <= 0.1f && distance < (range * 2))
                    {
                        this.firingMissles = true;
                    }

                    // directly below you! bombs away!
                    if (angleFromTarget >= 0.7f && distance < (range * 2))
                    {
                        this.firingBombs = true;
                    }
                }
            }
            base.generateObjects(gameTime, screenVector, objects);
        }
    }
}
