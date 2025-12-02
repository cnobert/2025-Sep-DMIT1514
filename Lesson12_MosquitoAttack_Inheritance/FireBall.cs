using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Lesson12_MosquitoAttack_Inheritance;

public class FireBall : Projectile
{
    private SimpleAnimation _animation;

    internal override void LoadContent(ContentManager content)
    {
        Texture2D texture = content.Load<Texture2D>("FireBall");
        _animation = new SimpleAnimation(texture, 5, texture.Height, texture.Width / 5, 8f);
        _dimensions = new Vector2(_animation.FrameDimensions.X, _animation.FrameDimensions.Y);
    }
    //"override" means "I'm hiding the parent method".
    internal override void Update(GameTime gameTime)
    {
        //"base" = the parent of the object that this code is running in
        base.Update(gameTime);
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
    internal override void Draw(SpriteBatch spriteBatch)
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
}
