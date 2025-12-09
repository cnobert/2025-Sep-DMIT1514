using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Lesson13_Platforms;

public class PlatformerGame : Game
{
    internal const float _Gravity = 160;

    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;

    private const int _WindowWidth = 550;
    private const int _WindowHeight = 400;
    private Rectangle _gameBoundingBox = new Rectangle(0, 0, _WindowWidth, _WindowHeight);

    private Player _player;
    private Collider _ground;

    private Collider _colliderTop, _colliderRight, _colliderBottom, _colliderLeft;

    public PlatformerGame()
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

        _player = new Player(new Vector2(250, 50), _gameBoundingBox);
        _ground = new Collider(new Vector2(0, 300), new Vector2(_WindowWidth, 1), Collider.ColliderType.Top);

        //the top collider's top left corner is at 160, 270
        //the right collider needs to begin at 250,270 PLUS THE WIDTH OF THE TOP COLLIDER
        _colliderTop = new Collider(new Vector2(160, 230), new Vector2(80, 1), Collider.ColliderType.Top);
        _colliderRight = new Collider(new Vector2(250, 230), new Vector2(1, 20), Collider.ColliderType.Right);
        _colliderBottom = new Collider(new Vector2(160, 250), new Vector2(80, 1), Collider.ColliderType.Bottom);
        _colliderLeft = new Collider(new Vector2(150, 230), new Vector2(1, 20), Collider.ColliderType.Left);


        base.Initialize();

        _player.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        _player.LoadContent(Content);
        _ground.LoadContent(GraphicsDevice);
        _colliderTop.LoadContent(GraphicsDevice);
        _colliderRight.LoadContent(GraphicsDevice);
        _colliderBottom.LoadContent(GraphicsDevice);
        _colliderLeft.LoadContent(GraphicsDevice);
    }

    protected override void Update(GameTime gameTime)
    {
        float dt = (float) gameTime.ElapsedGameTime.TotalSeconds;
        KeyboardState kbState = Keyboard.GetState();
        if (kbState.IsKeyDown(Keys.Left))
        {
            _player.MoveHorizontally(-1);
        }
        else if (kbState.IsKeyDown(Keys.Right))
        {
            _player.MoveHorizontally(1);
        }
        else
        {
            _player.Stop();
        }
        if(kbState.IsKeyDown(Keys.Space))
        {
            _player.Jump();
        }

        
        _ground.ProcessCollision(_player, dt);
        _colliderTop.ProcessCollision(_player, dt);
        _colliderRight.ProcessCollision(_player, dt);
        _colliderBottom.ProcessCollision(_player, dt);
        _colliderLeft.ProcessCollision(_player, dt);

        _player.Update(gameTime);

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);
        _spriteBatch.Begin();
        _player.Draw(_spriteBatch);
        _ground.Draw(_spriteBatch);
        _colliderTop.Draw(_spriteBatch);
        _colliderRight.Draw(_spriteBatch);
        _colliderBottom.Draw(_spriteBatch);
        _colliderLeft.Draw(_spriteBatch);

        _spriteBatch.End();

        base.Draw(gameTime);
    }
}
