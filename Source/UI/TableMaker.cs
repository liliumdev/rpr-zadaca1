using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UI
{
    public enum SizingModes { Minimal, Equal, Fixed }

    public static class TableSettings
    {
        public static int CELL_PADDING = 1;
    }

    public static class Box
    {
        public static char TopLeft = '╔';
        public static char TopRight = '╗';
        public static char TopDown = '╦';
        public static char BottomUp = '╩';
        public static char BottomLeft = '╚';
        public static char BottomRight = '╝';
        public static char MiddleRight = '╣';
        public static char MiddleLeft = '╠';
        public static char Horizontal = '═';
        public static char Vertical = '║';
        public static char Middle = '╬';
    }

    public class Table
    {
        public List<Row> Rows;
        private int StartX { get; set; }
        private int StartY { get; set; }

        public Table()
        {
            Rows = new List<Row>();
        }

        public Table AddRow(object[] cellValues, SizingModes[] sizingModes = null)
        {
            List<Cell> cells = new List<Cell>();
            for (int i = 0; i < cellValues.Length; i++)
            {
                if (sizingModes != null)
                    cells.Add(new Cell(cellValues[i], (i < sizingModes.Length ? sizingModes[i] : SizingModes.Equal)));
                else
                    cells.Add(new Cell(cellValues[i]));

            }

            if (sizingModes != null)
                if (sizingModes.Count() > 0)
                    if (!sizingModes.Contains(SizingModes.Equal))
                        throw new Exception("A row needs to have atleast one equal column!");

            Rows.Add(new Row(cells));

            return this;
        }

        // For previous row
        public Table SetColumnSize(int i, int size)
        {
            Rows.Last().Cells[i].Width = size;
            Rows.Last().CalculateWidths();

            return this;
        }

        // For cell color
        public Table SetCellColor(int i, ConsoleColor fore, ConsoleColor back)
        {
            Rows.Last().Cells[i].ForegroundColor = fore;
            Rows.Last().Cells[i].BackgroundColor = back;

            return this;
        }
        
        public void Draw()
        {
            StartX = Console.CursorLeft;
            StartY = Console.CursorTop;

            for (int i = 0; i < Rows.Count; i++)
            {
                // First let's draw the top border, if it's the first row
                if (i == 0)
                {
                    Console.Write(Box.TopLeft);
                    for (int x = 1; x < Console.BufferWidth - 1; x++)
                    {
                        if (Rows[i].DelimitersX.Contains(x))
                            Console.Write(Box.TopDown);
                        else
                            Console.Write(Box.Horizontal);
                    }
                    Console.Write(Box.TopRight);
                }

                // Now the middle, the actual "cells". We first
                // gotta see if this will be a multiline
                // cell, that is - the one that takes up more than
                // one character vertically (but still in one row)
                List<List<string>> cellValuesSplitted = new List<List<string>>();
                for (int j = 0; j < Rows[i].NumberOfCells; j++)
                    cellValuesSplitted.Add(Utility.SeparateString(Rows[i].Cells[j].Value.ToString(), Rows[i].Cells[j].Width - TableSettings.CELL_PADDING * 2));

                int verticalHeight = cellValuesSplitted.OrderByDescending(x => x.Count()).Take(1).ElementAt(0).Count();
                foreach (List<string> splits in cellValuesSplitted)
                {
                    if (splits.Count() < verticalHeight)
                    {
                        int razlika = verticalHeight - splits.Count();
                        for (int l = 0; l < razlika; l++)
                            splits.Add(" ");
                    }
                }

                for (int j = 0; j < verticalHeight; j++)
                {
                    Console.Write(Box.Vertical);

                    for (int l = 0; l < Rows[i].NumberOfCells; l++)
                    {
                        Console.ForegroundColor = Rows[i].Cells[l].ForegroundColor;
                        Console.BackgroundColor = Rows[i].Cells[l].BackgroundColor;

                        for (int k = 0; k < TableSettings.CELL_PADDING; k++)
                            Console.Write(" ");

                        if (j == 0)
                        {
                            Rows[i].Cells[l].X = Console.CursorLeft;
                            Rows[i].Cells[l].Y = Console.CursorTop;
                            Rows[i].Cells[l].Height = verticalHeight;
                        }
                        int cellWidth = (Rows[i].Cells[l].Width - TableSettings.CELL_PADDING * 2);
                        Console.Write((cellValuesSplitted[l][j]).PadRight(cellWidth));

                        for (int k = 0; k < TableSettings.CELL_PADDING; k++)
                            Console.Write(" ");

                        Console.ResetColor();

                        Console.Write(Box.Vertical);
                    }
                }

                // Now the bottom. If it's the last row, we just close it as we should
                if (i == Rows.Count - 1)
                {
                    Console.Write(Box.BottomLeft);
                    for (int x = 1; x < Console.BufferWidth - 1; x++)
                    {
                        if (Rows[i].DelimitersX.Contains(x))
                            Console.Write(Box.BottomUp);
                        else
                            Console.Write(Box.Horizontal);
                    }
                    Console.Write(Box.BottomRight);
                }
                else
                {
                    // Moramo paziti sljedeci red
                    Console.Write(Box.MiddleLeft);
                    for (int x = 1; x < Console.BufferWidth - 1; x++)
                    {
                        if (Rows[i].DelimitersX.Contains(x) && !Rows[i + 1].DelimitersX.Contains(x))
                            Console.Write(Box.BottomUp);
                        else if (Rows[i].DelimitersX.Contains(x) && Rows[i + 1].DelimitersX.Contains(x))
                            Console.Write(Box.Middle);
                        else if (!Rows[i].DelimitersX.Contains(x) && Rows[i + 1].DelimitersX.Contains(x))
                            Console.Write(Box.TopDown);
                        else
                            Console.Write(Box.Horizontal);
                    }
                    Console.Write(Box.MiddleRight);
                }
            }
        }

        public void Redraw()
        {
            int oldX = Console.CursorLeft, oldY = Console.CursorTop;

            Console.CursorLeft = StartX;
            Console.CursorTop = StartY;

            Draw();

            Console.CursorLeft = oldX;
            Console.CursorTop = oldY;
        }
    }
}
