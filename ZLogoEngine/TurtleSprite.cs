using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using ZLangRT.Utils;

namespace ZLogoEngine
{
    public class TurtleSprite
    {
        public float X { get; set; }
        public float Y { get; set; }

        private  float angle;
        public  float Angle
        {
            get
            {
                return angle;
            }
            set
            {
                angle = value % 360;
                if (angle < 0)
                {
                    angle += 360;
                }
            }
        }

        public  Color PenColor
        {
            get
            {
                return drawPen.Color;
            }
            set
            {
                drawPen.Color = value;
            }
        }

        public  float PenSize
        {
            get
            {
                return drawPen.Width;
            }
            set
            {
                drawPen.Width = value;
            }
        }

        private  bool penVisible;
        public  bool PenVisible
        {
            get
            {
                return penVisible;
            }
            set
            {
                penVisible = value;
            }
        }

        public  bool ShowTurtle
        {
            get
            {
                return turtleHeadImage.Visible;
            }
            set
            {
                turtleHeadImage.Visible = value;
            }
        }

        private  int delay;
        public  int Delay
        {
            get
            {
                return delay;
            }
            set
            {
                delay = value;
            }
        }

        public void SetDelay(int time)
        {
            Delay = time;
        }

        public const int DrawAreaSize = 10000;
        public  readonly Color DefaultColor = Color.Blue;
        public readonly int DefaultPenSize = 3;        

        private  Control drawControl;
        private  Image drawImage;
        private  Graphics drawGraphics;
        private  Pen drawPen;
        private  PictureBox turtleHeadImage;

        public  void Init(Control targetControl)
        {
            // Dispose all resources if already allocated
            Dispose();

            drawControl = targetControl;
            SetDoubleBuffered(drawControl);

            // Create an empty graphics area to be used by the turtle
            drawImage = new Bitmap(DrawAreaSize, DrawAreaSize); 
            drawControl.Paint += DrawControl_Paint;
            drawControl.ClientSizeChanged += DrawControl_ClientSizeChanged;
            drawGraphics = Graphics.FromImage(drawImage);
            drawGraphics.SmoothingMode = SmoothingMode.AntiAlias;

            // Initialize the pen size and color
            drawPen = new Pen(DefaultColor, DefaultPenSize);
            drawPen.StartCap = LineCap.Round;
            drawPen.EndCap = LineCap.Round;
            X = 0;
            Y = 0;
            Angle = 0;
            PenVisible = true;
            // Delay = 0;  // Intentionally preserve the "Delay" settings

            // Initialize the turtle head image
            turtleHeadImage = new PictureBox();
            turtleHeadImage.BackColor = Color.Transparent;
            drawControl.Controls.Add(turtleHeadImage);
        }

        public  void Dispose()
        {
            if (drawControl != null)
            {
                // Release the pen object
                drawPen.Dispose();
                drawPen = null;

                // Release the graphic object
                drawGraphics.Dispose();
                drawGraphics = null;

                // Release the draw surface (image) object
                drawImage.Dispose();
                drawImage = null;

                // Release the turtle (head) image
                drawControl.Controls.Remove(turtleHeadImage);
                turtleHeadImage.Dispose();
                turtleHeadImage = null;

                // Release the drawing control and its associated events
                drawControl.Paint -= DrawControl_Paint;
                drawControl.ClientSizeChanged -= DrawControl_ClientSizeChanged;
                drawControl.Invalidate();
                drawControl = null;
            }
        }

        public  void Reset()
        {
            Dispose();
        }

        public  void Forward(float distance )
        {
            //Console.WriteLine("--- Forward Stack ---- " + distance);
            //Console.WriteLine(DebugUtil.StackFramesToString(new StackTrace().GetFrames()));
            var angleRadians = Angle * Math.PI / 180;
            var newX = X + (float)(distance * Math.Sin(angleRadians));
            var newY = Y + (float)(distance * Math.Cos(angleRadians));
            //Console.WriteLine(string.Format("Forward:{0},newX:{1},newY:{2},Angle:{3},angleRadians:{4}" , distance,X,Y,newX,newY,Angle,angleRadians));
            MoveTo(newX, newY);
        }

        public  void Backward(float distance )
        {
            Forward(-distance);
        }

        public void MoveTo(FloatPoint fpoint)
        {
            MoveTo(fpoint.X, fpoint.Y);
        }

        public  void MoveTo(float newX, float newY)
        {
            //Console.WriteLine( string.Format("MoveTo {0},{1}", newX, newY));
           // InitOnDemand();
            var fromX = DrawAreaSize / 2 + X;
            var fromY = DrawAreaSize / 2 - Y;
            X = newX;
            Y = newY;
            if (PenVisible)
            {
                var toX = DrawAreaSize / 2 + X;
                var toY = DrawAreaSize / 2 - Y;
                drawGraphics.DrawLine(drawPen, fromX, fromY, toX, toY);
            }
            DrawTurtle();
            PaintAndDelay();
        }

        public void Rotate(float angleDelta)
        {
            //InitOnDemand();
            Angle += angleDelta;
            DrawTurtle();
            PaintAndDelay();
        }

        public void RotateLeft(float angleDelta)
        {
            Rotate(-angleDelta);
        }

        public void RotateRight(float angleDelta)
        {
            Rotate(angleDelta);
        }
        
        public  void RotateTo(float newAngle)
        {
           // InitOnDemand();
            Angle = newAngle;
            DrawTurtle();
            PaintAndDelay();
        }

        public  void PenUp()
        {
            PenVisible = false;
        }

        public  void PenDown()
        {
            PenVisible = true;
        }

        private  void SetDoubleBuffered(Control control)
        {
            // set instance non-public property with name "DoubleBuffered" to true
            typeof(Control).InvokeMember("DoubleBuffered",
                BindingFlags.SetProperty | BindingFlags.Instance | BindingFlags.NonPublic,
                null, control, new object[] { true });
        }

        private  void DrawTurtle()
        {
            if (ShowTurtle)
            {
                //Console.WriteLine(string.Format(" * X={0},Y={1},Angle=:{2}",X,Y,Angle));
                var turtleImg = Properties.Resources.Turtle;
                turtleImg = RotateImage(turtleImg, angle);
                var turtleImgSize = Math.Max(turtleImg.Width, turtleImg.Height);
                turtleHeadImage.BackgroundImage = turtleImg;
                turtleHeadImage.Width = turtleImg.Width;
                turtleHeadImage.Height = turtleImg.Height;

                var turtleX = 1 + drawControl.ClientSize.Width / 2 + X - turtleHeadImage.Width / 2;
                var turtleY = 1 + drawControl.ClientSize.Height / 2 - Y - turtleHeadImage.Height / 2;

                turtleHeadImage.Left = (int)Math.Round(turtleX);
                turtleHeadImage.Top = (int)Math.Round(turtleY);
            }
        }

        private  Bitmap RotateImage(Bitmap bmp, float angleDegrees)
        {
            Bitmap rotatedImage = new Bitmap(bmp.Width, bmp.Height);
            using (Graphics g = Graphics.FromImage(rotatedImage))
            {
                g.TranslateTransform(bmp.Width / 2, bmp.Height / 2);
                g.RotateTransform(angleDegrees);
                g.TranslateTransform(-bmp.Width / 2, -bmp.Height / 2);
                g.DrawImage(bmp, new Point(0, 0));
            }
            bmp.Dispose();

            return rotatedImage;
        }

        private  void PaintAndDelay()
        {
            drawControl.Invalidate();
            if (Delay == 0)
            {
                // No delay -> invalidate the control, so it will be repainted later
            }
            else
            {
                // Immediately paint the control and them delay
                drawControl.Update();
                Thread.Sleep(Delay);
                Application.DoEvents();
            }
        }

        private  void DrawControl_ClientSizeChanged(object sender, EventArgs e)
        {
            drawControl.Invalidate();
            DrawTurtle();
        }

        private  void DrawControl_Paint(object sender, PaintEventArgs e)
        {
            if (drawControl != null)
            {
                var top = (drawControl.ClientSize.Width - DrawAreaSize) / 2;
                var left = (drawControl.ClientSize.Height - DrawAreaSize) / 2;
                // TODO: needs a fix -> does not work correctly when drawControl has AutoScroll
                e.Graphics.DrawImage(drawImage, top, left);
            }
        }
    }
}
