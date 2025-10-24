using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Lesson09_Pong;

public class Paddle
{
    private Rectangle _gameBoundingBox;

    private Vector2 _dimensions, _position, _direction;
    private float _speed;

    private Texture2D _pixel;

    internal Vector2 Direction
    {
        set => _direction = value;
    }
    internal Rectangle BoundingBox
    {
        get => new Rectangle((int)_position.X, (int)_position.Y, (int)_dimensions.X,(int)_dimensions.Y);
    }
    internal void Initialize(Vector2 position, float speed, Vector2 dimensions, Rectangle gameBoundingBox)
    {
        _gameBoundingBox = gameBoundingBox;
        _position = position;
        _speed = speed;
        _dimensions = dimensions;
    }
    internal void LoadContent(GraphicsDevice graphicsDevice)
    {
        // create a 1x1 white pixel we can stretch to draw rectangles
        _pixel = new Texture2D(graphicsDevice, 1, 1);
        _pixel.SetData(new[] { Color.White });
    }

    internal void Update(GameTime gameTime)
    {
        _position += _direction * _speed * (float)gameTime.ElapsedGameTime.TotalSeconds;

        if (_position.Y <= _gameBoundingBox.Top)
        {
            _position.Y = _gameBoundingBox.Top;
        }
        else if ((_position.Y + _dimensions.Y) >= _gameBoundingBox.Bottom)
        {
            _position.Y = _gameBoundingBox.Bottom - _dimensions.Y;
        }
    }

    internal void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(_pixel, BoundingBox, Color.White);
    }
}