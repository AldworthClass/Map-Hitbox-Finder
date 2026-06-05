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

        Texture2D rectTexture, rectOutlineTexture;

        Rectangle window;
        //Make this the size of your background/world
        Rectangle worldRect;

        // Stores initial point where rectangle is created
        Vector2 rectStart;
        // Stores rectangle that is being drawn as it is created
        Rectangle drawRect;
        // Stores all created rectangles
        List<Rectangle> rectangles;


        SpriteFont instructionText;
        Rectangle instructionRect1, instructionRect2;

        // Where the camera is centered in the game world
        Vector2 viewLocation;
        // Where the in world the mouse coordinates are
        Vector2 mouseWorldPosition;

        // Speed that you scroll around the world
        Vector2 scrollSpeed;

        // Camera offset for matrix translation
        Vector2 cameraPosition;
        Matrix cameraTransform;

        float zoom;
        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
           
            // Set this size to match your game window
            window = new Rectangle(0, 0, 800, 600);
            _graphics.PreferredBackBufferWidth = window.Width;
            _graphics.PreferredBackBufferHeight = window.Height;
            _graphics.ApplyChanges();

            zoom = 1f;
            showHelp = false;

            viewLocation = window.Center.ToVector2();

            // This should be the size of your world or background.  It may be the same size of your game window.
            worldRect = new Rectangle(0, 0, 2000, 1000);

            rectangles = new List<Rectangle>();
            instructionRect1 = new Rectangle(5, 355, 330, 150);
            instructionRect2 = new Rectangle(5, 9, 200, 20);
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            rectTexture = Content.Load<Texture2D>("rectangle");
            rectOutlineTexture = Content.Load<Texture2D>("rectangle outline");
            instructionText = Content.Load<SpriteFont>("InstructionText");
        }

        protected override void Update(GameTime gameTime)
        {
            prevMouseState = mouseState;
            mouseState = Mouse.GetState();

            // Converts game window mouse coordinate to game world mouse coordinate
            Matrix inverseTransform = Matrix.Invert(cameraTransform);
            mouseWorldPosition = Vector2.Transform(mouseState.Position.ToVector2(), inverseTransform);

            prevKeyboardState = keyboardState;
            keyboardState = Keyboard.GetState();


            if (keyboardState.IsKeyDown(Keys.LeftControl) && mouseState.ScrollWheelValue > prevMouseState.ScrollWheelValue)
            {
                zoom += 0.05f; // zoom in
            }
            if (keyboardState.IsKeyDown(Keys.LeftControl) && mouseState.ScrollWheelValue < prevMouseState.ScrollWheelValue)
            {
                zoom -= 0.05f; // zoom out
            }
            zoom = MathHelper.Clamp(zoom, 0.22f, 5f);



            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // Determines scroll Speed
            scrollSpeed = Vector2.Zero;
            if (keyboardState.IsKeyDown(Keys.W))
                scrollSpeed.Y -= 5;
            if (keyboardState.IsKeyDown(Keys.S))
                scrollSpeed.Y += 5;
            if (keyboardState.IsKeyDown(Keys.A))
                scrollSpeed.X -= 5;
            if (keyboardState.IsKeyDown(Keys.D))
                scrollSpeed.X += 5;

            viewLocation += scrollSpeed;
            SetCamera();    // Determines translation matrix

            // Detects left click to create a rectangle
            if (mouseState.LeftButton == ButtonState.Pressed && prevMouseState.LeftButton == ButtonState.Released)
            {
                rectStart = mouseWorldPosition;
                drawRect = new Rectangle(rectStart.ToPoint(), new Point(0, 0));
            }
            // Scales created rectangle when user drags corner
            else if (mouseState.LeftButton == ButtonState.Pressed)
            {
                // Cursor is BELOW starting point
                if (mouseWorldPosition.Y > rectStart.Y)
                {
                    drawRect.X = (int)rectStart.X;
                    drawRect.Height = (int)mouseWorldPosition.Y - (int)rectStart.Y;

                }
                // Cursor is ABOVE starting point
                else if (mouseWorldPosition.Y < rectStart.Y)
                {
                    drawRect.Y = (int)mouseWorldPosition.Y;
                    drawRect.Height = (int)rectStart.Y - drawRect.Y;
                }
                else // Cursor is horizontal to starting point
                {
                    drawRect.Height = 0;
                }

                // Cursor is to the right of starting point
                if (mouseWorldPosition.X > rectStart.X)
                {
                    drawRect.X = (int)rectStart.X;
                    drawRect.Width = (int)mouseWorldPosition.X - (int)rectStart.X;
                }
                // Cursor is to the left of starting point
                else if (mouseWorldPosition.X < rectStart.X)
                {
                    drawRect.X = (int)mouseWorldPosition.X;
                    drawRect.Width = (int)rectStart.X - drawRect.X;
                }
                else // Cursor is vertical to starting point
                {
                    drawRect.Width = 0;
                }

            }
            // User releases the mouse (rectangle added to list)
            else if (mouseState.LeftButton == ButtonState.Released && prevMouseState.LeftButton == ButtonState.Pressed)
            {
                if (drawRect.Width != 0 && drawRect.Height != 0) 
                    rectangles.Add(drawRect);
                rectStart = Vector2.Zero;
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

            if (keyboardState.IsKeyDown(Keys.Z) && prevKeyboardState.IsKeyUp(Keys.Z))
            {
                zoom = 1f;
            }

            // Toggles instructions
            if (keyboardState.IsKeyDown(Keys.I) && prevKeyboardState.IsKeyUp(Keys.I))
            showHelp = !showHelp;



                this.Window.Title = drawRect.ToString();

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.White);

            _spriteBatch.Begin(transformMatrix: cameraTransform);

            // Draw your game world background/map here
            //_spriteBatch.Draw(your background texture, worldRect, color.White);

            _spriteBatch.Draw(rectOutlineTexture, window, Color.Black);
            _spriteBatch.Draw(rectTexture, drawRect, Color.Blue);
            foreach (Rectangle rect in rectangles)
                _spriteBatch.Draw(rectTexture, rect, Color.Black * 0.5f);

            _spriteBatch.End();


            _spriteBatch.Begin();
            _spriteBatch.Draw(rectTexture, instructionRect2, Color.White * 0.8f);
            if (showHelp)
            {
                _spriteBatch.Draw(rectTexture, instructionRect1, Color.White * 0.8f);
                _spriteBatch.DrawString(instructionText, "Press 'I' to hide instructions", new Vector2(10, 10), Color.Black);
                _spriteBatch.DrawString(instructionText, "Press 'Ctrl' + mouse wheel to zoom in and out", new Vector2(10, 390), Color.Black);
                _spriteBatch.DrawString(instructionText, "Press 'Z' to undo zoom", new Vector2(10, 410), Color.Black);
                _spriteBatch.DrawString(instructionText, "Space centers around game window", new Vector2(10, 440), Color.Black);
                _spriteBatch.DrawString(instructionText, "Use WASD to move the map around", new Vector2(10, 470), Color.Black);
                _spriteBatch.DrawString(instructionText, "Drag with left mouse button to make a barrier", new Vector2(10, 500), Color.Black);
                _spriteBatch.DrawString(instructionText, "Right-Click to remove a barrier", new Vector2(10, 530), Color.Black);
                _spriteBatch.DrawString(instructionText, "Enter to Rectangles to output", new Vector2(10, 560), Color.Black);
            }
            else
                _spriteBatch.DrawString(instructionText, "Press 'I' for instructions", new Vector2(10, 10), Color.Black);


            _spriteBatch.End();

            base.Draw(gameTime);
        }

        public void SetCamera()
        {
            // Calculates the offset between out players position and the center of the game window
            cameraPosition = viewLocation - window.Center.ToVector2();

            // Uses this offset to create a translation matrix that can be applied when we draw our world.
            cameraTransform = 
                Matrix.CreateTranslation(new Vector3(-cameraPosition, 0f)) * // Applies offset for location
                Matrix.CreateTranslation(   // Scaling happens relative to 0, 0 so we shift origin so scaling happens here
                    -GraphicsDevice.Viewport.Width * 0.5f,
                    -GraphicsDevice.Viewport.Height * 0.5f, 
                    0) *
                Matrix.CreateScale(zoom, zoom, 1f) *    // Applies Zoom
                Matrix.CreateTranslation(               // Re-centers after zoom
                    GraphicsDevice.Viewport.Width * 0.5f,
                    GraphicsDevice.Viewport.Height * 0.5f,
                    0);
            
        }

    }
}
