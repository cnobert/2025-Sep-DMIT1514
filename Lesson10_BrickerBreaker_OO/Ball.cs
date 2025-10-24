using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Lesson10_BrickerBreaker_OO;

public class Ball
{
    #region private data members
    private Vector2 _position, _velocity;
    private int _size;
    private float _speed;
    private bool _launched;
    private Rectangle _playAreaBoundingBox;
    private Random _rng = new Random();
    #endregion

    #region Trail
    private Queue<Vector2> _trail = new Queue<Vector2>();
    private const int _TrailMaxPoints = 10;
    private const float _TrailSampleSpacing = 0.01f;
    private float _trailTimer = 0.0f;
    #endregion

    #region Properties
    public Vector2 Position { get { return _position; } set { _position = value; } }
    public Vector2 Velocity { get { return _velocity; } set { _velocity = value; } }
    public int Size { get { return _size; } }
    public float Speed { get { return _speed; } }
    public bool Launched { get { return _launched; } set { _launched = value; } }
    
    public Rectangle BoundingBox
    {
        get
        {
            return new Rectangle((int)_position.X, (int)_position.Y, _size, _size);
        }
    }
    #endregion

    public void Initialize(Vector2 startPosition, int size, float speed, Rectangle playAreaBoundingBox)
    {
        _position = startPosition;
        _size = size;
        _speed = speed;
        _velocity = Vector2.Zero;
        _launched = false;
        _playAreaBoundingBox = playAreaBoundingBox;
        _trail.Clear();
        _trailTimer = 0.0f;
    }

    public void Update(GameTime gameTime)
    {
        float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

        if (_launched)
        {
            _position += _velocity * dt;

            if (_position.X <= _playAreaBoundingBox.Left)
            {
                _position.X = _playAreaBoundingBox.Left;
                _velocity.X *= -1.0f;
            }

            if (_position.X + _size >= _playAreaBoundingBox.Right)
            {
                _position.X = _playAreaBoundingBox.Right - _size;
                _velocity.X *= -1.0f;
            }

            if (_position.Y <= _playAreaBoundingBox.Top)
            {
                _position.Y = _playAreaBoundingBox.Top;
                _velocity.Y *= -1.0f;
            }

            _trailTimer += dt;
            if (_trailTimer >= _TrailSampleSpacing)
            {
                _trailTimer = 0.0f;
                Vector2 centre = new Vector2(
                    _position.X + _size * 0.5f,
                    _position.Y + _size * 0.5f
                );
                _trail.Enqueue(centre);
                if (_trail.Count > _TrailMaxPoints)
                {
                    _trail.Dequeue();
                }
            }
        }
    }

    public void Draw(SpriteBatch spriteBatch, Texture2D pixel, Color ballColor, Color trailBaseColor)
    {
        int index = 0;
        foreach (Vector2 segment in _trail)
        {
            Rectangle segmentRectangle = new Rectangle(
                (int)(segment.X - _size * 0.5f),
                (int)(segment.Y - _size * 0.5f),
                _size,
                _size
            );

            float alpha = 1.0f - (float)index / Math.Max(1, _trail.Count);
            Color c = trailBaseColor * (alpha * 0.5f);
            spriteBatch.Draw(pixel, segmentRectangle, c);
            index++;
        }

        spriteBatch.Draw(pixel, BoundingBox, ballColor);
    }

    public void ClearTrail()
    {
        _trail.Clear();
        _trailTimer = 0.0f;
    }

    public void CentreOn(Rectangle theRectangle)
    {
        float x = theRectangle.X + (theRectangle.Width / 2f) - (_size / 2f);
        float y = theRectangle.Y - _size - 5f;
        _position = new Vector2(x, y);
    }

    public void LaunchRandomUp()
    {
        float dirX = _rng.Next(2) == 0 ? -0.9f : 0.9f;
        Vector2 dir = new Vector2(dirX, -1.0f);
        SetDirectionNormalized(dir);
        _launched = true;
    }

    public void SetDirectionNormalized(Vector2 direction)
    {
        if (direction.LengthSquared() <= 0.000001f)
        {
            direction = new Vector2(0.0f, -1.0f);
        }

        direction.Normalize();
        _velocity = direction * _speed;
    }

    public void BounceX()
    {
        _velocity.X *= -1.0f;
    }

    public void BounceY()
    {
        _velocity.Y *= -1.0f;
    }

    public void CollideWith(Rectangle theRectangle)
    {
        bool collided = BoundingBox.Intersects(theRectangle);

        if (collided)
        {
            _position.Y = theRectangle.Y - _size;
            BounceY();

            float hitRatio = (
                (_position.X + _size / 2.0f) -
                (theRectangle.X + theRectangle.Width / 2.0f)
            ) / (theRectangle.Width / 2.0f);

            hitRatio = MathHelper.Clamp(hitRatio, -1.0f, 1.0f);
            Vector2 newDir = new Vector2(hitRatio, -1.0f);
            SetDirectionNormalized(newDir);
        }
    }
}
