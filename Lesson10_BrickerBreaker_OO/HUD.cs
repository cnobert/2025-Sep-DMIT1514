using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Lesson10_BrickerBreaker_OO;

public class HUD
{
    private Vector2 _position;
    private SpriteFont _font;
    private Color _colour;

    private int _bricksRemaining;

    internal int BricksRemaining { set => _bricksRemaining = value; }

    public void Initialize(Vector2 position, SpriteFont font, Color colour)
    {
        _position = position;
        _font = font;
        _colour = colour;
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.DrawString(_font, $"bricks left: {_bricksRemaining}", _position, _colour);
    }
}