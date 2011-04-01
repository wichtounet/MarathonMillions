﻿using System;
using System.Drawing;
using System.Windows.Forms;
using System.Speech.Synthesis;
using System.Speech.Recognition;
using WiimoteLib;

namespace Marathon
{
    public class MarathonForm : Form
    {
        private readonly QuestionPanel questionPanel;
        private readonly ScorePanel scorePanel;
        private readonly GamePanel gamePanel;
        private readonly Wiimote wm;

        public MarathonForm()
        {
            AutoScaleDimensions = new SizeF(6F, 13F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1000, 400);
            Name = "Marathon des millions";

            
            // create a new instance of the Wiimote
            wm = new Wiimote();

            scorePanel = new ScorePanel();
            gamePanel = new GamePanel(wm);
            questionPanel = new QuestionPanel(gamePanel);

            // setup the event to handle state changes
            wm.WiimoteChanged += WiimoteChanged;

            // setup the event to handle insertion/removal of extensions
            wm.WiimoteExtensionChanged += WiimoteExtensionChanged;

            // connect to the Wiimote
            wm.Connect();

            // set the report type to return the IR sensor and accelerometer data (buttons always come back)
            wm.SetReportType(InputReport.IRAccel, IRSensitivity.WiiLevel5, true);

            Layout += MarathonLayout;

            Controls.Add(questionPanel);
            Controls.Add(scorePanel);
            Controls.Add(gamePanel);

            questionPanel.Start();
        }

        void MarathonLayout(object sender, LayoutEventArgs e)
        {
            scorePanel.SetBounds(5, 5, Width - 10, 40);

            questionPanel.SetBounds(5, 50, (Width - 10) / 2, Height - 50);
            gamePanel.SetBounds((Width / 2)+5, 50, (Width / 2) -10, Height-100);
        }

        void WiimoteExtensionChanged(object sender, WiimoteExtensionChangedEventArgs args)
        {
            wm.SetReportType(args.Inserted ? InputReport.IRExtensionAccel : InputReport.IRAccel, true);
        }

        void WiimoteChanged(object sender, WiimoteChangedEventArgs args)
        {
            gamePanel.WiimoteChanged(sender, args);
        }
    }
}
