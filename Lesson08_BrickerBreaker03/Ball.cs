using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Lesson08_BrickBreaker03;

/*
    Tenets of Object-Oriented Programming:
    1. Encapsulation
    2. Abstraction
    3. Inheritance
    4. Polymorphism
*/
public class Ball
{
    
    private const float _BallSpeed = 500.0f;

    private Vector2 _position;
    private Vector2 _velocity;
    private int _size;
    private bool _launched = false;

    internal Vector2 Velocity { get => _velocity; set => _velocity = value; }
    internal Vector2 Position { get => _position; set => _position = value; }
    internal bool Launched { get => _launched; set => _launched = value; }

    internal void Initialize(Vector2 position, int size)
    {
        _position = position;
        _size = size;
    }
    
    internal void Update(GameTime gameTime)
    {
        _position += _velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;
    }
}