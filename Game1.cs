using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ButtonUI;

namespace Gravity_Simulation
{
    public class Game1 : Game
    {
        readonly GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        RenderTarget2D renderTarget;
        readonly int screenWidth, screenHeight;
        ToggleButton startStop;
        ToggleButton tracking;
        Label startStopLabel, title;
        Canvas canvas;
        UIGroup group = new UIGroup();
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
           
            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            renderTarget = new RenderTarget2D(GraphicsDevice, 1920, 1080);

            //UI
            Texture2D[] textures = new Texture2D[2];
            textures[0] = Content.Load<Texture2D>("button1");
            textures[1] = Content.Load<Texture2D>("button2");
            SpriteFont font = Content.Load<SpriteFont>("Arial");

            startStop = new ToggleButton(new Rectangle(1700, 200, 200, 100), textures, false);
            startStop.DefineText("Running", "Stopped", font, 10, Color.Black);
                group.Add(startStop);
            startStopLabel = new Label(new Rectangle(1700, 100, 200, 100), "Simulation", font, Color.Black);
                group.Add(startStopLabel);
            title = new Label(new Rectangle(0, 10, 1920, 100), "Gravity Simulator", font, Color.Black);
                group.Add(title);
            canvas = new Canvas(new Rectangle(50, 150, 1500, 850));
            tracking = new ToggleButton(new Rectangle(1700, 350, 200, 100), textures, true);
            tracking.DefineText("Tracking on", "Tracking off", font, 10, Color.Black);
                group.Add(tracking);
            DrawShape.Load(GraphicsDevice);
        }

        protected override void Update(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            if (Keyboard.GetState().IsKeyDown(Keys.F11))
                graphics.ToggleFullScreen();

            MouseState mouse = Mouse.GetState();
            ///KeyboardState keyboard = Keyboard.GetState();

            group.Update(mouse, new KeyboardState());
            if (startStop.on)
            {
                canvas.Update(mouse);
                if (tracking.on) canvas.TrackCenterOfMass();
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.SetRenderTarget(renderTarget);
            GraphicsDevice.Clear(Color.CornflowerBlue);

            //render to FullHD
            spriteBatch.Begin(samplerState: SamplerState.PointClamp);
            group.Draw(spriteBatch);
            canvas.Draw(spriteBatch);
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
