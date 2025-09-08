using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Lesson01_Exercise;

public class Lesson01_Exercise : Game
{
    private const int _WindowWidth = 640, _WindowHeight = 320;
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private SpriteFont _arial;
    private string _outputString = "This is a string that will bounce around.";
    private float _x = 0, _amountToAddToX = 150, _directionX = 1;
    private float _y;

    public Lesson01_Exercise()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        _graphics.PreferredBackBufferWidth = _WindowWidth;
        _graphics.PreferredBackBufferHeight = _WindowHeight;
        _graphics.ApplyChanges();
        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        _arial = Content.Load<SpriteFont>("SystemArialFont");
    }

    protected override void Update(GameTime gameTime)
    {
        Vector2 stringDimensions = _arial.MeasureString(_outputString);

        _x += _amountToAddToX * _directionX * (float) gameTime.ElapsedGameTime.TotalSeconds;
        if (_x + stringDimensions.X > _WindowWidth || _x < 0)
        {
            _directionX *= -1;
        }

        //_y += 15 * (float) gameTime.ElapsedGameTime.TotalSeconds;
            base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        _spriteBatch.Begin();
        _spriteBatch.DrawString(_arial, _outputString, new Vector2(_x, _y), Color.White);
        _spriteBatch.End();

        base.Draw(gameTime);
    }
}
