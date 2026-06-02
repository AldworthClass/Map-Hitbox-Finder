using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace Map_Hitbox_Finder
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        MouseState mouseState, prevMouseState;

        Texture2D rectTexture;

        Rectangle window;
        //Make this the size of your background/world
        Rectangle worldRect;

        Rectangle currentRect;
        List<Rectangle> Rectangles;

        SpriteFont instructionText;

        Vector2 centerScreen;

        Vector2 cameraPosition;
        Matrix cameraTransform;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            window = new Rectangle(0, 0, 800, 600);
            _graphics.PreferredBackBufferWidth = window.Width;
            _graphics.PreferredBackBufferHeight = window.Height;
            _graphics.ApplyChanges();

            centerScreen = new Vector2(window.Width / 2, window.Height / 2);

            // This should be the size of your world or background.
            worldRect = new Rectangle(0, 0, 2000, 1000);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            rectTexture = Content.Load<Texture2D>("rectangle");
            instructionText = Content.Load<SpriteFont>("InstructionText");
        }

        protected override void Update(GameTime gameTime)
        {
            prevMouseState = mouseState;
            mouseState = Mouse.GetState();

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.White);


            _spriteBatch.Begin();
            _spriteBatch.DrawString(instructionText, "Use WASD to move the map around", new Vector2(10, 470), Color.Black);
            _spriteBatch.DrawString(instructionText, "With left mouse button, draw to make a barrier", new Vector2(10, 500), Color.Black);
            _spriteBatch.DrawString(instructionText, "Right Click removes barrier", new Vector2(10, 530), Color.Black);
            _spriteBatch.DrawString(instructionText, "Enter prints Rectangles to Debug", new Vector2(10, 560), Color.Black);
            _spriteBatch.DrawString(instructionText, "Hit Space to center around game window", new Vector2(10, 560), Color.Black);
            _spriteBatch.End();

            base.Draw(gameTime);
        }

        public void SetCamera()
        {
            // Calculates the offset between out players position and the center of the game window
            cameraPosition = centerScreen - window.Center.ToVector2();

            // Uses this offset to create a translation matrix that can be applied when we draw our world.
            cameraTransform = Matrix.CreateTranslation(new Vector3(-cameraPosition, 0f));
        }

    }
}
