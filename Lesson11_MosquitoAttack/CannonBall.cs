using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Lesson11_MosquitoAttack;

public class CannonBall
{
    private Texture2D _texture;
    private Vector2 _position, _velocity, _dimensions;
    private Rectangle _gameBoundingBox;
    private const float _Speed = 100;

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
        //in-class exercise:
        //make the cannon reload itself once it has left the screen
        switch(_state)
        {
            case State.Flying:
                _position += _velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;
                if(!BoundingBox.Intersects(_gameBoundingBox))
                {
                    _state = State.NotFlying;
                }
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
    internal void Shoot(Vector2 position, Vector2 direction)
    {
        if(_state == State.NotFlying)
        {
            _position = position;
            _velocity = _Speed * direction;
            _state = State.Flying;
        }
    }
    internal bool Shootable()
    {
        return _state == State.NotFlying;
    }
}