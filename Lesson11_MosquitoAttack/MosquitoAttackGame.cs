using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Lesson11_MosquitoAttack;

public class MosquitoAttackGame : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private const int _WindowWidth = 550;
    private const int _WindowHeight = 400;

    private Texture2D _background;
    private Cannon _cannon;

    public MosquitoAttackGame()
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
        _cannon = new Cannon();
        _cannon.Initialize(new Vector2(50, 325), new Rectangle(0, 0, _WindowWidth, _WindowHeight));
        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        _background = Content.Load<Texture2D>("Background");
        _cannon.LoadContent(Content);
    }

    protected override void Update(GameTime gameTime)
    {
        KeyboardState kbState = Keyboard.GetState();
        if(kbState.IsKeyDown(Keys.A))
        {
            _cannon.Direction = new Vector2(-1, 0);
        }
        else if(kbState.IsKeyDown(Keys.D))
        {
            _cannon.Direction = new Vector2(1, 0);
        }
        else
        {
            _cannon.Direction = Vector2.Zero;
        }

        _cannon.Update(gameTime);
        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        _spriteBatch.Begin();
        _spriteBatch.Draw(_background, Vector2.Zero, Color.White);
        _cannon.Draw(_spriteBatch);
        _spriteBatch.End();

        base.Draw(gameTime);
    }
}
