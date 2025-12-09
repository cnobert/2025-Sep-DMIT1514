using System;
using System.IO.Compression;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Lesson13_Platforms;

public class Collider
{
    public enum ColliderType
    {
        Top, Right, Bottom, Left
    }
    private ColliderType _type;
    private Vector2 _position, _dimensions;
    private Texture2D _pixel;
    internal Rectangle BoundingBox
    {
        get
        {
            return new Rectangle((int)_position.X, (int)_position.Y, (int)_dimensions.X, (int)_dimensions.Y);
        }
    }
    public Collider(Vector2 position, Vector2 dimensions, ColliderType colliderType)
    {
        _position = position;
        _dimensions = dimensions;
        _type = colliderType;
    }
    internal void LoadContent(GraphicsDevice graphicsDevice)
    {
        Color myColour = Color.White;
        switch(_type)
        {
            case ColliderType.Left:
                myColour = Color.Red;
                break;
            case ColliderType.Right:
                myColour = Color.Blue;
                break;
            case ColliderType.Top:
                myColour = Color.Chocolate;
                break;
            case ColliderType.Bottom:
                myColour = Color.Purple;
                break;
        }
        if (_pixel == null)
        {
            _pixel = new Texture2D(graphicsDevice, 1, 1);
            _pixel.SetData(new[] { myColour });
        }
    }
    internal void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(_pixel, BoundingBox, Color.White);
    }
    internal bool ProcessCollision(Player player, float dt)
    {
        bool didCollide = false;
        if(BoundingBox.Intersects(player.BoundingBox)) 
        {
            didCollide = true;
            switch (_type)
            {
                case ColliderType.Left:
                    //is the player moving rightwards?
                    if(player.Velocity.X > 0)
                    {
                        player.MoveHorizontally(0);
                    }
                    break;
                case ColliderType.Right:
                    //is the player moving leftwards?
                    if(player.Velocity.X < 0)
                    {
                        player.MoveHorizontally(0);
                    }
                    break;
                case ColliderType.Top:
                    player.Land(BoundingBox);
                    player.StandOn(BoundingBox, dt);
                    break;
                case ColliderType.Bottom:
                    if(player.Velocity.Y < 0)
                    {
                        player.MoveVertically(0);
                    }
                    break;
            }
        }
        return didCollide;
    }
}