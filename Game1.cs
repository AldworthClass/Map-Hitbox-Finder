using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.Diagnostics;

namespace Map_Hitbox_Finder
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        MouseState mouseState, prevMouseState;
        KeyboardState keyboardState, prevKeyboardState;

        bool showHelp;

        Texture2D rectTexture;

        Rectangle window;
        //Make this the size of your background/world
        Rectangle worldRect;

        Rectangle currentRect, drawRect;
        List<Rectangle> rectangles;


        SpriteFont instructionText;

        // Where the camera is centered in the game world
        Vector2 viewLocation;
        // Where in the world the mouse coordinates are
        Vector2 mouseWorldPosition;

        // Speed that you scroll around the world
        Vector2 scrollSpeed;

        // Camera offset for matrix translation
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

            showHelp = false;

            viewLocation = window.Center.ToVector2();

            // This should be the size of your world or background.
            worldRect = new Rectangle(0, 0, 2000, 1000);
            rectangles = new List<Rectangle>();
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

            Matrix inverseTransform = Matrix.Invert(cameraTransform);
            mouseWorldPosition = Vector2.Transform(mouseState.Position.ToVector2(), inverseTransform);

            prevKeyboardState = keyboardState;
            keyboardState = Keyboard.GetState();


            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // Scroll Speed
            scrollSpeed = Vector2.Zero;
            if (keyboardState.IsKeyDown(Keys.W))
                scrollSpeed.Y -= 2;
            if (keyboardState.IsKeyDown(Keys.S))
                scrollSpeed.Y += 2;
            if (keyboardState.IsKeyDown(Keys.A))
                scrollSpeed.X -= 2;
            if (keyboardState.IsKeyDown(Keys.D))
                scrollSpeed.X += 2;

            viewLocation += scrollSpeed;

            SetCamera();

            // TODO: Add your update logic here
            // Create rectangle that is being drawn and the current one.
            if (mouseState.LeftButton == ButtonState.Pressed && prevMouseState.LeftButton == ButtonState.Released)
            {
                currentRect = new Rectangle(mouseWorldPosition.ToPoint(), new Point(0, 0));
                drawRect = currentRect;
            }
            else if (mouseState.LeftButton == ButtonState.Pressed)
            {

                // Cursor is BELOW starting point
                if (mouseWorldPosition.Y > currentRect.Y)
                {
                    drawRect.X = currentRect.X;
                    drawRect.Height = (int)mouseWorldPosition.Y - currentRect.Y;

                }
                // Cursor is ABOVE starting point
                else if (mouseWorldPosition.Y < currentRect.Y)
                {
                    drawRect.Y = (int)mouseWorldPosition.Y;
                    drawRect.Height = currentRect.Y - drawRect.Y;
                }
                else 
                {
                    drawRect.Height = 0;
                }

                // Cursor is to the right of starting point
                if (mouseWorldPosition.X > currentRect.X)
                {
                    drawRect.X = currentRect.X;
                    drawRect.Width = (int)mouseWorldPosition.X - currentRect.X;
                }
                // Cursor is to the left of starting point
                else if (mouseWorldPosition.X < currentRect.X)
                {
                    drawRect.X = (int)mouseWorldPosition.X;
                    drawRect.Width = currentRect.X - drawRect.X;
                }
                else
                {
                    drawRect.Width = 0;
                }

            }
            // User releases the mouse
            else if (mouseState.LeftButton == ButtonState.Released && prevMouseState.LeftButton == ButtonState.Pressed)
            {
                currentRect = drawRect;
                if (currentRect.Width != 0 && currentRect.Height != 0) 
                    rectangles.Add(currentRect);
                currentRect = Rectangle.Empty;
                drawRect = Rectangle.Empty;
            }

            // Deletes Rectangles upon right click
            if (mouseState.LeftButton == ButtonState.Released && mouseState.RightButton == ButtonState.Pressed && prevMouseState.RightButton == ButtonState.Released)
            {
                for (int i = 0; i < rectangles.Count; i++)
                {
                    if (rectangles[i].Contains(mouseWorldPosition.ToPoint()))
                        rectangles.RemoveAt(i);
                }
            }


            // Prints list of rectangles to output
            if (keyboardState.IsKeyDown(Keys.Enter) && prevKeyboardState.IsKeyUp(Keys.Enter))
            {
                foreach (Rectangle rect in rectangles)
                    Debug.WriteLine($"new Rectangle({rect.X}, {rect.Y}, {rect.Width}, {rect.Height});");
                Debug.WriteLine("DONE");
            }

            // Re-Centers window
            if (keyboardState.IsKeyDown(Keys.Space) && prevKeyboardState.IsKeyUp(Keys.Space))
            {
                viewLocation = window.Center.ToVector2();
            }

                this.Window.Title = drawRect.ToString();

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.White);

            _spriteBatch.Begin(transformMatrix: cameraTransform);
            _spriteBatch.Draw(rectTexture, drawRect, Color.Blue);
            foreach (Rectangle rect in rectangles)
                _spriteBatch.Draw(rectTexture, rect, Color.Black * 0.5f);

            _spriteBatch.End();


            _spriteBatch.Begin();
            
            _spriteBatch.DrawString(instructionText, "Hit Space to center around game window", new Vector2(10, 440), Color.Black);
            _spriteBatch.DrawString(instructionText, "Use WASD to move the map around", new Vector2(10, 470), Color.Black);
            _spriteBatch.DrawString(instructionText, "With left mouse button, draw to make a barrier", new Vector2(10, 500), Color.Black);
            _spriteBatch.DrawString(instructionText, "Right Click removes barrier", new Vector2(10, 530), Color.Black);
            _spriteBatch.DrawString(instructionText, "Enter prints Rectangles to Debug", new Vector2(10, 560), Color.Black);
            _spriteBatch.End();

            base.Draw(gameTime);
        }

        public void SetCamera()
        {
            // Calculates the offset between out players position and the center of the game window
            cameraPosition = viewLocation - window.Center.ToVector2();

            // Uses this offset to create a translation matrix that can be applied when we draw our world.
            cameraTransform = Matrix.CreateTranslation(new Vector3(-cameraPosition, 0f));
        }

    }
}
