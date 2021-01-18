using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WPF_MindMap.Resources.scripts
{
    public static class Utils
    {
        public static int MAX_CHILD_COUNT = 6;
        public static Point[] ROOTS_CHILD_POINT_VALUE = { new Point{ X = -200, Y = 300 }, new Point { X = -400, Y = 0 }, new Point { X = -200, Y = -300 }, new Point { X = 200, Y = 300 }, new Point { X = 400, Y = 0 }, new Point { X = 200, Y = -300 } };
    }
}
