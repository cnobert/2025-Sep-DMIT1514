using System;
using System.IO.Compression;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Lesson13_Platforms;

internal class Platform
{
    private Texture2D _texture;
    private string _textureName;

    private Vector2 _position;
    private Vector2 _dimensions;

    private Collider _colliderTop;
    private Collider _colliderRight;
    private Collider _colliderBottom;
    private Collider _colliderLeft;

    public Platform(Vector2 position, Vector2 dimensions, string textureName)
    {
        _textureName = textureName;
        _colliderTop = new Collider(new Vector2(position.X + 3, position.Y), new Vector2(dimensions.X - 6, 1), Collider.ColliderType.Top);
        _colliderRight = new Collider(new Vector2(position.X + dimensions.X - 1, position.Y + 1), new Vector2(1, dimensions.Y - 2), Collider.ColliderType.Right);
        _colliderBottom = new Collider(new Vector2(position.X + 3, position.Y + dimensions.Y), new Vector2(dimensions.X - 6, 1), Collider.ColliderType.Bottom);
        _colliderLeft = new Collider(new Vector2(position.X + 1, position.Y + 1), new Vector2(1, dimensions.Y - 2), Collider.ColliderType.Left);
    }
    internal void LoadContent(GraphicsDevice graphicsDevice)
    {
        _colliderTop.LoadContent(graphicsDevice);
        _colliderRight.LoadContent(graphicsDevice);
        _colliderBottom.LoadContent(graphicsDevice);
        _colliderLeft.LoadContent(graphicsDevice);
    }
    internal void Draw(SpriteBatch spriteBatch)
    {
        
        _colliderTop.Draw(spriteBatch);
        _colliderRight.Draw(spriteBatch);
        _colliderBottom.Draw(spriteBatch);
        _colliderLeft.Draw(spriteBatch);
    }
    internal void ProcessCollisions(Player player, float dt)
    {
        _colliderTop.ProcessCollision(player, dt);
        _colliderRight.ProcessCollision(player, dt);
        _colliderBottom.ProcessCollision(player, dt);
        _colliderLeft.ProcessCollision(player, dt);
    }
}