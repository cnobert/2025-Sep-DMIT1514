using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Lesson10_BrickerBreaker_OO;

public class Paddle
{
    private Vector2 _position, _velocity;
    private Vector2 _dimensions;
    private float _speed;
    private Rectangle _gameBoundingBox;
    private Color _color;
    private Texture2D _pixel;

    public Rectangle BoundingBox
    {
        get
        {
            return new Rectangle((int)_position.X, (int)_position.Y, (int)_dimensions.X, (int)_dimensions.Y);
        }
    }

    public void Initialize(Vector2 startPosition, Vector2 dimensions, float speed, Color color, Rectangle playAreaBoundingBox)
    {
        _position = startPosition;
        _dimensions = dimensions;
        _speed = speed;
        _velocity = Vector2.Zero;
        _color = color;
        _gameBoundingBox = playAreaBoundingBox;
    }
    public void LoadContent(GraphicsDevice graphicsDevice)
    {
        _pixel = new Texture2D(graphicsDevice, 1, 1);
        _pixel.SetData(new[] { Color.White });
    }
    public void Update(GameTime gameTime)
    {
        _position += _velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;
        if (_position.X <= _gameBoundingBox.Left)
        {
            _position.X = _gameBoundingBox.Left;

        }
        if (_position.X + BoundingBox.Width >= _gameBoundingBox.Right)
        {
            _position.X = _gameBoundingBox.Right - BoundingBox.Width;
        }
    }
    
    public void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(_pixel, BoundingBox, _color);
    }
    internal void Move(float direction)
    {
        _velocity = new Vector2(direction * _speed, 0);
    }

}