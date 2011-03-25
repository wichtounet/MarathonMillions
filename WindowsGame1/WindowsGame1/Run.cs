using System;
using System.Drawing;
using System.Windows.Forms;
using WiimoteLib;

namespace Marathon
{
    class Run : MiniGame
    {
        private const float ComputerSpeed = 3.0f;
        private const float StopLine = 320.0f;

        private Timer timer;

        private int fps;

        private float distance;
        private float speed;
        private int count;

        private float xComputer;
        private float xPlayer;
        
        private State state = State.Starting;
        private int startingTime;

        private enum State
        {
            Starting,
            Started,
            Finished
        }

        public Run(GamePanel panel, Wiimote wm) : base(panel, wm)
        {
            //Nothing to to do here
        }

        protected override void Initialize()
        {
            timer = new Timer { Interval = 100 };
            timer.Tick += TimerClock;
        }

        public override void Start()
        {
            fps = 0;
            speed = 0;
            count = 0;

            state = State.Starting;

            startingTime = 0;

            distance = 0;

            xComputer = 0;
            xPlayer = 0;

            timer.Start();
        }

        private void TimerClock(object sender, EventArgs e)
        {
            if(state == State.Starting)
            {
                startingTime += 1;

                if(startingTime == 30)
                {
                    state = State.Started;
                }
            } 
            else
            {
                xComputer += ComputerSpeed;
                xPlayer += speed;

                if (xComputer >= StopLine)
                {
                    timer.Stop();

                    state = State.Finished;

                    GamePanel.GameEnded(false);
                }
                else if (xPlayer >= StopLine)
                {
                    timer.Stop();

                    state = State.Finished;

                    GamePanel.GameEnded(true);
                }

                UpdateView();
            }
        }

        private static readonly Pen LinePen = new Pen(Color.Black, 2F);

        protected override void Draw()
        {
            /*if (state == State.Starting)
            {
                g.DrawString("" + (3 - startingTime / 10), new Font("Verdana", 20), new SolidBrush(Color.Black), Width / 2, Height / 2);
            }
            else if (state == State.Started)
            {
                g.DrawEllipse(LinePen, xComputer, Height / 4.0f, 25, 25);
                g.DrawRectangle(LinePen, xPlayer, 3 * Height / 4.0f, 25, 25);

                g.DrawLine(LinePen, 0, Height / 2, StopLine, Height / 2);
                g.DrawLine(LinePen, StopLine, 0, StopLine, Height);
            }
            else if (state == State.Finished)
            {
                if (xComputer >= StopLine)
                {
                    g.DrawString("Computer wins", new Font("Verdana", 20), new SolidBrush(Color.Black), Width / 3, Height / 2 - 50);
                }
                else if (xPlayer >= StopLine)
                {
                    g.DrawString("Player wins", new Font("Verdana", 20), new SolidBrush(Color.Black), Width / 3, Height / 2 - 50);
                }
            }*/
        }

        internal override void WiimoteChanged(WiimoteState ws)
        {
            var state = ws.AccelState;

            if(0 > state.Values.Y)
            {
                distance += 0 - state.Values.Y;
            } else
            {
                distance += state.Values.Y - 0;
            }

            if(count++ == 20)
            {
                speed = distance / 20;
                distance = 0;

                count = 0;
            }

            if (fps++ == 10)
            {
                fps = 0;

                UpdateView();
            }
        }
    }
}
