using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Lesson09_Pong;

public class Pong : Game
{
    private const int _Scale = 4;

    private const int _WindowWidth = 250 * _Scale, _WindowHeight = 150 * _Scale;
    
    private const int _BallWidthAndHeight = 3 * _Scale;

    private const int _PaddleWidth = 2 * _Scale, _PaddleHeight = 18 * _Scale;

    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;

    private Rectangle _playAreaBoundingBox;

    private Ball[] _balls;

    private Vector2 _paddleDimensions, _paddlePosition, _paddleDirection;
    private float _paddleSpeed;

    // runtime 1x1 pixel for drawing primitives (no Content texture required)
    private Texture2D _pixel;

    public Pong()
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

        _playAreaBoundingBox = new Rectangle(0, 0, _WindowWidth, _WindowHeight);

        _balls = new Ball[4];
        for(int c = 0; c < 4; c++)
        {
            _balls[c] = new Ball();

            Vector2 ballPosition = new Vector2(c+50 * _Scale, c+65 * _Scale);
            float ballSpeed = 100 * _Scale;
            Vector2 ballDirection = new Vector2(-1, -1);
            Vector2 ballDimensions = new Vector2(_BallWidthAndHeight, _BallWidthAndHeight);
            _balls[c].Initialize(ballPosition, ballSpeed, ballDirection, ballDimensions, _playAreaBoundingBox);
        }

        _paddlePosition = new Vector2(210 * _Scale, 75 * _Scale);
        _paddleSpeed = 100 * _Scale;
        _paddleDimensions = new Vector2(_PaddleWidth, _PaddleHeight);

        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        // create a 1x1 white pixel we can stretch to draw rectangles
        _pixel = new Texture2D(GraphicsDevice, 1, 1);
        _pixel.SetData(new[] { Color.White });

        foreach (Ball ball in _balls)
        {
            ball.LoadContent(GraphicsDevice);
        }

    }

    protected override void Update(GameTime gameTime)
    {
        foreach (Ball ball in _balls)
        {
            ball.Update(gameTime);
        }

        #region paddle movement

        KeyboardState kbState = Keyboard.GetState();
        if (kbState.IsKeyDown(Keys.W))
        {
            _paddleDirection = new Vector2(0, -1);
        }
        else if (kbState.IsKeyDown(Keys.S))
        {
            _paddleDirection = new Vector2(0, 1);
        }
        else
        {
            _paddleDirection = new Vector2(0, 0);
        }
        _paddlePosition += _paddleDirection * _paddleSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;

        if (_paddlePosition.Y <= _playAreaBoundingBox.Top)
        {
            _paddlePosition.Y = _playAreaBoundingBox.Top;
        }
        else if ((_paddlePosition.Y + _paddleDimensions.Y) >= _playAreaBoundingBox.Bottom)
        {
            _paddlePosition.Y = _playAreaBoundingBox.Bottom - _paddleDimensions.Y;
        }

        #endregion
        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        _spriteBatch.Begin();

        foreach (Ball ball in _balls)
        {
            ball.Draw(_spriteBatch);
        }

        // draw paddle as a stretched 1x1 pixel
        Rectangle paddleRect = new Rectangle(
            (int)_paddlePosition.X,
            (int)_paddlePosition.Y,
            (int)_paddleDimensions.X,
            (int)_paddleDimensions.Y
        );
        _spriteBatch.Draw(_pixel, paddleRect, Color.White);

        _spriteBatch.End();

        base.Draw(gameTime);
    }
}