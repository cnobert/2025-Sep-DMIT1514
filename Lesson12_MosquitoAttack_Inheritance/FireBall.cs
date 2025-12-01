using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Lesson12_MosquitoAttack_Inheritance;

public class FireBall : Projectile
{
    private const float _Speed = 100;
    
    private SimpleAnimation _animation;

    internal Rectangle BoundingBox
    {
        get
        {
            return new Rectangle((int)_position.X, (int)_position.Y, (int)_animation.FrameDimensions.X, (int)_animation.FrameDimensions.Y);
        }
    }

    internal void LoadContent(ContentManager content)
    {
        Texture2D texture = content.Load<Texture2D>("FireBall");
        _animation = new SimpleAnimation(texture, 5, texture.Height, texture.Width / 5, 8f);
    }

    internal void Update(GameTime gameTime)
    {
        switch(_state)
        {
            case State.Flying:
                if(!BoundingBox.Intersects(_gameBoundingBox))
                {
                    _state = State.NotFlying;
                }
                _animation.Update(gameTime);
                break;
            case State.NotFlying:
                break;
        }
    }
    internal void Draw(SpriteBatch spriteBatch)
    {
        switch(_state)
        {
            case State.Flying:
                _animation.Draw(spriteBatch, _position, SpriteEffects.None);
                break;
            case State.NotFlying:
                break;
        }
    }

    internal void Shoot(Vector2 position, Vector2 direction)
    {
        if(_state == State.NotFlying)
        {
            //adjust the position by half the width and height of the dimensions
            _position = position - _dimensions / 2f;
            _velocity = _Speed * direction;
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
}
