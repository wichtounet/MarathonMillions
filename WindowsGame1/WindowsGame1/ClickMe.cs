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

        private int startingTime = 3;
        private State state = State.Starting;
        private enum State
        {
            Starting,
            Started,
            Won,
            Lost
        }
        

        SpriteBatch spriteBatch;
        Texture2D marmotteSprite;
        Texture2D viseurSprite;
        Texture2D background;
        SpriteFont font;
        ContentManager content;

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
            content = new ContentManager(Services, "Content");
            font = content.Load<SpriteFont>("SpriteFont1");
            
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // Load game content
            content = new ContentManager(Services, "Content");

            marmotteSprite = content.Load<Texture2D>("marmotte");
            viseurSprite = content.Load<Texture2D>("viseur");
            background = content.Load<Texture2D>("green_grass"); 

            font = content.Load<SpriteFont>("SpriteFont1");
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
                string sTime = "" + startingTime;
                spriteBatch.DrawString(font, sTime, new Vector2(Width / 2, Height / 2) - (font.MeasureString(sTime) / 2), Color.Black);
            }
            else
            {
                if (timer.Enabled)
                {
                    RectangleSize = Width / 20;
                    spriteBatch.Draw(marmotteSprite, new Rectangle((int)recPos.X, (int)recPos.Y, RectangleSize, RectangleSize), Color.White);
                    string sTime = "" + time;
                    spriteBatch.DrawString(font, sTime, new Vector2(Width - font.MeasureString(sTime).X, font.MeasureString(sTime).Y), Color.Black);
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
            win = 0;

            x = 0;
            y = 0;

            recPos = new System.Drawing.PointF(50, 50);

            timer.Start();
        }

        public override void Stop()
        {
            timer.Stop();
            viewTimer.Stop();
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
                if (state == State.Lost)
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
                    win++;

                    if (win == 10)
                    {
                        state = State.Won;
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
