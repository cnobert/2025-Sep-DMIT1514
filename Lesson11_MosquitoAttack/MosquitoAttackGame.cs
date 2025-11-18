using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Lesson11_MosquitoAttack;

public class MosquitoAttackGame : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;

    private const int _WindowWidth = 550;
    private const int _WindowHeight = 400;

    private Texture2D _background;
    private Cannon _cannon;
    private Mosquito[] _mosquitoes;

    private Random _random;

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

        _random = new Random();

        _cannon = new Cannon();
        _cannon.Initialize(
            new Vector2(50, 325),
            new Rectangle(0, 0, _WindowWidth, _WindowHeight)
        );

        Rectangle gameBounds = new Rectangle(0, 0, _WindowWidth, _WindowHeight);

        _mosquitoes = new Mosquito[10];

        for (int i = 0; i < _mosquitoes.Length; i++)
        {
            Mosquito m = new Mosquito();

            float x = _random.Next(40, _WindowWidth - 47);
            float y = _random.Next(20, _WindowHeight - 200);
            Vector2 startPos = new Vector2(x, y);

            m.Initialize(startPos, gameBounds);

            float dirX = (float)(_random.NextDouble() * 2 - 1);
            float dirY = 0;
            Vector2 dir = new Vector2(dirX, dirY);

            dir.Normalize();
            
            m.Direction = dir;
            _mosquitoes[i] = m;
        }

        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        _background = Content.Load<Texture2D>("Background");
        _cannon.LoadContent(Content);

        foreach (Mosquito m in _mosquitoes)
        {
            m.LoadContent(Content);
        }
    }

    protected override void Update(GameTime gameTime)
    {
        KeyboardState kbState = Keyboard.GetState();

        if (kbState.IsKeyDown(Keys.A))
        {
            _cannon.Direction = new Vector2(-1, 0);
        }
        else if (kbState.IsKeyDown(Keys.D))
        {
            _cannon.Direction = new Vector2(1, 0);
        }
        else
        {
            _cannon.Direction = Vector2.Zero;
        }

        _cannon.Update(gameTime);

        foreach (Mosquito m in _mosquitoes)
        {
            m.Update(gameTime);
        }

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        _spriteBatch.Begin();
        _spriteBatch.Draw(_background, Vector2.Zero, Color.White);

        _cannon.Draw(_spriteBatch);

        foreach (Mosquito m in _mosquitoes)
        {
            m.Draw(_spriteBatch);
        }

        _spriteBatch.End();

        base.Draw(gameTime);
    }
}