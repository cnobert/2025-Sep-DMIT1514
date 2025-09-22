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

    //Ball
    private Vector2 _ballPosition;
    private const int _BallSize = 16;

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

        // Place ball above paddle
        _ballPosition = new Vector2(
            _paddlePosition.X + (_PaddleWidth / 2) - (_BallSize / 2),
            _paddlePosition.Y - _BallSize - 5
        );

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
        KeyboardState kb = Keyboard.GetState();
        float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
        if (kb.IsKeyDown(Keys.Left) || kb.IsKeyDown(Keys.A))
        {
            _paddlePosition.X -= _paddleSpeed * deltaTime;
        }
        if (kb.IsKeyDown(Keys.Right) || kb.IsKeyDown(Keys.D))
        {
            _paddlePosition.X += _paddleSpeed * deltaTime;
        }
        //fix any screen overrunds (pin to one side or the other if we're exiting the screen)
        //right side
        if (_paddlePosition.X > _WindowWidth - _PaddleWidth)
        {
            _paddlePosition.X = _WindowWidth - _PaddleWidth;
        }
        //left side
        if (_paddlePosition.X < 0.0f)
        {
            _paddlePosition.X = 0.0f;
        }

        //keep ball resting above paddle
        _ballPosition.X = _paddlePosition.X + (_PaddleWidth / 2) - (_BallSize / 2);
        _ballPosition.Y = _paddlePosition.Y - _BallSize - 5;

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);
        _spriteBatch.Begin();

        Rectangle paddleRectangle = new Rectangle((int)_paddlePosition.X, (int)_paddlePosition.Y, _PaddleWidth, _PaddleHeight);
        Rectangle ballRectangle = new Rectangle((int)_ballPosition.X, (int)_ballPosition.Y, _BallSize, _BallSize);
        _spriteBatch.Draw(_pixel, paddleRectangle, Color.Brown);
        _spriteBatch.Draw(_pixel, ballRectangle, Color.OrangeRed);

        _spriteBatch.End();

        base.Draw(gameTime);
    }
}
