using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UI
{
    public class Cell
    {
        public object Value { get; set; }
        public int Width { get; set; }
        public SizingModes SizingMode { get; set; }

        public int X { get; set; }
        public int Y { get; set; }
        public int Height { get; set; }

        public ConsoleColor ForegroundColor { get; set; }
        public ConsoleColor BackgroundColor { get; set; }

        public Cell(object value, SizingModes mode = SizingModes.Equal)
        {
            Value = value; SizingMode = mode;
            ForegroundColor = ConsoleColor.Gray;
            BackgroundColor = ConsoleColor.Black;
        }
    }
}
