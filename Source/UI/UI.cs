using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UI
{
    /// <summary>
    /// Class which creates (prints) a user interface to the console. Can
    /// create multiple input field forms, forms with a single input field,
    /// alerts over the current console buffer and has support for
    /// pseudo control scrolling (incase the inputted text is wider
    /// than the control width in characters).
    /// </summary>
    public partial class UserInterface
    {
        /// <summary>
        /// Creates a form with multiple input fields, formatted as a two-column
        /// table. Prompts are displayed in the left column, whereas the input
        /// fields are displayed in the right column. 
        /// </summary>
        /// <param name="prompts">An array of input prompts for each input field.</param>
        /// <param name="types">
        /// Name of the accepted type in the input field. 
        /// Must be either string or int.
        /// </param>
        /// <param name="validators">
        /// An array of validator functions which return bool and accept object 
        /// as their first and only parameter. Two default validators for string
        /// and int objects are implemented, but are overriden if a custom validator
        /// is passed for the respective input field.
        /// </param>
        /// <returns>
        /// Returns a list of the objects the user entered in each input field. 
        /// </returns>
        public List<object> MultipleInputString(string[] prompts, string[] types, Func<object, bool>[] validators)
        {
            Table t = new Table();
            foreach (string prompt in prompts)
                t.AddRow(new object[] { prompt, "" });
            t.Draw();
            
            int oldX = Console.CursorLeft;
            int oldY = Console.CursorTop;

            List<object> toReturn = new List<object>();
            for (int i = 0; i < prompts.Length; i++)
            {
                Console.CursorLeft = t.Rows[i].Cells[1].X;
                Console.CursorTop = t.Rows[i].Cells[1].Y;

                string vrijednost = InputWithOverflow(t.Rows[i].Cells[1].Width - TableSettings.CELL_PADDING * 2);
                if(vrijednost == "")
                {
                    Alert(strings.NoInputError);
                    ResetField(ref t, i, vrijednost.Length);
                    i--;
                }
                else if (types[i] == "string")
                {
                    if (validators.Length > i)
                    {
                        bool parsed = validators[i](vrijednost);
                        if (parsed)
                            toReturn.Add(vrijednost);
                        else
                        {
                            Alert(strings.NotAStringOrNotValidated);
                            ResetField(ref t, i, t.Rows[i].Cells[1].Width - TableSettings.CELL_PADDING * 2);
                            i--;
                        }
                    }
                }
                else if (types[i] == "int")
                {
                    int number = 0;
                    bool parsed = int.TryParse(vrijednost, out number);

                    if (validators.Length > i)
                        if (parsed)
                            parsed = validators[i](number);

                    if (!parsed)
                    {
                        Alert(strings.NotANumberOrNotValidated);
                        ResetField(ref t, i, t.Rows[i].Cells[1].Width - TableSettings.CELL_PADDING * 2);
                        i--;
                        continue;
                    }
                    else
                        toReturn.Add(number);
                }
                else if (types[i] == "date")
                {
                    DateTime date;
                    bool parsed = DateTime.TryParseExact(vrijednost, "dd.MM.yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out date);

                    if (validators.Length > i)
                        if (parsed)
                            parsed = validators[i](date);

                    if (!parsed)
                    {
                        Alert(strings.NotADate);
                        ResetField(ref t, i, t.Rows[i].Cells[1].Width - TableSettings.CELL_PADDING * 2);
                        i--;
                        continue;
                    }
                    else
                        toReturn.Add(date);
                }
            }

            Console.CursorLeft = oldX;
            Console.CursorTop = oldY;

            return toReturn;
        }

        /// <summary>
        /// Creates a single input field formatted as a single-row two-column 
        /// table with the prompt text shown in the left column.
        /// </summary>
        /// <param name="prompt">The prompt text which appears to the left.</param>
        /// <returns>Returns a string entered by the user in the input field.</returns>
        public string InputString(string prompt, Func<string, bool> validator = null, string errMsg = null)
        {
            Table t = new Table();
            t.AddRow(new object[] { prompt, "" })
             .Draw();

            int oldX = Console.CursorLeft;
            int oldY = Console.CursorTop;
            
            Console.CursorLeft = t.Rows[0].Cells[1].X;
            Console.CursorTop = t.Rows[0].Cells[1].Y;

            string toRet = "";
            while(true)
            {
                toRet = InputWithOverflow(t.Rows[0].Cells[1].Width - TableSettings.CELL_PADDING * 2);
                if (validator != null)
                {
                    if (!validator(toRet))
                    {
                        if (errMsg == null)
                            Alert(strings.NotAStringOrNotValidated);
                        else
                            Alert(errMsg);
                        ResetField(ref t, 0, t.Rows[0].Cells[1].Width - TableSettings.CELL_PADDING * 2);
                    }
                    else if(toRet.Length > 0)
                        break;
                }
                else if(toRet.Length > 0)
                    break;
            }

            Console.ResetColor();
            Console.CursorLeft = oldX;
            Console.CursorTop = oldY;

            return toRet;
        }

        /// <summary>
        /// This basically creates an input field with a specified maximum
        /// display width. That is, if the user enters more characters than
        /// this width, the input field will scroll to the right. 
        /// </summary>
        /// <param name="width">
        /// Maximum number of characters the input field will display.
        /// </param>
        /// <returns>Returns a string entered by the user in the input field.</returns>
        private string InputWithOverflow(int width)
        {
            string input = "";
            int startX = Console.CursorLeft;
            ConsoleKeyInfo ck = new ConsoleKeyInfo();
            while ((ck = Console.ReadKey(true)).Key != ConsoleKey.Enter)
            {
                if (ck.Key == ConsoleKey.Backspace)
                {
                    if (Console.CursorLeft > startX)
                    {
                        Console.CursorLeft = Console.CursorLeft - 1;
                        Console.Write(" ");
                        Console.CursorLeft = Console.CursorLeft - 1;
                        input = input.Substring(0, input.Length - 1);

                        if (input.Length >= width)
                        {
                            Console.CursorLeft = startX;
                            Console.Write(input.Substring(input.Length - width));
                        }

                        continue;
                    }
                }
                else if (ck.KeyChar >= 33 && ck.KeyChar <= 126 || ck.Key == ConsoleKey.Spacebar)
                {
                    char c = ck.KeyChar;
                    input += c;
                    if (input.Length > width)
                    {
                        Console.CursorLeft = startX;
                        Console.Write(input.Substring(input.Length - width));
                    }
                    else
                    {
                        Console.Write(c);
                    }
                }
            }

            return input;
        }

        /// <summary>
        /// Displays an alert box over the current console buffer. Can either be
        /// an informative alert box, or a yes/no question box. The alert box is
        /// 20 characters shorter than the console screen width.
        /// </summary>
        /// <param name="message">The message that will be displayed inside the box.</param>
        /// <param name="foregroundColor">Font color of the alert box text.</param>
        /// <param name="backgroundColor">Background color of the alert box.</param>
        /// <param name="yesNo">
        /// A boolean indicating whether this is a yes/no question box.
        /// </param>
        /// <returns></returns>
        public bool Alert(string message, ConsoleColor foregroundColor = ConsoleColor.White, ConsoleColor backgroundColor = ConsoleColor.Red, bool yesNo = false)
        {
            int i = 0;
            for(int j = 0; j <= 10; j++)
            {
                i += 2;
            }
            
            int oldX = Console.CursorLeft, oldY = Console.CursorTop;

            short left = 10, top = (short)oldY, w = (short)(Console.BufferWidth - 20);

            List<string> messageLines = Utility.SeparateString(message, w - 4);

            short h = (short)messageLines.Count;
            h += 4;
            if (yesNo) h += 2;

            top -= (short)(h + 10);
            if (top < 5) top = 5;

            IEnumerable<string> prevBuffer = ConsoleReader.ReadFromBuffer(left, top, w, h);
            List<string> prevBuffer2 = new List<string>(prevBuffer);

            Console.ForegroundColor = foregroundColor;
            Console.BackgroundColor = backgroundColor;

            // Top border
            Console.SetCursorPosition(left, top);
            Console.Write(Box.TopLeft);
            for (int j = 0; j < w - 2; j++)
                Console.Write(Box.Horizontal);
            Console.Write(Box.TopRight);

            // Spacer
            Console.SetCursorPosition(left, top + 1);
            Console.Write("{0" + ",-" + (w - 1) + "}{0}", Box.Vertical);

            // Message, keep watch on overflow
            int i = 0;
            for(i = 0; i < messageLines.Count; i++)
            {
                Console.SetCursorPosition(left, top + 2 + i);
                Console.Write("{0} {1,-" + (w - 4) + "} {0}", Box.Vertical, messageLines[i]);
            }

            // Spacer
            Console.SetCursorPosition(left, top + 2 + i);
            Console.Write("{0" + ",-" + (w - 1) + "}{0}", Box.Vertical);

            if (yesNo)
            {
                // Spacer
                Console.SetCursorPosition(left, top + 2 + i);
                Console.Write("{0" + ",-" + (w - 1) + "}{0}", Box.Vertical);

                i++;
                Console.SetCursorPosition(left, top + 2 + i);
                Console.Write("{0} {1,-" + (w - 9) + "}{2,5} {0}", Box.Vertical, "[Y]es", "[N]o");

                i++;
                // Spacer
                Console.SetCursorPosition(left, top + 2 + i);
                Console.Write("{0" + ",-" + (w - 1) + "}{0}", Box.Vertical);
            }

            // Bottom border
            Console.SetCursorPosition(left, top + 3 + i);
            Console.Write(Box.BottomLeft);
            for (int j = 0; j < w - 2; j++)
                Console.Write(Box.Horizontal);
            Console.Write(Box.BottomRight);

            ConsoleKeyInfo keyPressed = Console.ReadKey(true);

            // Return the old state
            Console.ResetColor();
            int k = 0;
            foreach (string s in prevBuffer2)
            {
                Console.CursorLeft = left;
                Console.CursorTop = top + k;
                Console.Write(s);
                k++;
            }

            Console.CursorLeft = oldX;
            Console.CursorTop = oldY;

            if (yesNo)
            {
                return (keyPressed.KeyChar == 'Y' || keyPressed.KeyChar == 'y');
            }

            return false;

        }

        /// <summary>
        /// Clears the current text/value of the 2nd cell inside a 
        /// two-columned table used as input forms in some functions.
        /// </summary>
        /// <param name="t">Reference to the table maker.</param>
        /// <param name="i">Row number of the cell.</param>
        /// <param name="width">Width of the text that was previously
        /// entered, so we don't have to clear the whole cell.</param>
        private void ResetField(ref Table t, int i, int width)
        {
            Console.CursorLeft = t.Rows[i].Cells[1].X;
            Console.CursorTop = t.Rows[i].Cells[1].Y;
            for (int j = 0; j < width; j++)
                Console.Write(" ");
            Console.CursorLeft = t.Rows[i].Cells[1].X;
            Console.CursorTop = t.Rows[i].Cells[1].Y;
        }
    }
}
