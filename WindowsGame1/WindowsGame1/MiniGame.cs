using WiimoteLib;

namespace Marathon
{
    public abstract class MiniGame : GraphicsDeviceControl
    {
        private delegate void UpdateCallback();

        protected MiniGame(GamePanel gamePanel, Wiimote wm)
        {
            GamePanel = gamePanel;
            Wm = wm;
        }

        protected GamePanel GamePanel { get; private set; }
        protected Wiimote Wm { get; private set; }

        public abstract void Start();

        internal abstract void WiimoteChanged(WiimoteState ws);

        public float CalibrateX(float x)
        {
            var x2 = Width - ((x - 0.2f) / 60 * 100) * Width;

            return (x2 < 0) ? 0 : x2;
        }

        public float CalibrateY(float y)
        {
            var y2 = ((y - 0.1f) / 70 * 100) * Height;

            return (y2 < 0) ? 0 : y2;
        }

        public void UpdateView()
        {
            if (InvokeRequired)
            {
                BeginInvoke(new UpdateCallback(Repaint));
            }
            else
            {
                Repaint();
            }
        }

        private void Repaint()
        {
            Invalidate(false);
        }
    }
}