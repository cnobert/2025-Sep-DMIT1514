using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Lesson09_Pong;

public class Ball
{
    private Rectangle _gameBoundingBox;
    private Vector2 _dimensions, _position, _direction;
    private float _speed;
    private Texture2D _pixel;
    internal void Initialize(Vector2 position, float speed, Vector2 direction, Vector2 dimensions, Rectangle gameBoundingBox)
    {
        _gameBoundingBox = gameBoundingBox;
        _position = position;
        _speed = speed;
        _direction = direction;
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

        // bounce ball off left and right sides
        if (_position.X <= _gameBoundingBox.Left || (_position.X + _dimensions.X) >= _gameBoundingBox.Right)
        {
            _direction.X *= -1;
        }
        // bounce ball off top and bottom
        if
        (
            _position.Y <= (_gameBoundingBox.Top)
            || (_position.Y + _dimensions.Y) >= (_gameBoundingBox.Bottom)
        )
        {
            _direction.Y *= -1;
        }
    }
    internal void Draw(SpriteBatch spriteBatch)
    {
        // draw ball as a stretched 1x1 pixel
        Rectangle ballRect = new Rectangle(
            (int)_position.X,
            (int)_position.Y,
            (int)_dimensions.X,
            (int)_dimensions.Y
        );
        spriteBatch.Draw(_pixel, ballRect, Color.White);
    }
}