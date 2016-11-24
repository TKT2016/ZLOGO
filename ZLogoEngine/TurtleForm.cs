using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ZLogoEngine
{
    public class TurtleForm:Form
    {
        public TurtleSprite Turtle { get;private set; }
        public TurtleForm Window { get; private set; }

        public TurtleForm()
        {
            StackFrame[] stacks = new StackTrace().GetFrames();
            //Console.WriteLine(StackFramesToString(stacks));
            Turtle = new TurtleSprite();
            //Console.WriteLine("TurtleTestForm Init");
            Turtle.Init(this);
            this.Size = new Size(800, 600);
            Window = this;
            //RunZLogo();
            //this.Load += new System.EventHandler(this.TurtleForm_Load);
            this.Activated += new System.EventHandler(this.TurtleForm_Activated);
        }
        
        /*private void TurtleForm_Load(object sender, EventArgs e)
        {
            RunZLogo();
        }*/
        private bool isRuned = false;
        private void TurtleForm_Activated(object sender, EventArgs e)
        {
            if (!isRuned)
            {
                //Console.WriteLine("开始");
                Turtle.Delay = 200;
                RunZLogo();
                isRuned = true;
                //Console.WriteLine("结束");
            }
        }

        public virtual void RunZLogo()
        {
            
        }

        public void SetTitle(string title)
        {
            this.Text = title;
        }
    }
}
