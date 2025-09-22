using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Lesson06_BrickBreaker01;

public class BrickBreaker : Game
{
    private const int _WindowWidth = 800, _WindowHeight = 600;
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private Texture2D _pixel;

    //Paddle
    private Vector2 _paddlePosition;
    private const int _PaddleWidth = 120, _PaddleHeight = 18;
    private float _paddleSpeed = 400.0f;

    public BrickBreaker()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        _graphics.PreferredBackBufferWidth = _WindowWidth;
        _graphics.PreferredBackBufferHeight = _WindowHeight;
        _graphics.ApplyChanges();

        _paddlePosition = new Vector2((_WindowWidth - _PaddleWidth) / 2, _WindowHeight - 50);

        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        _pixel = new Texture2D(GraphicsDevice, 1, 1); //create 1-by-1 pixel Texture2D

        Color[] colours = new Color[1];
        colours[0] = Color.White;
        _pixel.SetData(colours);

        //or, we could just use _pixel.SetData(new[] { Color.White });
    }

    protected override void Update(GameTime gameTime)
    {

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);
        _spriteBatch.Begin();

        Rectangle paddleRectangle = new Rectangle((int)_paddlePosition.X, (int)_paddlePosition.Y, _PaddleWidth, _PaddleHeight);

        _spriteBatch.Draw(_pixel, paddleRectangle, Color.White);

        _spriteBatch.End();

        base.Draw(gameTime);
    }
}
