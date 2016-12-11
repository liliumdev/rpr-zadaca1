using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UI
{
    public class MenuOption
    {
        public Func<SelOption> Function { get; set; }
        public string Text { get; set; }
        public bool IsClickable { get; set; }
        public int MenuID { get; set; }
        public int Y { get; set; }

        public MenuOption(string text, Func<SelOption> f, bool clickable, int id)
        { 
            Text = text; Function = f; IsClickable = clickable; MenuID = id;
        }
    }

    public class SelOption
    {
        public int SelectedIndex { get; set; }
        public Func<SelOption> Func { get; set; }

        public SelOption(int i, Func<SelOption> f)
        {
            SelectedIndex = i;
            Func = f;
        }
    }

    public class Menu
    {
        private int menuId;
        private List<MenuOption> Options;
        public string Title { get; set; }
        private Table t;
        public int SelectedOption { get; set; }
        
        public Menu(string title = "")
        {
            Options = new List<MenuOption>();
            menuId = 1;
            Title = title;
            t = new Table();
            SelectedOption = 1;
        }

        public SelOption Draw(bool clear = true)
        {
            if(clear)
                Console.Clear();
            if(Title != "")
                t.AddRow(new object[] { Title }).SetCellColor(0, ConsoleColor.White, ConsoleColor.Blue);

            int i = (Title == "" ? 0 : 1);
            bool setDefaultSelection = false;
            foreach (var option in Options)
            {
                if (option.IsClickable)
                {
                    t.AddRow(new object[] { option.MenuID, option.Text }, new SizingModes[] { SizingModes.Minimal, SizingModes.Equal });
                    if (!setDefaultSelection) { t.SetCellColor(1, ConsoleColor.Black, ConsoleColor.Gray); setDefaultSelection = true; }
                }
                else
                {
                    t.AddRow(new object[] { option.Text });
                }

                option.Y = i;
                i++;
            }

            t.Draw();

            ConsoleKeyInfo ck = new ConsoleKeyInfo();
            while ((ck = Console.ReadKey(true)).Key != ConsoleKey.Enter)
            {
                if (ck.Key == ConsoleKey.DownArrow)
                {
                    if (SelectedOption + 1 == menuId)
                    {
                        // We're already at the bottom of all options, now go to top
                        SelectedOption = 0;
                    }

                    SelectedOption++;
                    foreach (var option in Options)
                    {
                        if (option.MenuID != -1)
                        {
                            if (option.MenuID != SelectedOption)
                            {
                                t.Rows[option.Y].Cells[1].ForegroundColor = ConsoleColor.Gray;
                                t.Rows[option.Y].Cells[1].BackgroundColor = ConsoleColor.Black;
                            }
                            else
                            {
                                t.Rows[option.Y].Cells[1].ForegroundColor = ConsoleColor.Black;
                                t.Rows[option.Y].Cells[1].BackgroundColor = ConsoleColor.Gray;
                            }
                        }
                    }
                    t.Redraw();

                }
                else if (ck.Key == ConsoleKey.UpArrow)
                {
                    if (SelectedOption - 1 == 0)
                    {
                        // We're already at the top of all options, now go to bottom
                        SelectedOption = menuId;
                    }

                    SelectedOption--;
                    foreach (var option in Options)
                    {
                        if (option.MenuID != -1)
                        {
                            if (option.MenuID != SelectedOption)
                            {
                                t.Rows[option.Y].Cells[1].ForegroundColor = ConsoleColor.Gray;
                                t.Rows[option.Y].Cells[1].BackgroundColor = ConsoleColor.Black;
                            }
                            else
                            {
                                t.Rows[option.Y].Cells[1].ForegroundColor = ConsoleColor.Black;
                                t.Rows[option.Y].Cells[1].BackgroundColor = ConsoleColor.Gray;
                            }
                        }
                    }
                    t.Redraw();

                }
            }

            // Pressed enter
            foreach (var option in Options)
            {
                if (option.MenuID == SelectedOption && option.Function != null)
                    return new SelOption(SelectedOption, option.Function);
            }

            return new SelOption(SelectedOption, null);
        }

        public Menu AddOption(string menuText, Func<SelOption> action = null)
        {
            MenuOption o = new MenuOption(menuText, action, true, menuId);
            Options.Add(o);
            menuId++;

            return this;
        }

        public Menu AddNote(string menuText)
        {
            MenuOption o = new MenuOption(menuText, null, false, -1);
            Options.Add(o);

            return this;
        }
    }
}
