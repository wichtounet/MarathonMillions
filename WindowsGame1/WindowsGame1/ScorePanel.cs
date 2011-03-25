using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Marathon
{
    class ScorePanel : Panel
    {
        private delegate void UpdateCallback();

        public ScorePanel()
        {
            Paint += PaintView;

            ScoreManager.GetInstance().ScoreChanged += ScoreChanged;
        }
        
        private void PaintView(object sender, PaintEventArgs e)
        {
            e.Graphics.DrawString("Millions : " + ScoreManager.GetInstance().Score + " Bonus = " + ScoreManager.GetInstance().Multiplier, 
                new Font("Verdana", 16), new SolidBrush(Color.Black), 5, 5);
        }

        private void ScoreChanged(object sender, ScoreChangedArgs args)
        {
            if (InvokeRequired)
            {
                UpdateCallback callback = new UpdateCallback(UpdateView);
                BeginInvoke(callback);
            }
            else
            {
                UpdateView();
            }
        }

        private void UpdateView()
        {
            Refresh();
        }
    }
}
