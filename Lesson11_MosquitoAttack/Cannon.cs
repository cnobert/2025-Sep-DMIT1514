using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Diagnostics;

namespace Lesson11_MosquitoAttack;

public class Cannon
{
    private SimpleAnimation _animation;
    private Vector2 _position;

    internal void Initialize(Vector2 position)
    {
        _position = position;
    }
    internal void LoadContent(ContentManager content)
    {
        Texture2D texture = content.Load<Texture2D>("Cannon");
        _animation = new SimpleAnimation(texture, texture.Width / 4, texture.Height, 4, 8.0f);
        _animation.Paused = false;
    }
    internal void Update(GameTime gameTime)
    {
        
        _animation.Update(gameTime);
    }
    internal void Draw(SpriteBatch spriteBatch)
    {
        if(_animation != null)
        {
            _animation.Draw(spriteBatch, _position, SpriteEffects.None);
        }
    }
}