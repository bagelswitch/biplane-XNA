using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.Content;
using Biplane.Objects.Player;
using Biplane.Objects.Target;
using Biplane.Objects.Weapon;
using Biplane.Objects.UI;
using Biplane.Objects.Static;
using Biplane.Objects;
using Biplane.ParticleSystem;
using Biplane.Service;
using Biplane.Components;

namespace Biplane
{
    public class BiplaneGame : Microsoft.Xna.Framework.Game
    {
        
        // general
        public GraphicsDeviceManager graphics;
        public SpriteBatch spriteBatch;
        private ContentManager content;

        public Texture2D background;

        private float aspectRatio = 1280.0f / 720.0f;

        // particle systems
        public ParticleSystem.ParticleSystem explosionParticles;
        public ParticleSystem.ParticleSystem explosionSmokeParticles;
        public ParticleSystem.ParticleSystem projectileTrailParticles;
        public ParticleSystem.ParticleSystem smokePlumeParticles;
        public ParticleSystem.ParticleSystem fireParticles;
        public ParticleSystem.ParticleSystem vaporTrailParticles;
        public ParticleSystem.ParticleSystem vaporPlumeParticles;
        public ParticleSystem.ParticleSystem tracerParticles;

        // Audio
        public SoundBank soundBank;
        public AudioEngine audioEngine;
        public WaveBank waveBank;
        public WaveBank musicWaveBank;
        public SoundBank musicSoundBank;

        public class GameState
        {
            // Game state
            public GameObject player1;
            public GameObject player2;
            public bool player1Alive = false;
            public bool player1Active = false;
            public int player1score = 0;
            public bool player2Alive = false;
            public bool player2Active = false;
            public int player2score = 0;
            public Vector3 player1pos = new Vector3();
            public Vector3 player2pos = new Vector3();

            public int numPlayers;

            public int stage;

            public int level;

            public Matrix viewMatrix;
            public Matrix projectionMatrix;

            public bool paused;
            public bool loading;

            public long pauseTime;

            public bool zMotion = false;

            public int idleCam = 1;

            public List<GameObject> objects;
        }

        public GameState gameState;

        public BiplaneGame()
        {
            this.gameState = new GameState();

            Content.RootDirectory = "Content";
            content = this.Content;

            this.gameState.objects = new List<GameObject>();

            graphics = new GraphicsDeviceManager(this);
            graphics.IsFullScreen = true;

            graphics.MinimumPixelShaderProfile = ShaderProfile.PS_2_0;

            // GameObject manager setup
            PlayerManager plm = new PlayerManager(this);
            BaronManager bam = new BaronManager(this);
            BomberManager bom = new BomberManager(this);
            ZeppelinManager zem = new ZeppelinManager(this);
            BuildingManager bgm = new BuildingManager(this);
            GameObjectManager gom = new GameObjectManager(this);

            plm.UpdateOrder = 100;
            bam.UpdateOrder = 200;
            bom.UpdateOrder = 300;
            zem.UpdateOrder = 400;
            bgm.UpdateOrder = 500;
            gom.UpdateOrder = 1000;

            Components.Add(plm);
            Components.Add(bam);
            Components.Add(bom);
            Components.Add(zem);
            Components.Add(bgm);
            Components.Add(gom);

            DebugLineManager dlm = new DebugLineManager(this);
            dlm.DrawOrder = 9999;
            dlm.Enabled = true;
            Components.Add(dlm);

            // particle system setup
            // Construct our particle system components.
            explosionParticles = new ExplosionParticleSystem(this, Content);
            explosionSmokeParticles = new ExplosionSmokeParticleSystem(this, Content);
            projectileTrailParticles = new ProjectileTrailParticleSystem(this, Content);
            vaporTrailParticles = new VaporTrailParticleSystem(this, Content);
            smokePlumeParticles = new SmokePlumeParticleSystem(this, Content);
            fireParticles = new FireParticleSystem(this, Content);
            vaporPlumeParticles = new VaporPlumeParticleSystem(this, Content);
            tracerParticles = new TracerParticleSystem(this, Content);
            // Set the draw order so the explosions and fire
            // will appear over the top of the smoke.
            smokePlumeParticles.DrawOrder = 100;
            vaporPlumeParticles.DrawOrder = 100;
            explosionSmokeParticles.DrawOrder = 200;
            projectileTrailParticles.DrawOrder = 300;
            vaporTrailParticles.DrawOrder = 400;
            explosionParticles.DrawOrder = 500;
            fireParticles.DrawOrder = 600;
            tracerParticles.DrawOrder = 700;
            // Register the particle system components.
            Components.Add(explosionParticles);
            Components.Add(explosionSmokeParticles);
            Components.Add(projectileTrailParticles);
            Components.Add(vaporTrailParticles);
            Components.Add(smokePlumeParticles);
            Components.Add(fireParticles);
            Components.Add(vaporPlumeParticles);
            Components.Add(tracerParticles);

            // camera manager setup
            CameraService cs = new CameraService(this);
            Services.AddService(typeof(CameraProvider), cs);
            Components.Add(cs);

            // postprocessing setup
            BloomComponent bloom = new BloomComponent(this);
            bloom.Settings = BloomSettings.PresetSettings[6];
            bloom.Visible = true;
            Components.Add(bloom);    

            // UI text service setup
            UITextService uitService = new UITextService(this);
            uitService.DrawOrder = 1000;
            Services.AddService(typeof(UITextProvider), uitService);
            Components.Add(uitService);

            Components.Add(new MenuManager(this));

            Components.Add(new LoadManager(this));

            this.gameState.paused = true;
            this.gameState.pauseTime = 0;
            this.gameState.stage = 1;
            this.gameState.level = 1;
            this.gameState.numPlayers = 2;

            this.gameState.loading = false;
        }

        protected override void Initialize()
        {
            audioEngine = new AudioEngine("Content\\Audio\\Biplane.xgs");
            waveBank = new WaveBank(audioEngine, "Content\\Audio\\Wave Bank.xwb");
            soundBank = new SoundBank(audioEngine, "Content\\Audio\\Sound Bank.xsb");

            musicWaveBank = new WaveBank(audioEngine, "Content\\Audio\\Music Wave Bank.xwb");
            musicSoundBank = new SoundBank(audioEngine, "Content\\Audio\\Music Sound Bank.xsb");

            musicSoundBank.PlayCue("usmchymn");

            base.Initialize();
        }

        protected override void LoadContent()
        {
            InitializeTransform();

            spriteBatch = new SpriteBatch(graphics.GraphicsDevice);

            this.background = Content.Load<Texture2D>("Textures\\sky" + this.gameState.level);

            base.LoadContent();
        }

        private void InitializeTransform()
        {
            gameState.projectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45.0f), aspectRatio, 5000.0f, 2000000.0f);
        }

        protected override void UnloadContent()
        {
            content.Unload();

            base.UnloadContent();
        }

        protected override void Update(GameTime gameTime)
        {
            // Allows the default game to exit on Xbox 360 and Windows
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                this.Exit();

            if (GamePad.GetState(PlayerIndex.One).Buttons.Start == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Enter))
            {
                if ((gameTime.TotalRealTime.Ticks - this.gameState.pauseTime) > 3000000)
                {
                    this.gameState.paused = !this.gameState.paused;
                    this.gameState.pauseTime = gameTime.TotalRealTime.Ticks;
                }
            }

            if (this.gameState.level == 1 && (this.gameState.player1score * 347 > 500000 || this.gameState.player2score * 347 > 500000))
            {
                this.gameState.level = 2;
                this.gameState.loading = true;
            }

            if (this.gameState.level == 2 && (this.gameState.player1score * 347 > 5000000 || this.gameState.player2score * 347 > 5000000))
            {
                this.gameState.level = 3;
                this.gameState.loading = true;
            }

            foreach (GameComponent gc in this.Components)
            {
                if (gc is MenuManager)
                {
                    ((DrawableGameComponent)gc).Enabled = this.gameState.paused;
                    ((DrawableGameComponent)gc).Visible = this.gameState.paused;
                }
                else if (gc is LoadManager)
                {
                    ((DrawableGameComponent)gc).Enabled = this.gameState.loading && !this.gameState.paused;
                    ((DrawableGameComponent)gc).Visible = this.gameState.loading && !this.gameState.paused;
                }
                else if (gc is DrawableGameComponent)
                {
                    gc.Enabled = !this.gameState.paused && !this.gameState.loading;
                    ((DrawableGameComponent)gc).Visible = !this.gameState.paused && !this.gameState.loading;
                }
            }

            base.Update(gameTime);     
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // Pass camera matrices through to the particle system components.
            explosionParticles.SetCamera(gameState.viewMatrix, gameState.projectionMatrix);
            explosionSmokeParticles.SetCamera(gameState.viewMatrix, gameState.projectionMatrix);
            projectileTrailParticles.SetCamera(gameState.viewMatrix, gameState.projectionMatrix);
            vaporTrailParticles.SetCamera(gameState.viewMatrix, gameState.projectionMatrix);
            smokePlumeParticles.SetCamera(gameState.viewMatrix, gameState.projectionMatrix);
            fireParticles.SetCamera(gameState.viewMatrix, gameState.projectionMatrix);
            vaporPlumeParticles.SetCamera(gameState.viewMatrix, gameState.projectionMatrix);
            tracerParticles.SetCamera(gameState.viewMatrix, gameState.projectionMatrix);

            base.Draw(gameTime);
        }

        static class Program
        {
            static void Main()
            {
                using (BiplaneGame game = new BiplaneGame())
                {
                    game.Run();
                }
            }
        }
    }
}
