using System;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using WiimoteLib;
using ButtonState = WiimoteLib.ButtonState;
using Color = Microsoft.Xna.Framework.Color;

namespace Marathon
{
    class Buttons : MiniGame
    {
        private enum State
        {
            Started,
            Finished
        }

        private readonly Timer timer;

        private State state = State.Started;
        private bool ok;

        private SpriteBatch spriteBatch;
        private SpriteFont font;
        private ContentManager content;

        private ButtonState buttonState;
        private int sucess;

        private bool a;
        private bool b;
        private bool one;
        private bool two;
        private bool up;
        private bool down;
        private bool left;
        private bool right;

        private string message;
        
        public Buttons(GamePanel panel, Wiimote wm) : base(panel, wm)
        {
            timer = new Timer {Interval = 5000};
            timer.Tick += TimerClock;
        }

        protected override void Initialize()
        {
            content = new ContentManager(Services, "Content");
            font = content.Load<SpriteFont>("SpriteFont1");

            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
        }

        public override void Start()
        {
            sucess = 0;
            a = b = one = two = left = right = up = down = false;
            a = true;
            message = "A";

            state = State.Started;

            timer.Start();

            UpdateView();
        }

        public override void Stop()
        {
            timer.Stop();
        }

        private void TimerClock(object sender, EventArgs e)
        {
            if (a == buttonState.A && b == buttonState.B && one == buttonState.One && two == buttonState.Two
                && up == buttonState.Up && down == buttonState.Down && right == buttonState.Right && left == buttonState.Left)
            {
                sucess++;

                if(sucess == 5)
                {
                    ok = true;

                    timer.Stop();

                    state = State.Finished;

                    GamePanel.GameEnded(true);
                } 
                else
                {
                    var random = new Random().Next(8);

                    a = b = one = two = left = right = up = down = false;

                    switch (random)
                    {
                        case 0:
                            a = b = true;
                            message = "A + B";
                            break;
                        case 1:
                            b = true;
                            message = "B";
                            break;
                        case 2:
                            a = true;
                            message = "A";
                            break;
                        case 3:
                            one = two = true;
                            message = "1 + 2";
                            break;
                        case 4:
                            two = true;
                            message = "2";
                            break;
                        case 5:
                            down = b = true;
                            message = "DOWN + B";
                            break;
                        case 6:
                            up = b = true;
                            message = "UP + B";
                            break;
                        case 7:
                            down = true;
                            message = "DOWN";
                            break;
                    }
                }
            } 
            else
            {
                ok = false;

                timer.Stop();

                state = State.Finished;

                GamePanel.GameEnded(false);
            }
            
            UpdateView();
        }

        protected override void Draw()
        {
            GraphicsDevice.Clear(Color.White);

            spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.NonPremultiplied);
            
            switch (state)
            {
                case State.Started:
                    spriteBatch.DrawString(font, message, new Vector2(Width / 3, Height / 2 - 50), Color.Black);
                    break;
                case State.Finished:
                    spriteBatch.DrawString(font, ok ? "Congratulations" : "Game Over", new Vector2(Width / 3, Height/2 - 50), Color.Black);
                    break;
            }

            spriteBatch.End();
        }

        internal override void WiimoteChanged(WiimoteState ws)
        {
            buttonState = ws.ButtonState;
        }
    }
}
