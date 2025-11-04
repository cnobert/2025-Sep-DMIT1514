using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Lesson10_BrickerBreaker_OO;

public class Particle
{
    private Vector2 _position;
    private Vector2 _velocity;
    private float _lifetime;
    private Color _color;
    private float _size;

    public Rectangle BoundingBox
    {
        get
        {
            return new Rectangle((int)_position.X, (int)_position.Y, (int)_size, (int)_size);
        }
    }
    public bool Alive
    {
        get { return _lifetime > 0.0f; }
    }

    public void Initialize(Vector2 startPos, Color color)
    {
        Random rng = new Random();

        _position = startPos;
        //each particle has a random direction, and speed of 200f
        _velocity = new Vector2(
            (float)(rng.NextDouble() - 0.5f) * 200f,
            (float)(rng.NextDouble() - 0.5f) * 200f
        );
        //each particle exists for a random amount of time
        _lifetime = 0.5f + (float)rng.NextDouble() * 0.5f;
        _color = color;
        //random size
        _size = 4f + (float)rng.NextDouble() * 3f;
    }

    public void Update(float dt)
    {
        _position += _velocity * dt;
        _lifetime -= dt;
    }

    public void Draw(SpriteBatch sb, Texture2D pixel)
    {
        if (_lifetime > 0.0f)
        {    
                float alpha = MathHelper.Clamp(_lifetime, 0.0f, 1.0f);
                sb.Draw(pixel, BoundingBox, _color * alpha);
        }
    }
}