using System;
using System.Windows.Forms;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using WiimoteLib;
using Microsoft.Xna.Framework.Content;

namespace Marathon
{
    public class ClickMe : MiniGame
    {
        private int RectangleSize = 25;

        private readonly Random rnd = new Random();
        private readonly Timer timer;
        private readonly Timer viewTimer;

        private float x;
        private float y;
        private System.Drawing.PointF recPos = new System.Drawing.PointF(50, 50);

        private int time;
        private int win;

        private SpriteBatch spriteBatch;
        private Texture2D marmotteSprite;
        private Texture2D viseurSprite;
        private Texture2D background;
        private SpriteFont font;
        private SpriteFont font2;
        private SpriteFont smallFont;
        private ContentManager content;

        private int startingTime = 3;
        private State state = State.Starting;
        private enum State
        {
            Starting,
            Started,
            Won,
            Lost
        }

        public ClickMe(GamePanel panel, Wiimote wm) : base(panel, wm)
        {
            Wm.SetRumble(false);

            timer = new Timer { Interval = 1000 };
            timer.Tick += TimerClock;

            viewTimer = new Timer { Interval = 15 };
            viewTimer.Tick += ViewTimerClock;
        }

        protected override void Initialize()
        {
            
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // Load game content
            content = new ContentManager(Services, "Content");

            marmotteSprite = content.Load<Texture2D>("marmotte");
            viseurSprite = content.Load<Texture2D>("viseur");
            background = content.Load<Texture2D>("green_grass");
            font = content.Load<SpriteFont>("SpriteFont1");
            font2 = content.Load<SpriteFont>("SpriteFont2");
            smallFont = content.Load<SpriteFont>("SmallFont");
        }

        protected override void Draw()
        {
            GraphicsDevice.Clear(Color.Green);

            var t = new Texture2D(GraphicsDevice, 1, 1);
            t.SetData(new[] { Color.Red });

            spriteBatch.Begin(); //SpriteSortMode.BackToFront, BlendState.NonPremultiplied

            spriteBatch.Draw(background, new Rectangle(0,0, Width, Height), Color.White);

            if (state == State.Starting)
            {

                string sTmp = "" + startingTime;
                if (startingTime == 0) sTmp = "GO";
                spriteBatch.DrawString(font, sTmp, new Vector2(Width / 2, Height / 2) - (font.MeasureString(sTmp) / 2), Color.Black);

                sTmp = "Catch 10 marmots by shooting\nthem with the B button";
                spriteBatch.DrawString(font2, sTmp, new Vector2(Width / 2 - (font2.MeasureString(sTmp) / 2).X, 10), Color.Black);
            }
            else
            {
                if (timer.Enabled)
                {
                    RectangleSize = Width / 20;
                    spriteBatch.Draw(marmotteSprite, new Rectangle((int)recPos.X, (int)recPos.Y, RectangleSize, RectangleSize), Color.White);
                    string tmp = "Remaining time: " + time;
                    spriteBatch.DrawString(smallFont, tmp, new Vector2(Width - smallFont.MeasureString(tmp).X, 0), Color.Black);
                    tmp = "Remaining marmot: " + win;
                    spriteBatch.DrawString(smallFont, tmp, new Vector2(Width - smallFont.MeasureString(tmp).X, smallFont.MeasureString(tmp).Y), Color.Black);
                }
                else
                {
                    if (state == State.Won)
                    {
                        string text = "GAME WON!";
                        spriteBatch.DrawString(font, text, new Vector2(Width / 2, Height / 2) - (font.MeasureString(text) / 2), Color.Black);
                    }
                    else
                    {
                        string text = "GAME OVER!";
                        spriteBatch.DrawString(font, text, new Vector2(Width / 2, Height / 2) - (font.MeasureString(text) / 2), Color.Red);
                        Wm.SetRumble(false);
                    }
                }
            }

            spriteBatch.Draw(viseurSprite, new Rectangle((int)x - 10, (int)y - 10, 20, 20), Color.White);

            spriteBatch.End();
        }

        private void ViewTimerClock(object sender, EventArgs e)
        {
            Invalidate();
        }

        public override void Start()
        {
            time = 20;
            win = 10;
            startingTime = 3;

            state = State.Starting;

            x = 0;
            y = 0;

            recPos = new System.Drawing.PointF(50, 50);

            timer.Start();
        }

        public override void Stop()
        {
            timer.Stop();
            viewTimer.Stop();
            state = State.Lost;
            Console.WriteLine("Stop()");
        }

        private void TimerClock(object obj, EventArgs ea)
        {
            if (state == State.Starting)
            {
                if (startingTime == 0)
                {
                    state = State.Started;
                    viewTimer.Start();
                }
                startingTime--;
                UpdateView();
            }
            else
            {
                if (time == 0)
                {
                    state = State.Lost;
                    timer.Stop();
                    viewTimer.Stop();
                    Invalidate(true);
                    GamePanel.GameEnded(false);
                }
                if (state == State.Won)
                {
                    timer.Stop();
                    viewTimer.Stop();
                    Invalidate(true);
                    GamePanel.GameEnded(true);
                }

                if (Wm.WiimoteState.Rumble)
                {
                    Wm.SetRumble(false);
                }

                time--;
            }

        }

        internal override void WiimoteChanged(WiimoteState ws)
        {
            x = CalibrateX(ws.IRState.Midpoint.X);
            y = CalibrateY(ws.IRState.Midpoint.Y);

            if (timer.Enabled)
            {
                if (x >= recPos.X && x <= recPos.X + RectangleSize && y >= recPos.Y && y <= recPos.Y + RectangleSize && ws.ButtonState.B)
                {
                    win--;

                    if (win == 0)
                    {
                        state = State.Won;
                        Console.WriteLine("WON");
                    }
                    else
                    {
                        Wm.SetRumble(true);
                    }

                    var randx = (float)(rnd.NextDouble() * Width) - RectangleSize;
                    var randy = (float)(rnd.NextDouble() * Height)- RectangleSize;

                    recPos.X = randx > 0 ? randx : 0;
                    recPos.Y = randy > 0 ? randy : 0;
                }
            }
        }
    }
}
