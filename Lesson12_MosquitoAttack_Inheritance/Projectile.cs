using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Lesson12_MosquitoAttack_Inheritance;

public abstract class Projectile
{
    protected Vector2 _position, _velocity, _dimensions;
    protected Rectangle _gameBoundingBox;

    protected enum State
    {
        Flying,
        NotFlying
    }
    protected State _state = State.NotFlying;

    internal Rectangle BoundingBox
    {
        get => new Rectangle((int)_position.X, (int)_position.Y, (int)_dimensions.X,(int)_dimensions.Y);
    }

    internal void Initialize(Rectangle gameBoundingBox)
    {
        _gameBoundingBox = gameBoundingBox;
        _state = State.NotFlying;
    }

    //the children of Projectile must implement this method
    internal abstract void LoadContent(ContentManager content);

    //"virtual" means "my children MAY override this method, but they don't have to"
    internal virtual void Update(GameTime gameTime)
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

    //"abstract" forces the child class to define a method with this signature
    //but does not define said method itself
    internal abstract void Draw(SpriteBatch spriteBatch);

    //we will assume that the position is "unadjusted" 
    internal void Shoot(Vector2 position, Vector2 direction, float speed)
    {
        if(_state == State.NotFlying)
        {
            //adjust the position by half the width and height of the dimensions
            _position = position - _dimensions / 2f;
            _velocity = speed * direction;
            _state = State.Flying;
        }
    }
    internal bool Shootable()
    {
        return _state == State.NotFlying;
    }

    internal bool ProcessCollision(Rectangle boundingBox)
    {
        bool hit = false;
        if(_state == State.Flying && BoundingBox.Intersects(boundingBox))
        {
            hit = true;
            _state = State.NotFlying;
        }
        return hit;
    }

    internal void MakeShootable()
    {
        if(!BoundingBox.Intersects(_gameBoundingBox))
        {
            _state = State.NotFlying;
        }
    }
}