using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ButtonUI;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Gravity_Simulation
{
    public class Game1 : Game
    {
        readonly GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        RenderTarget2D renderTarget;
        readonly int screenWidth, screenHeight;
        ToggleButton startStop, tracking, drawTrajectories;
        Label startStopLabel, title, simTimeLabel, simSpeedLabel, simPrecisionLabel, simFrequencyLabel;
        Canvas canvas;
        Slider simSpeedSlider, simPrecisionSlider;
        UIGroup group = new UIGroup();
        Color background = Color.CornflowerBlue;
        public static Game self;

        public Game1()
        {
            self = this;

            Content.RootDirectory = "Content";
            IsMouseVisible = true;
           
            graphics = new GraphicsDeviceManager(this);
            screenHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
            screenWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;                       
        }

        protected override void Initialize()
        {
            //graphics settings
            graphics.PreferredBackBufferWidth = screenWidth;
            graphics.PreferredBackBufferHeight = screenHeight;
            ///graphics.ToggleFullScreen();
            graphics.ApplyChanges();
            IsFixedTimeStep = true;
            ThreadStart simThreadStart = new ThreadStart(Physics.StartSimulation);
            Thread simThread = new Thread(simThreadStart);
            simThread.Start();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            renderTarget = new RenderTarget2D(GraphicsDevice, 1920, 1080);

            //UI
            Texture2D[] textures = new Texture2D[3];
            textures[0] = Content.Load<Texture2D>("button1");
            textures[1] = Content.Load<Texture2D>("button2");
            textures[2] = Content.Load<Texture2D>("button2");
            SpriteFont font = Content.Load<SpriteFont>("Arial");
            Texture2D sky = Content.Load<Texture2D>("sky");

            startStop = new ToggleButton(new Rectangle(1700, 200, 200, 100), textures, false);
            startStop.DefineText("Running", "Stopped", font, 10, Color.Black);
                group.Add(startStop);
            startStopLabel = new Label(new Rectangle(1700, 100, 200, 100), "Simulation", font, Color.Black);
                group.Add(startStopLabel);
            title = new Label(new Rectangle(0, 10, 1920, 100), "Gravity Simulator", font, Color.Black);
                group.Add(title);
            canvas = new Canvas(new Rectangle(50, 150, 1500, 850));
            canvas.sky = sky;
            tracking = new ToggleButton(new Rectangle(1700, 350, 200, 100), textures, true);
            tracking.DefineText("Tracking on", "Tracking off", font, 10, Color.Black);
                group.Add(tracking);
            drawTrajectories = new ToggleButton(new Rectangle(1700, 500, 200, 100), textures, false);
            drawTrajectories.DefineText("Drawing on", "Drawing off", font, 10, Color.Black);
                group.Add(drawTrajectories);
            simTimeLabel = new Label(new Rectangle(1400, 0, 400, 100), "Simulation time elapsed: 0 s", font, Color.Black);
                group.Add(simTimeLabel);
            simSpeedSlider = new Slider(new Rectangle(1600, 650, 300, 20), 1, 8, 8, textures);
            simSpeedSlider.value = 4;
                group.Add(simSpeedSlider);
            simSpeedLabel = new Label(new Rectangle(1600, 670, 200, 50), "Simulation speed: 1x", font, Color.Black);
                group.Add(simSpeedLabel);
            simPrecisionSlider = new Slider(new Rectangle(1600, 750, 300, 20), 1, 8, 8, textures);
            simPrecisionSlider.value = 1;
                group.Add(simPrecisionSlider);
            simPrecisionLabel = new Label(new Rectangle(1600, 770, 250, 50), "Simulation precision: 1x", font, Color.Black);
                group.Add(simPrecisionLabel);
            simFrequencyLabel = new Label(new Rectangle(100, 0, 400, 100), "Simulation frequency: 0 Hz", font, Color.Black);
                group.Add(simFrequencyLabel);
            DrawShape.Load(GraphicsDevice);
        }

        protected override void Update(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            if (Keyboard.GetState().IsKeyDown(Keys.F11))
                graphics.ToggleFullScreen();

            MouseState mouse = Mouse.GetState();

            group.Update(mouse, new KeyboardState());
           
            decimal simSpeed = (decimal)Math.Pow(2, simSpeedSlider.value - 4);
            simSpeedLabel.text = "Simulation speed: " + simSpeed.ToString("0.00") + "x";
            Physics.simSpeed = simSpeed;

            decimal simPrecision = (decimal)Math.Pow(2, simPrecisionSlider.value - 1);
            simPrecisionLabel.text = "Simulation precision: " + simPrecision.ToString("0.00") + "x";
            Physics.simPrecision = simPrecision;

            decimal simTime = Physics.simTime;
            simTimeLabel.text = "Simulation time elapsed: " + simTime.ToString("0.00") + " s";

            decimal simFrequency = Physics.simFrequency;
            simFrequencyLabel.text = "Simulation frequency: " + simFrequency.ToString("0") + " Hz";
            decimal idealSimFrequency = 1m / Physics.simPeriod;

            if (simFrequency / idealSimFrequency < 0.90m) simFrequencyLabel.color = Color.Red;
            else if (simFrequency / idealSimFrequency < 0.98m) simFrequencyLabel.color = Color.Orange;
            else simFrequencyLabel.color = Color.Black;


            Physics.running = startStop.on;
            canvas.tracking = tracking.on;
            canvas.trajectories = drawTrajectories.on;
            canvas.Update(mouse);           

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.SetRenderTarget(renderTarget);
            GraphicsDevice.Clear(Color.CornflowerBlue);

            //render to FullHD
            spriteBatch.Begin(samplerState: SamplerState.PointClamp);            
            canvas.Draw(spriteBatch);
            DrawShape.Rectangle(spriteBatch, new Rectangle(0, 0, 1920, canvas.rect.Y), background);
            DrawShape.Rectangle(spriteBatch, new Rectangle(canvas.rect.X + canvas.rect.Width, 0, 1920 - canvas.rect.X + canvas.rect.Width, 1080), background);
            DrawShape.Rectangle(spriteBatch, new Rectangle(0, canvas.rect.Y + canvas.rect.Height, 1920, 1080 - canvas.rect.Y + canvas.rect.Height), background);
            DrawShape.Rectangle(spriteBatch, new Rectangle(0, 0, canvas.rect.X, 1080), background);
            group.Draw(spriteBatch);
            spriteBatch.End();

            AdjustRenderTarget();     

            base.Draw(gameTime);
        }

        void AdjustRenderTarget()
        {
            GraphicsDevice.SetRenderTarget(null);
            spriteBatch.Begin(samplerState: SamplerState.PointClamp);

            if ((9 * screenWidth) == (16 * screenHeight))   ///for 16/9 screen ratio
                spriteBatch.Draw(renderTarget, new Rectangle(0, 0, screenWidth, screenHeight), Color.White);
            else
            {
                if ((9 * screenWidth) > (16 * screenHeight))    ///for wider screens (e.g. 21/9 ratio)
                {
                    int renderWidth = screenHeight * 16 / 9;
                    int blackBar = (screenWidth - renderWidth) / 2;

                    spriteBatch.Draw(renderTarget, new Rectangle(blackBar, 0, renderWidth, screenHeight), Color.White);
                }
                else ///for taller screens (e.g. 4/3 ratio)
                {
                    int renderHeight = screenWidth * 9 / 16;
                    int blackBar = (screenHeight - renderHeight) / 2;

                    spriteBatch.Draw(renderTarget, new Rectangle(0, blackBar, screenWidth, renderHeight), Color.White);
                }
            }
            spriteBatch.End();
        }
    }
}
