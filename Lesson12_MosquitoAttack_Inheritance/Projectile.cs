using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Lesson12_MosquitoAttack_Inheritance;

public class Projectile
{
    protected Vector2 _position, _velocity, _dimensions;
    protected Rectangle _gameBoundingBox;

    protected enum State
    {
        Flying,
        NotFlying
    }
    protected State _state = State.NotFlying;

    internal void Initialize(Rectangle gameBoundingBox)
    {
        _gameBoundingBox = gameBoundingBox;
        _state = State.NotFlying;
    }
    internal void Update(GameTime gameTime)
    {
        switch(_state)
        {
            case State.Flying:
                _position += _velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;
                break;
            case State.NotFlying:
                break;
        }
    }
}