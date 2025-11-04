using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace Lesson10_BrickerBreaker_OO;

public class ParticleSystem
    {
        private List<Particle> _particles = new List<Particle>();

        public void Update(GameTime gameTime)
        {
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

            for (int i = _particles.Count - 1; i >= 0; i--)
            {
                _particles[i].Update(dt);

                if (!_particles[i].Alive)
                {
                    _particles.RemoveAt(i);
                }
            }
        }

    public void Draw(SpriteBatch spriteBatch, Texture2D pixel)
    {
        for (int i = 0; i < _particles.Count; i++)
        {
            _particles[i].Draw(spriteBatch, pixel);
        }
    }
        
    public void SpawnExplosion(Vector2 position, Color color, int count = 20)
    {
        for (int i = 0; i < count; i++)
        {
            Particle p = new Particle();
            p.Initialize(position, color);
            _particles.Add(p);
        }
    }
}