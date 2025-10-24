using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Lesson09a_Pong_and_AI;

public class Ball
{
    private const int _CollisionTimerInterval = 400;

    private Rectangle _gameBoundingBox;
    private Vector2 _dimensions, _position, _direction;
    private float _speed;

    private int _collisionTimerMillis = 0;

    private Texture2D _pixel;

    internal Rectangle BoundingBox
    {
        get
        {
            return new Rectangle((int)_position.X, (int)_position.Y, (int)_dimensions.X, (int)_dimensions.Y);
        }
    }

    internal void Initialize(Vector2 position, float speed, Vector2 direction, Vector2 dimensions, Rectangle gameBoundingBox)
    {
        _gameBoundingBox = gameBoundingBox;
        _position = position;
        _speed = speed;
        _direction = direction;
        _dimensions = dimensions;
        _collisionTimerMillis = _CollisionTimerInterval;
    }

    internal void LoadContent(GraphicsDevice graphicsDevice)
    {
        _pixel = new Texture2D(graphicsDevice, 1, 1);
        _pixel.SetData(new[] { Color.White });
    }

    internal void Update(GameTime gameTime)
    {
        _collisionTimerMillis += gameTime.ElapsedGameTime.Milliseconds;
        _position += _direction * _speed * (float)gameTime.ElapsedGameTime.TotalSeconds;

        // Bounce off top and bottom only
        bool hitTop = _position.Y <= _gameBoundingBox.Top;
        bool hitBottom = (_position.Y + _dimensions.Y) >= _gameBoundingBox.Bottom;

        if (hitTop)
        {
            _position.Y = _gameBoundingBox.Top;
            _direction.Y *= -1f;
        }
        else if (hitBottom)
        {
            _position.Y = _gameBoundingBox.Bottom - _dimensions.Y;
            _direction.Y *= -1f;
        }
    }

    internal void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(_pixel, BoundingBox, Color.White);
    }

    internal bool ProcessCollision(Rectangle otherBoundingBox)
    {
        bool didCollide = false;

        if (_collisionTimerMillis >= _CollisionTimerInterval && BoundingBox.Intersects(otherBoundingBox))
        {
            _collisionTimerMillis = 0;
            didCollide = true;

            Rectangle intersection = Rectangle.Intersect(BoundingBox, otherBoundingBox);

            if (intersection.Width > intersection.Height)
            {
                _direction.Y *= -1f;
            }
            else
            {
                _direction.X *= -1f;
            }
        }

        return didCollide;
    }

    internal bool IsOutLeft()
    {
        bool result = false;

        if ((_position.X + _dimensions.X) < _gameBoundingBox.Left)
        {
            result = true;
        }

        return result;
    }

    internal bool IsOutRight()
    {
        bool result = false;

        if (_position.X > _gameBoundingBox.Right)
        {
            result = true;
        }

        return result;
    }

    internal void Reset(Vector2 position, Vector2 direction)
    {
        _position = position;
        _direction = direction;
        _collisionTimerMillis = _CollisionTimerInterval;
    }
}