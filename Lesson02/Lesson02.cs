using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Lesson02;

public class Lesson02 : Game
{
    private const int _WindowWidth = 640, _WindowHeight = 320;
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private float _x = 0, _amountToAddToX = 150, _directionX = 1;
    private float _y, _amountToAddToY = 150, _directionY = 1;
    private Texture2D _spaceStation, _ship;
    private float _rotation = 0;
    public Lesson02()
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
        _spaceStation = Content.Load<Texture2D>("Station");
        _ship = Content.Load<Texture2D>("Beetle");

    }

    protected override void Update(GameTime gameTime)
    {

        _x += _amountToAddToX * _directionX * (float)gameTime.ElapsedGameTime.TotalSeconds;
        if (_x + _ship.Width > _WindowWidth || _x < 0)
        {
            _directionX *= -1;
        }

        _y += _amountToAddToY * _directionY * (float)gameTime.ElapsedGameTime.TotalSeconds;
        if (_y + _ship.Height > _WindowHeight || _y < 0)
        {
            _directionY *= -1;
        }
        _rotation += 6 * (float)gameTime.ElapsedGameTime.TotalSeconds;
        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        _spriteBatch.Begin();

        _spriteBatch.Draw(_spaceStation, Vector2.Zero, Color.White);
        _spriteBatch.Draw(_ship, new Vector2(_x + (_ship.Bounds.Width / 2), 
                _y + (_ship.Bounds.Height / 2)), null, Color.White, _rotation, 
                new Vector2(_ship.Bounds.Width / 2, _ship.Bounds.Height / 2), 1, SpriteEffects.None, 0);
 
        _spriteBatch.End();

        base.Draw(gameTime);
    }
}
