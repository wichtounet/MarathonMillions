using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WiimoteLib;


namespace Marathon
{
    public class Labyrinthe : MiniGame
    {
        private static readonly Brush WallBrush = new SolidBrush(Color.Black);
        private static readonly Brush EllipseBrush = new SolidBrush(Color.BlueViolet);
        private static readonly Brush EndBrush = new SolidBrush(Color.Red);
        private static readonly Pen EllipsePen = new Pen(Color.Blue, 2F);

        private float x, y;
        private int fps;
        private int state;

        enum S{
            Wall, Empty, Start, End
        };

        private readonly S[][] matrice = new []{ 
            new[]{S.Wall , S.Wall , S.Wall , S.Wall , S.Empty, S.Wall , S.Empty, S.Empty,S.Empty}, 
            new[]{S.End  , S.Empty, S.Empty, S.Wall , S.Empty, S.Wall , S.Wall , S.Wall ,S.Empty}, 
            new[]{S.Wall , S.Wall , S.Empty, S.Wall , S.Empty, S.Empty, S.Empty, S.Empty,S.Empty},
            new[]{S.Empty, S.Empty, S.Empty, S.Wall , S.Empty, S.Wall , S.Wall , S.Wall ,S.Wall },
            new[]{S.Empty, S.Wall , S.Wall , S.Wall , S.Empty, S.Wall , S.Start, S.Wall ,S.Empty},
            new[]{S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Wall , S.Empty, S.Wall ,S.Empty},
            new[]{S.Wall , S.Empty, S.Wall , S.Wall , S.Wall , S.Wall , S.Empty, S.Wall ,S.Empty},
            new[]{S.Empty, S.Empty, S.Wall , S.Empty, S.Empty, S.Empty, S.Empty, S.Wall ,S.Empty},
            new[]{S.Empty, S.Empty, S.Empty, S.Empty, S.Wall , S.Empty, S.Wall , S.Wall ,S.Empty},

        };
        
        public Labyrinthe(GamePanel panel, Wiimote wm) : base(panel, wm)
        {
            Paint += PaintLabyrinth;
        }

        protected override void Initialize()
        {
            
        }

        protected override void Draw()
        {
            throw new NotImplementedException();
        }

        public override void Start()
        {
            x = 0; 
            y = 0; 
            fps = 0; 
            state = 0; 
        }

        private void PaintLabyrinth(object sender, PaintEventArgs e)
        {
            var g = e.Graphics;

            var width = Width/ matrice[0].Length;
            var height = Height/ matrice.Length;

            for (var i = 0; i < matrice.Length; i++)
            {
                for (var j = 0; j < matrice[i].Length; j++)
                {
                    var xPos = width * j;
                    var yPos = height * i;

                    switch (matrice[i][j])
                    {
                        case S.Empty:

                            break;
                        case S.Wall:
                            g.FillRectangle(WallBrush, xPos, yPos, width, height);
                            break;
                        case S.Start:
                            switch (state) { 
                                case 0:
                                    g.FillEllipse(EllipseBrush, xPos, yPos, width / 3, height / 3);
                                    break;
                                case 1:
                                    g.FillEllipse(EllipseBrush, (int) x, (int) y, width / 3, height / 3);
                                    break;
                            }
                            break;
                        case S.End:
                            g.FillEllipse(EndBrush, xPos, yPos, width / 2, height / 2);
                            break;
                    }
                }
            }

            if (state == 0)
            {
                g.DrawEllipse(EllipsePen, x, y, 3, 3);
            }
        }

        internal override void WiimoteChanged(WiimoteState ws)
        {
            x = CalibrateX(ws.IRState.Midpoint.X);
            y = CalibrateY(ws.IRState.Midpoint.Y);

            if (fps++ == 10)
            {
                fps = 0;

                CheckMatrice();

                UpdateView();
            }
        }

        public void CheckMatrice()
        {
            var xPos = (int)(x / Width * matrice[0].Length) % matrice[0].Length;
            var yPos = (int)(y / Height * matrice.Length) % matrice.Length;

            //Console.WriteLine("position x:" + xPos + " y:" + yPos + "state:" + matrice[yPos][xPos]);

            if (state == 0 && matrice[yPos][xPos] == S.Start){
                Console.WriteLine("GAME START");
                state = 1;
            }
            else if (state == 1 && matrice[yPos][xPos] == S.Wall)
            {
                Console.WriteLine("GAME OVER");
                state = 2;
                GamePanel.GameEnded(false);
            }
            else if (state == 1 && matrice[yPos][xPos] == S.End)
            {
                Console.WriteLine("GAME FINISH");
                state = 3;
                GamePanel.GameEnded(true);
            }
        }
    }
}
