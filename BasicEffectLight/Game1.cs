using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace BasicEffectLight
{
	/// <summary>
	/// ゲームメインクラス
	/// </summary>
	public class Game1 : Game
	{
    /// <summary>
    /// ライトパラメータ
    /// </summary>
    public class LightParameter
    {
      /// <summary>
      /// ライト有効フラグ
      /// </summary>
      public bool Enabled;

      /// <summary>
      /// ディフューズカラー
      /// </summary>
      public Vector3 DiffuseColor;

      /// <summary>
      /// スペキュラーカラー
      /// </summary>
      public Vector3 SpecularColor;

      /// <summary>
      /// ライトの方向
      /// </summary>
      public Vector3 Direction;
    }

    /// <summary>
    /// パラメータの最大数
    /// </summary>
    private static int MaxParameterCount = 15;

    /// <summary>
    /// メニューリスト
    /// </summary>
    private static string[] MenuNameList = new string[]
        {
                "LightIndex",
                "Light Enable",
                "DiffuseColor (Red)",
                "DiffuseColor (Green)",
                "DiffuseColor (Blue)",
                "SpecularColor (Red)",
                "SpecularColor (Green)",
                "SpecularColor (Blue)",
                "Direction X",
                "Direction Y",
                "Direction Z",
                "AmbientLightColor (Red)",
                "AmbientLightColor (Green)",
                "AmbientLightColor (Blue)",
                "PreferPerPixelLighting",
        };

    /// <summary>
    /// グラフィックデバイス管理クラス
    /// </summary>
    private readonly GraphicsDeviceManager _graphics = null;

    /// <summary>
    /// スプライトのバッチ化クラス
    /// </summary>
    private SpriteBatch _spriteBatch = null;

    /// <summary>
    /// スプライトでテキストを描画するためのフォント
    /// </summary>
    private SpriteFont _font = null;

    /// <summary>
    /// 直前のキーボード入力の状態
    /// </summary>
    private KeyboardState _oldKeyboardState = new KeyboardState();

    /// <summary>
    /// 直前のマウスの状態
    /// </summary>
    private MouseState _oldMouseState = new MouseState();

    /// <summary>
    /// 直前のゲームパッド入力の状態
    /// </summary>
    private GamePadState _oldGamePadState = new GamePadState();

    /// <summary>
    /// モデル
    /// </summary>
    private Model _model = null;

    /// <summary>
    /// 選択しているメニューのインデックス
    /// </summary>
    private int _selectedMenuIndex = 0;

    /// <summary>
    /// 選択しているライトのインデックス
    /// </summary>
    private int _selectedLightIndex = 0;

    /// <summary>
    /// ライトのパラメータ
    /// </summary>
    private LightParameter[] _lightParameters = new LightParameter[]
        {
                new LightParameter(),
                new LightParameter(),
                new LightParameter()
        };

    /// <summary>
    /// アンビエントカラー
    /// </summary>
    private Vector3 _ambientLightColor = Vector3.Zero;

    /// <summary>
    /// ピクセル単位のライティング
    /// </summary>
    private bool _isPreferPerPixelLighting = false;


    /// <summary>
    /// パラメータテキストリスト
    /// </summary>
    private string[] _parameters = new string[MaxParameterCount];


    /// <summary>
    /// GameMain コンストラクタ
    /// </summary>
    public Game1()
    {
      // グラフィックデバイス管理クラスの作成
      _graphics = new GraphicsDeviceManager(this);

      // ゲームコンテンツのルートディレクトリを設定
      Content.RootDirectory = "Content";

      // ウインドウ上でマウスのポインタを表示するようにする
      IsMouseVisible = true;
    }

    /// <summary>
    /// ゲームが始まる前の初期化処理を行うメソッド
    /// グラフィック以外のデータの読み込み、コンポーネントの初期化を行う
    /// </summary>
    protected override void Initialize()
    {
      // TODO: ここに初期化ロジックを書いてください

      // コンポーネントの初期化などを行います
      base.Initialize();
    }

    /// <summary>
    /// ゲームが始まるときに一回だけ呼ばれ
    /// すべてのゲームコンテンツを読み込みます
    /// </summary>
    protected override void LoadContent()
    {
      // テクスチャーを描画するためのスプライトバッチクラスを作成します
      _spriteBatch = new SpriteBatch(GraphicsDevice);

      // フォントをコンテンツパイプラインから読み込む
      _font = Content.Load<SpriteFont>("Font");

      // モデルを作成
      _model = Content.Load<Model>("Model");

      // ライトとビュー、プロジェクションはあらかじめ設定しておく
      foreach (ModelMesh mesh in _model.Meshes)
      {
        foreach (BasicEffect effect in mesh.Effects)
        {
          // デフォルトのライト適用
          effect.EnableDefaultLighting();

          // ビューマトリックスをあらかじめ設定 ((0, 0, 8) から原点を見る)
          effect.View = Matrix.CreateLookAt(
              new Vector3(0.0f, 0.0f, 8.0f),
              Vector3.Zero,
              Vector3.Up
          );

          // プロジェクションマトリックスをあらかじめ設定
          effect.Projection = Matrix.CreatePerspectiveFieldOfView(
              MathHelper.ToRadians(45.0f),
              (float)GraphicsDevice.Viewport.Width /
                  (float)GraphicsDevice.Viewport.Height,
              1.0f,
              100.0f
          );

          // デフォルトのパラメータを受け取る
          _lightParameters[0].Enabled = effect.DirectionalLight0.Enabled;
          _lightParameters[0].DiffuseColor = effect.DirectionalLight0.DiffuseColor;
          _lightParameters[0].SpecularColor = effect.DirectionalLight0.SpecularColor;
          _lightParameters[0].Direction = effect.DirectionalLight0.Direction;

          _lightParameters[1].Enabled = effect.DirectionalLight1.Enabled;
          _lightParameters[1].DiffuseColor = effect.DirectionalLight1.DiffuseColor;
          _lightParameters[1].SpecularColor = effect.DirectionalLight1.SpecularColor;
          _lightParameters[1].Direction = effect.DirectionalLight1.Direction;

          _lightParameters[2].Enabled = effect.DirectionalLight2.Enabled;
          _lightParameters[2].DiffuseColor = effect.DirectionalLight2.DiffuseColor;
          _lightParameters[2].SpecularColor = effect.DirectionalLight2.SpecularColor;
          _lightParameters[2].Direction = effect.DirectionalLight2.Direction;

          // アンビエントカラー
          _ambientLightColor = effect.AmbientLightColor;

          // ピクセル単位のライティング
          _isPreferPerPixelLighting = effect.PreferPerPixelLighting;
        }
      }
    }

    /// <summary>
    /// ゲームが終了するときに一回だけ呼ばれ
    /// すべてのゲームコンテンツをアンロードします
    /// </summary>
    protected override void UnloadContent()
    {
      // TODO: ContentManager で管理されていないコンテンツを
      //       ここでアンロードしてください
    }

    /// <summary>
    /// 描画以外のデータ更新等の処理を行うメソッド
    /// 主に入力処理、衝突判定などの物理計算、オーディオの再生など
    /// </summary>
    /// <param name="gameTime">このメソッドが呼ばれたときのゲーム時間</param>
    protected override void Update(GameTime gameTime)
    {
      // 入力デバイスの状態取得
      KeyboardState keyboardState = Keyboard.GetState();
      MouseState mouseState = Mouse.GetState();
      GamePadState gamePadState = GamePad.GetState(PlayerIndex.One);

      // ゲームパッドの Back ボタン、またはキーボードの Esc キーを押したときにゲームを終了させます
      if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
      {
        Exit();
      }

      // メニューの選択
      if ((keyboardState.IsKeyDown(Keys.Up) && _oldKeyboardState.IsKeyUp(Keys.Up)) ||
          (gamePadState.ThumbSticks.Left.Y >= 0.5f &&
              _oldGamePadState.ThumbSticks.Left.Y < 0.5f))
      {
        // 選択メニューをひとつ上に移動
        _selectedMenuIndex =
            (_selectedMenuIndex + _parameters.Length - 1) % _parameters.Length;
      }
      if ((keyboardState.IsKeyDown(Keys.Down) && _oldKeyboardState.IsKeyUp(Keys.Down)) ||
          (gamePadState.ThumbSticks.Left.Y <= -0.5f &&
              _oldGamePadState.ThumbSticks.Left.Y > -0.5f) ||
          (_oldMouseState.LeftButton == ButtonState.Pressed &&
           mouseState.LeftButton == ButtonState.Released))
      {
        // 選択メニューをひとつ下に移動
        _selectedMenuIndex =
            (_selectedMenuIndex + _parameters.Length + 1) % _parameters.Length;
      }

      // 各マテリアルの値を操作
      float moveValue = 0.0f;
      if (keyboardState.IsKeyDown(Keys.Left))
      {
        moveValue -= (float)gameTime.ElapsedGameTime.TotalSeconds;
      }
      if (keyboardState.IsKeyDown(Keys.Right))
      {
        moveValue += (float)gameTime.ElapsedGameTime.TotalSeconds;
      }
      if (mouseState.LeftButton == ButtonState.Pressed)
      {
        moveValue += (mouseState.X - _oldMouseState.X) * 0.005f;
      }
      if (gamePadState.IsConnected)
      {
        moveValue += gamePadState.ThumbSticks.Left.X *
                     (float)gameTime.ElapsedGameTime.TotalSeconds;
      }

      if (moveValue != 0.0f)
      {
        LightParameter selectedLight = _lightParameters[_selectedLightIndex];

        switch (_selectedMenuIndex)
        {
        case 0:
          // ライトのインデックス
          if ((keyboardState.IsKeyDown(Keys.Left) &&
                  _oldKeyboardState.IsKeyUp(Keys.Left)) ||
              (mouseState.LeftButton == ButtonState.Pressed &&
                  (mouseState.X - _oldMouseState.X) >= 5) ||
              (gamePadState.ThumbSticks.Left.X >= 0.5f &&
                  _oldGamePadState.ThumbSticks.Left.X < 0.5f))
          {
            // ライトのインデックスをひとつ減らす
            _selectedLightIndex =
                (_selectedLightIndex + _lightParameters.Length - 1) %
                    _lightParameters.Length;
          }
          if ((keyboardState.IsKeyDown(Keys.Right) &&
                  _oldKeyboardState.IsKeyUp(Keys.Right)) ||
              (mouseState.LeftButton == ButtonState.Pressed &&
                  (mouseState.X - _oldMouseState.X) <= -5) ||
              (gamePadState.ThumbSticks.Left.X <= -0.5f &&
                  _oldGamePadState.ThumbSticks.Left.X > -0.5f))
          {
            // ライトのインデックスをひとつ増やす
            _selectedLightIndex =
                (_selectedLightIndex + _lightParameters.Length + 1) %
                    _lightParameters.Length;
          }
          if (mouseState.LeftButton == ButtonState.Pressed)
          {
            _selectedLightIndex = (int)(
                MathHelper.Clamp((float)mouseState.X / GraphicsDevice.Viewport.Width * 3, 0, 2));
          }
          break;
        case 1:
          // ライト有効フラグ
          selectedLight.Enabled = (moveValue > 0.0f);
          break;
        case 2:
          // ライトのディフーズカラー(赤)
          Vector3 diffuseX = selectedLight.DiffuseColor;
          diffuseX.X = MathHelper.Clamp(diffuseX.X + moveValue, 0.0f, 1.0f);
          selectedLight.DiffuseColor = diffuseX;
          break;
        case 3:
          // ライトのディフーズカラー(緑)
          Vector3 diffuseY = selectedLight.DiffuseColor;
          diffuseY.Y = MathHelper.Clamp(diffuseY.Y + moveValue, 0.0f, 1.0f);
          selectedLight.DiffuseColor = diffuseY;
          break;
        case 4:
          // ライトのディフーズカラー(青)
          Vector3 diffuseZ = selectedLight.DiffuseColor;
          diffuseZ.Z = MathHelper.Clamp(diffuseZ.Z + moveValue, 0.0f, 1.0f);
          selectedLight.DiffuseColor = diffuseZ;
          break;
        case 5:
          // ライトのスペキュラーカラー(赤)
          Vector3 specularX = selectedLight.SpecularColor;
          specularX.X = MathHelper.Clamp(specularX.X + moveValue, 0.0f, 1.0f);
          selectedLight.SpecularColor = specularX;
          break;
        case 6:
          // ライトのスペキュラーカラー(緑)
          Vector3 specularY = selectedLight.SpecularColor;
          specularY.Y = MathHelper.Clamp(specularY.Y + moveValue, 0.0f, 1.0f);
          selectedLight.SpecularColor = specularY;
          break;
        case 7:
          // ライトのスペキュラーカラー(青)
          Vector3 specularZ = selectedLight.SpecularColor;
          specularZ.Z = MathHelper.Clamp(specularZ.Z + moveValue, 0.0f, 1.0f);
          selectedLight.SpecularColor = specularZ;
          break;
        case 8:
          // ライトの方向X
          Vector3 directionX = selectedLight.Direction;
          directionX.X += moveValue;
          selectedLight.Direction = directionX;
          break;
        case 9:
          // ライトの方向Y
          Vector3 directionY = selectedLight.Direction;
          directionY.Y += moveValue;
          selectedLight.Direction = directionY;
          break;
        case 10:
          // ライトの方向Z
          Vector3 directionZ = selectedLight.Direction;
          directionZ.Z += moveValue;
          selectedLight.Direction = directionZ;
          break;
        case 11:
          // アンビエントカラー(赤)
          _ambientLightColor.X =
              MathHelper.Clamp(_ambientLightColor.X + moveValue, 0.0f, 1.0f);
          break;
        case 12:
          // アンビエントカラー(緑)
          _ambientLightColor.Y =
              MathHelper.Clamp(_ambientLightColor.Y + moveValue, 0.0f, 1.0f);
          break;
        case 13:
          // アンビエントカラー(青)
          _ambientLightColor.Z =
              MathHelper.Clamp(_ambientLightColor.Z + moveValue, 0.0f, 1.0f);
          break;
        case 14:
          // ピクセル単位のライティング
          _isPreferPerPixelLighting = (moveValue > 0.0f);
          break;
        }
      }

      // ライトを設定
      foreach (ModelMesh mesh in _model.Meshes)
      {
        foreach (BasicEffect effect in mesh.Effects)
        {
          for (int i = 0; i < _lightParameters.Length; i++)
          {
            // ライトを取得
            DirectionalLight light = null;

            switch (i)
            {
            case 0:
              light = effect.DirectionalLight0;
              break;
            case 1:
              light = effect.DirectionalLight1;
              break;
            case 2:
              light = effect.DirectionalLight2;
              break;
            }

            // ライト有効フラグ
            light.Enabled = _lightParameters[i].Enabled;

            // ライトのディフーズカラー
            light.DiffuseColor = _lightParameters[i].DiffuseColor;

            // ライトのスペキュラーカラー
            light.SpecularColor = _lightParameters[i].SpecularColor;

            // ライトの方向
            light.Direction = _lightParameters[i].Direction;
          }

          // アンビエントカラー
          effect.AmbientLightColor = _ambientLightColor;

          // ピクセル単位のライティング
          effect.PreferPerPixelLighting = _isPreferPerPixelLighting;

        }
      }

      // 入力情報を記憶
      _oldKeyboardState = keyboardState;
      _oldMouseState = mouseState;
      _oldGamePadState = gamePadState;

      // 登録された GameComponent を更新する
      base.Update(gameTime);
    }

    /// <summary>
    /// 描画処理を行うメソッド
    /// </summary>
    /// <param name="gameTime">このメソッドが呼ばれたときのゲーム時間</param>
    protected override void Draw(GameTime gameTime)
    {
      // 画面を指定した色でクリアします
      GraphicsDevice.Clear(Color.CornflowerBlue);

      // 深度バッファを有効にする
      GraphicsDevice.DepthStencilState = DepthStencilState.Default;

      // モデルを描画
      foreach (ModelMesh mesh in _model.Meshes)
      {
        mesh.Draw();
      }

      // スプライトの描画準備
      _spriteBatch.Begin();

      // 操作
      _spriteBatch.DrawString(_font,
          "Up, Down : Select Menu",
          new Vector2(20.0f, 20.0f), Color.White);
      _spriteBatch.DrawString(_font,
          "Left, right : Change Value",
          new Vector2(20.0f, 45.0f), Color.White);
      _spriteBatch.DrawString(_font,
          "MouseClick & Drag :",
          new Vector2(20.0f, 70.0f), Color.White);
      _spriteBatch.DrawString(_font,
          "    Select Menu & Change Value",
          new Vector2(20.0f, 95.0f), Color.White);

      // 各メニュー //
      for (int i = 0; i < MenuNameList.Length; i++)
      {
        _spriteBatch.DrawString(_font,
            MenuNameList[i],
            new Vector2(40.0f, 120.0f + i * 20.0f), Color.White);
      }

      // 各パラメータ //

      LightParameter selectedLight = _lightParameters[_selectedLightIndex];

      // ライトのインデックス
      _parameters[0] = _selectedLightIndex.ToString();

      // ライトの有効フラグ
      _parameters[1] = selectedLight.Enabled.ToString();

      // ライトのディフーズカラー(赤)
      _parameters[2] = selectedLight.DiffuseColor.X.ToString();

      // ライトのディフーズカラー(緑)
      _parameters[3] = selectedLight.DiffuseColor.Y.ToString();

      // ライトのディフーズカラー(青)
      _parameters[4] = selectedLight.DiffuseColor.Z.ToString();

      // ライトのスペキュラーカラー(赤)
      _parameters[5] = selectedLight.SpecularColor.X.ToString();

      // ライトのスペキュラーカラー(緑)
      _parameters[6] = selectedLight.SpecularColor.Y.ToString();

      // ライトのスペキュラーカラー(青)
      _parameters[7] = selectedLight.SpecularColor.Z.ToString();

      // ライトの方向X
      _parameters[8] = selectedLight.Direction.X.ToString();

      // ライトの方向Y
      _parameters[9] = selectedLight.Direction.Y.ToString();

      // ライトの方向Z
      _parameters[10] = selectedLight.Direction.Z.ToString();

      // アンビエントカラー(赤)
      _parameters[11] = _ambientLightColor.X.ToString();

      // アンビエントカラー(緑)
      _parameters[12] = _ambientLightColor.Y.ToString();

      // アンビエントカラー(青)
      _parameters[13] = _ambientLightColor.Z.ToString();

      // ピクセル単位のライティング
      _parameters[14] = _isPreferPerPixelLighting.ToString();


      for (int i = 0; i < _parameters.Length; i++)
      {
        _spriteBatch.DrawString(_font,
            _parameters[i],
            new Vector2(300.0f, 120.0f + i * 20.0f), Color.White);
      }

      // 選択インデックス
      _spriteBatch.DrawString(_font, "*",
          new Vector2(20.0f, 124.0f + _selectedMenuIndex * 20.0f), Color.White);

      // スプライトの一括描画
      _spriteBatch.End();

      // 登録された DrawableGameComponent を描画する
      base.Draw(gameTime);
    }
  }
}
