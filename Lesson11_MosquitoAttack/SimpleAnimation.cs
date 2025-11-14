using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Lesson11_MosquitoAttack;
/// <summary>
/// Handles simple sprite sheet animations by cycling through frames over time.
/// </summary>
public class SimpleAnimation
{
    private readonly Texture2D _texture;
    private readonly List<Rectangle> _frames;
    private readonly float _timePerFrame;

    private float _timer;
    private int _frameIndex;

    /// <summary>
    /// Gets or sets whether the animation should loop when it reaches the end.
    /// </summary>
    public bool Looping { get; set; } = true;

    /// <summary>
    /// Gets or sets whether the animation is paused.
    /// </summary>
    public bool Paused { get; set; } = false;

    /// <summary>
    /// Creates a new SimpleAnimation.
    /// </summary>
    /// <param name="texture">The sprite sheet texture.</param>
    /// <param name="frameWidth">Width of each frame in pixels.</param>
    /// <param name="frameHeight">Height of each frame in pixels.</param>
    /// <param name="frameCount">Total number of frames in the animation.</param>
    /// <param name="framesPerSecond">Playback speed in frames per second.</param>
    public SimpleAnimation(Texture2D texture, int frameWidth, int frameHeight, int frameCount, float framesPerSecond)
    {
        _texture = texture;
        _frames = new List<Rectangle>();
        for (int i = 0; i < frameCount; i++)
        {
            _frames.Add(new Rectangle(i * frameWidth, 0, frameWidth, frameHeight));
        }

        _timePerFrame = 1f / framesPerSecond;
    }

    /// <summary>
    /// Updates the animation based on elapsed game time.
    /// Call this once per frame inside your Update method.
    /// </summary>
    /// <param name="gameTime">GameTime from MonoGame's Update method.</param>
    public void Update(GameTime gameTime)
    {
        bool shouldAdvance = !Paused;
        
        if (shouldAdvance)
        {
            
            _timer += (float)gameTime.ElapsedGameTime.TotalSeconds;
            Console.WriteLine(_timePerFrame);
            if (_timer >= _timePerFrame)
            {
                _timer -= _timePerFrame;
                _frameIndex++;

                if (_frameIndex >= _frames.Count)
                {
                    if (Looping)
                    {
                        _frameIndex = 0;
                    }
                    else
                    {
                        _frameIndex = _frames.Count - 1;
                    }
                }
            }
        }
    }

    /// <summary>
    /// Draws the current frame of the animation at the given position, 
    /// allowing optional flipping with SpriteEffects.
    /// </summary>
    /// <param name="spriteBatch">The SpriteBatch used for drawing.</param>
    /// <param name="position">The position on screen to draw the frame.</param>
    /// <param name="effects">SpriteEffects to apply (e.g., flip horizontally or vertically).</param>
    public void Draw(SpriteBatch spriteBatch, Vector2 position, SpriteEffects effects)
    {
        spriteBatch.Draw(_texture, position, _frames[_frameIndex], Color.White, 0f, Vector2.Zero, 1f, effects, 0f);
    }
}