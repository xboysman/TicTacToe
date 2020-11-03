using System;
using System.Drawing;
using System.Windows.Forms;

namespace Login
{
    public static class ControlMove
    {
        private static bool MouseIsDown = false;
        private static Point FirstPoint;
        private static Control c;

        private static void Control_MouseDown(object sender, MouseEventArgs e)
        {
            c = (Control)sender;

            MouseIsDown = true;
            FirstPoint = e.Location;

            c.Cursor = Cursors.SizeAll;
        }

        private static void Control_MouseUp(object sender, MouseEventArgs e)
        {
            c = (Control)sender;

            MouseIsDown = false;
            c.Cursor = Cursors.Default;
        }

        private static void Control_MouseMove(object sender, MouseEventArgs e)
        {
            if (MouseIsDown)
            {
                c = (Control)sender;

                int xDiff = FirstPoint.X - e.Location.X;
                int yDiff = FirstPoint.Y - e.Location.Y;

                int x = c.Location.X - xDiff;
                int y = c.Location.Y - yDiff;

                c.Location = new Point(x, y);
            }
        }

        public static void ControlEventSubscription(Control c)
        {
            c.MouseDown += Control_MouseDown;
            c.MouseUp += Control_MouseUp;
            c.MouseMove += Control_MouseMove;
        }
    }
}
