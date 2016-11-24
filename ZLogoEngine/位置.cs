using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ZLangRT.Attributes;

namespace ZLogoEngine
{
    [ZMapping(typeof(FloatPoint))]
    public abstract class 位置
    {
        [ZCode("X坐标")]
        public float X { get; set; }

        [ZCode("Y坐标")]
        public float Y { get; set; }
    }
}
