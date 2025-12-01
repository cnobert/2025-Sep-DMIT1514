using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Lesson12_MosquitoAttack_Inheritance;

public class CannonBall
{
    private const float _Speed = 100;
    
    private Texture2D _texture;

    private Vector2 _position, _velocity, _dimensions;
    private Rectangle _gameBoundingBox;
    
    private enum State
    {
        Flying,
        NotFlying
    }
    private State _state = State.NotFlying;

    internal Rectangle BoundingBox
    {
        get => new Rectangle((int)_position.X, (int)_position.Y, (int)_dimensions.X,(int)_dimensions.Y);
    }
    internal void Initialize(Rectangle gameBoundingBox)
    {
        _gameBoundingBox = gameBoundingBox;
        _state = State.NotFlying;
    }
    internal void LoadContent(ContentManager content)
    {
        _texture = content.Load<Texture2D>("CannonBall");
        _dimensions = new Vector2(_texture.Width, _texture.Height);
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
    internal void Draw(SpriteBatch spriteBatch)
    {
        switch(_state)
        {
            case State.Flying:
                spriteBatch.Draw(_texture, _position, Color.White);
                break;
            case State.NotFlying:
                break;
        }
    }
    //we will assume that the position is "unadjusted" 
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
    internal void MakeShootable()
    {
        if(!BoundingBox.Intersects(_gameBoundingBox))
        {
            _state = State.NotFlying;
        }

    }
    internal bool ProcessCollision(Rectangle boundingBox)
    {
        bool hit = false;
        if(_state == State.Flying && BoundingBox.Intersects(boundingBox))
        {
            hit = true;
            //_state = State.NotFlying;
        }
        return hit;
    }
}