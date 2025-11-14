using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Diagnostics;

namespace Lesson11_MosquitoAttack;

public class Cannon
{
    private const float _MaxSpeed = 50;

    private SimpleAnimation _animation;
    private Vector2 _position, _direction;

    internal Vector2 Direction { set => _direction = value; }

    internal void Initialize(Vector2 position)
    {
        _position = position;
    }
    internal void LoadContent(ContentManager content)
    {
        Texture2D texture = content.Load<Texture2D>("Cannon");
        _animation = new SimpleAnimation(texture, texture.Width / 4, texture.Height, 4, 2f);
        _animation.Paused = false;
    }
    internal void Update(GameTime gameTime)
    {
        _position += _direction * _MaxSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
        if(_direction.X != 0)
        {
            if(_direction.X == -1)
            {
                _animation.Reverse = true;
            }
            else
            {
                _animation.Reverse = false;
            }
            _animation.Update(gameTime);
        }
    }
    internal void Draw(SpriteBatch spriteBatch)
    {
        if(_animation != null)
        {
            _animation.Draw(spriteBatch, _position, SpriteEffects.None);
        }
    }
}