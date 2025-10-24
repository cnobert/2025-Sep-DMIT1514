using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Lesson09a_Pong_and_AI;

public class Paddle
{
    private Rectangle _gameBoundingBox;

    private Vector2 _dimensions, _position, _direction;
    private float _speed;

    private Texture2D _pixel;

    // Multi-colour stripes from top to bottom
    private Color[] _stripeColors = new Color[] { Color.White };

    internal Vector2 Direction
    {
        set
        {
            _direction = value;
        }
    }

    internal Rectangle BoundingBox
    {
        get
        {
            return new Rectangle((int)_position.X, (int)_position.Y, (int)_dimensions.X, (int)_dimensions.Y);
        }
    }

    internal void Initialize(Vector2 position, float speed, Vector2 dimensions, Rectangle gameBoundingBox, Color[] stripeColors)
    {
        _gameBoundingBox = gameBoundingBox;
        _position = position;
        _speed = speed;
        _dimensions = dimensions;

        if (stripeColors != null && stripeColors.Length > 0)
        {
            _stripeColors = stripeColors;
        }
    }

    internal void LoadContent(GraphicsDevice graphicsDevice)
    {
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
        int stripeCount = _stripeColors.Length;
        int fullStripeHeight = stripeCount > 0 ? (int)(_dimensions.Y / stripeCount) : (int)_dimensions.Y;
        int yCursor = (int)_position.Y;
        int stripesDrawn = 0;

        while (stripesDrawn < stripeCount)
        {
            int height = fullStripeHeight;

            if (stripesDrawn == stripeCount - 1)
            {
                int remaining = (int)(_position.Y + _dimensions.Y) - yCursor;
                height = remaining;
            }

            Rectangle stripeRect = new Rectangle((int)_position.X, yCursor, (int)_dimensions.X, height);
            spriteBatch.Draw(_pixel, stripeRect, _stripeColors[stripesDrawn]);

            yCursor += height;
            stripesDrawn++;
        }
    }
}