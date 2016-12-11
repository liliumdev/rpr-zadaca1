using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UI
{
    /// <summary>
    /// Represents a single row in a table, taking notes where
    /// the table borders/seperators/delimiters should be printed
    /// and calculating the cell widths in the background.
    /// </summary>
    public class Row
    {
        /// <summary>
        /// A list which holds all informations about Cells in this row.
        /// </summary>
        public List<Cell> Cells { get; set; }

        /// <summary>
        /// X-positions of the table borders/delimitors for this row.
        /// </summary>
        public List<int> DelimitersX { get; set; }

        /// <summary>
        /// The number of cells this row has.
        /// </summary>
        public int NumberOfCells
        {
            get { return Cells.Count; }
        }


        public Row()
        {
            Cells = new List<Cell>();
        }

        public Row(List<Cell> cells)
        {
            Cells = cells;
            CalculateWidths();
        }

        /// <summary>
        /// Adds a cell to this row with the specified text/value.
        /// </summary>
        /// <param name="value">Printable object which this cell holds.</param>
        /// <param name="mode">The size of this cell. Atleast one cell in the whole row
        /// should have the Equal SizingMode.</param>
        /// <returns></returns>
        public Row AddCell(object value, SizingModes mode = SizingModes.Equal)
        {
            Cells.Add(new Cell(value, mode));

            return this;
        }

        /// <summary>
        /// Calculates the widths of cells in this row, taking into
        /// account the maximum console buffer/window width.
        /// </summary>
        public void CalculateWidths()
        {
            DelimitersX = new List<int>();
            DelimitersX.Add(0);

            int availableSpace = Console.BufferWidth - (NumberOfCells + 1);
            foreach (Cell cell in Cells)
                availableSpace -= (cell.SizingMode == SizingModes.Fixed ? cell.Width : 0); // incase width is fixed

            int leftCells = NumberOfCells;
            foreach (Cell cell in Cells)
                leftCells -= (cell.SizingMode == SizingModes.Fixed ? 1 : 0);

            int currentX = 0;
            foreach (Cell cell in Cells)
            {
                if (cell.SizingMode == SizingModes.Minimal)
                {
                    if (availableSpace >= cell.Value.ToString().Length)
                        cell.Width = cell.Value.ToString().Length + 2;
                    else
                        cell.Width = availableSpace;
                }
                else if (cell.SizingMode == SizingModes.Equal)
                {
                    cell.Width = availableSpace / leftCells;
                }
                else if (cell.SizingMode == SizingModes.Fixed)
                {
                    cell.Width = 20; // just a default, but 
                }


                currentX += cell.Width + 1;
                DelimitersX.Add(currentX);
                availableSpace -= cell.Width;

                leftCells--;
            }
        }
    }
}
