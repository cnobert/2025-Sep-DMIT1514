using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Lesson01;

public class Lesson01 : Game
{
    private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;

        SpriteFont arial;
        const int WindowWidth = 640;
        const int WindowHeight = 320;

        float x = 0;
        float amountToAddToX = 4;

        float y = 0;
        float amountToAddToY = 4;

        string output = "This is the string I want to output";

        public Lesson01()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            graphics.PreferredBackBufferWidth = WindowWidth;
            graphics.PreferredBackBufferHeight = WindowHeight;
            graphics.ApplyChanges();
            base.Initialize();
        }
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            arial = Content.Load<SpriteFont>("SystemArialFont");
        }
        protected override void Update(GameTime gameTime)
        {
            Vector2 stringDimensions = arial.MeasureString(output);

            x += amountToAddToX;
            if (x < 0 || x > WindowWidth - stringDimensions.X)
            {
                amountToAddToX *= -1;
            }

            y += amountToAddToY;
            if(y < 0 || y > WindowHeight - stringDimensions.Y)
            {
                amountToAddToY *= -1;
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();

            spriteBatch.DrawString(arial, output, new Vector2(x, y), Color.White);

            spriteBatch.End();
            
            base.Draw(gameTime);
        }
}
