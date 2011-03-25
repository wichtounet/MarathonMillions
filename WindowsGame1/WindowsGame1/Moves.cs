using System;
using System.Drawing;
using System.Windows.Forms;
using WiimoteLib;

namespace Marathon
{
    class Moves : MiniGame
    {
        private readonly Timer timer;

        private int fps;

        private float lastY;
        private float lastX;
        private float lastZ;

        private float distanceX;
        private float distanceY;
        private float distanceZ;

        private float distanceX2;
        private float distanceY2;
        private float distanceZ2;
        
        private State state = State.Started;
        private bool ok;

        private enum State
        {
            Started,
            Finished
        }

        private enum Move
        {
            Up, 
            Down, 
            Left,
            Right
        }

        private int current;

        private Move[] moves = new[]
        {
            Move.Up,Move.Down, Move.Right, Move.Left, Move.Down, Move.Up
        };
        
        public Moves(GamePanel panel, Wiimote wm) : base(panel, wm)
        {
            Paint += PaintMove;

            timer = new Timer {Interval = 5000};
            timer.Tick += TimerClock;
        }

        public override void Start()
        {
            state = State.Started;

            timer.Start();
        }

        protected override void Initialize()
        {throw new NotImplementedException();
        }

        protected override void Draw()
        {
            throw new NotImplementedException();
        }

        private void TimerClock(object sender, EventArgs e)
        {
            Console.WriteLine("distance X " + distanceX);
            Console.WriteLine("distance Y " + distanceY);
            Console.WriteLine("distance Z " + distanceZ);

            Console.WriteLine("distance X 2 " + distanceX2);
            Console.WriteLine("distance Y 2 " + distanceY2);
            Console.WriteLine("distance Z 2 " + distanceZ2);

            distanceX = distanceY = distanceZ = 0;
            distanceX2 = distanceY2 = distanceZ2 = 0;

            switch (moves[current])
            {
                case Move.Up:
                    break;
                case Move.Down:
                    break;
                case Move.Left:
                    break;
                case Move.Right:
                    break;
            }

            current++;

            if(current == moves.Length - 1)
            {
                ok = true;

                timer.Stop();

                state = State.Finished;

                GamePanel.GameEnded(true);
            }
            
            Refresh();
        }

        private void PaintMove(object sender, PaintEventArgs e)
        {
            var g = e.Graphics;

            if(state == State.Started)
            {
                var currentMove = moves[current];

                switch (currentMove)
                {
                    case Move.Up:
                        g.DrawString("UP", new Font("Verdana", 20), new SolidBrush(Color.Black), Width / 3, Height / 2 - 50);
                        break;
                    case Move.Down:
                        g.DrawString("DOWN", new Font("Verdana", 20), new SolidBrush(Color.Black), Width / 3, Height / 2 - 50);
                        break;
                    case Move.Left:
                        g.DrawString("LEFT", new Font("Verdana", 20), new SolidBrush(Color.Black), Width / 3, Height / 2 - 50);
                        break;
                    case Move.Right:
                        g.DrawString("RIGHT", new Font("Verdana", 20), new SolidBrush(Color.Black), Width / 3, Height / 2 - 50);
                        break;
                }
            } 
            else if(state == State.Finished)
            {
                if (ok)
                {
                    g.DrawString("Game over", new Font("Verdana", 20), new SolidBrush(Color.Black), Width / 3, Height / 2 - 50);
                }
                else
                {
                    g.DrawString("Player wins", new Font("Verdana", 20), new SolidBrush(Color.Black), Width / 3, Height / 2 - 50);
                }
            }
        }

        internal override void WiimoteChanged(WiimoteState ws)
        {
            var state = ws.AccelState;

            distanceX2 += lastX - state.Values.X;
            distanceY2 += lastY - state.Values.Y;
            distanceZ2 += lastZ - state.Values.Z;

            distanceX += state.Values.X;
            distanceY += state.Values.Y;
            distanceZ += state.Values.Z;

            lastX = state.Values.X;
            lastY = state.Values.Y;
            lastZ = state.Values.Z;
        }
    }
}
