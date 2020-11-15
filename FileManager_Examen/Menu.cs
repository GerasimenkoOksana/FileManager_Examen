using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileManager_Examen
{
    class Menu
    {
        
        public static void BottomLine(int pos, List<string> menuFullNames)
        {
            if (menuFullNames[pos] != "[..]")
            {
                int xnow = Console.CursorLeft;
                int ynow = Console.CursorTop;
                Console.SetCursorPosition(0, 40);
                ConsoleColor bgNow = Console.BackgroundColor;
                ConsoleColor fgNow = Console.ForegroundColor;
                Console.BackgroundColor = ConsoleColor.DarkBlue; ;
                Console.ForegroundColor = ConsoleColor.White; ;
                Console.Write("                                                                                              ");
                Console.SetCursorPosition(1, 40);
                Console.Write(menuFullNames[pos]);
                Console.SetCursorPosition(xnow, ynow);
                Console.BackgroundColor = bgNow;
                Console.ForegroundColor = fgNow;
            }
        }

        public static int GeneralMenu(string[] elements, List<string> menuFullNames, char activePanel, out int nomDir)
        {
            if (elements.Count() > 34)
                return BigMenu(elements, menuFullNames, activePanel, out nomDir);
            else return RegularMenu(elements, menuFullNames, activePanel, out nomDir);
        }

        public static int RegularMenu(string[] elements, List<string> menuFullNames, char activePanel, out int nomDir)
        {
            if (activePanel == 'l')
                Console.SetCursorPosition(0, 4);
            else Console.SetCursorPosition(66, 4);
            int maxLen = 60;           
            ConsoleColor bg = ConsoleColor.DarkBlue;
            ConsoleColor fg = ConsoleColor.White;
            int x = Console.CursorLeft;
            int y = Console.CursorTop;
            Console.CursorVisible = false;
            int pos = 0;
            BottomLine(pos, menuFullNames);
            while (true)
            {
                for (int i = 0; i < elements.Length; i++)
                {
                    Console.SetCursorPosition(x, y + i);
                    if (i == pos)
                    {
                        Console.BackgroundColor = fg;
                        Console.ForegroundColor = bg;
                    }
                    else
                    {
                        Console.BackgroundColor = bg;
                        Console.ForegroundColor = fg;
                    }
                    Console.Write(elements[i].PadRight(maxLen));
                }

                ConsoleKey consoleKey = Console.ReadKey().Key;
                switch (consoleKey)
                {
                    case ConsoleKey.Enter:
                        nomDir = pos;
                        return pos;
                        break;
                    case ConsoleKey.UpArrow:
                         if (pos > 0)
                         {
                            nomDir = pos;
                            pos--;
                            BottomLine(pos, menuFullNames);
                         }
                        break;
                    case ConsoleKey.DownArrow:
                         if (pos < elements.Length - 1)
                         {
                            nomDir = pos;
                            pos++;
                            BottomLine(pos, menuFullNames);
                         }
                        break;
                    case ConsoleKey.Tab:
                        nomDir = pos;
                        return -11;
                        break;
                    case ConsoleKey.F1:
                        nomDir = pos;
                        return -1;
                        break;
                    case ConsoleKey.F2:
                        nomDir = pos;
                        return -2;
                        break;
                    case ConsoleKey.F3:
                        nomDir = pos;
                        return -3;
                        break;
                    case ConsoleKey.F4:
                        nomDir = pos;
                        return -4;
                        break;
                    case ConsoleKey.F5:
                        nomDir = pos;
                        return -5;
                        break;
                    case ConsoleKey.F6:
                        nomDir = pos;
                        return -6;
                        break;
                    case ConsoleKey.F7:
                        nomDir = pos;
                        return -7;
                        break;
                    case ConsoleKey.F8:
                        nomDir = pos;
                        return -8;
                        break;
                    case ConsoleKey.F9:
                        nomDir = pos;
                        return -9;
                        break;
                    case ConsoleKey.F10:
                        nomDir = pos;
                        return -10;
                        break;
                }
            }
        }

        public static int RegularMenuForBig(string[] elements, List<string> menuFullNames, char activePanel, int pos, out int nomDir, ref int up, ref int down)
        {
            if (activePanel == 'l')
                Console.SetCursorPosition(0, 4);
            else Console.SetCursorPosition(66, 4);
            int maxLen = 60;
            ConsoleColor bg = ConsoleColor.DarkBlue;
            ConsoleColor fg = ConsoleColor.White;
            int x = Console.CursorLeft;
            int y = Console.CursorTop;
            Console.CursorVisible = false;           
            BottomLine(pos, menuFullNames);
            while (true)
            {
                for (int i = 0; i < elements.Length; i++)
                {
                    Console.SetCursorPosition(x, y + i);
                    if (i == pos)
                    {
                        Console.BackgroundColor = fg;
                        Console.ForegroundColor = bg;
                    }
                    else
                    {
                        Console.BackgroundColor = bg;
                        Console.ForegroundColor = fg;
                    }
                    Console.Write(elements[i].PadRight(maxLen));
                }

                ConsoleKey consoleKey = Console.ReadKey().Key;
                switch (consoleKey)
                {
                    case ConsoleKey.Enter:
                        nomDir = pos;
                        return pos;
                        break;
                    case ConsoleKey.UpArrow:
                        nomDir = pos;
                        if (pos > 0)
                        {
                            pos--;
                            BottomLine(pos, menuFullNames);
                        }
                        else if (up>0) return -38;
                        break;
                    case ConsoleKey.DownArrow:
                        nomDir = pos;
                        if (pos < elements.Length - 1)
                        {                            
                            pos++;
                            BottomLine(pos, menuFullNames);
                        }
                        else if (down>0) return -40;
                        break;
                    case ConsoleKey.Tab:
                        nomDir = pos;
                        return -11;
                        break;
                    case ConsoleKey.F1:
                        nomDir = pos;
                        return -1;
                        break;
                    case ConsoleKey.F2:
                        nomDir = pos;
                        return -2;
                        break;
                    case ConsoleKey.F3:
                        nomDir = pos;
                        return -3;
                        break;
                    case ConsoleKey.F4:
                        nomDir = pos;
                        return -4;
                        break;
                    case ConsoleKey.F5:
                        nomDir = pos;
                        return -5;
                        break;
                    case ConsoleKey.F6:
                        nomDir = pos;
                        return -6;
                        break;
                    case ConsoleKey.F7:
                        nomDir = pos;
                        return -7;
                        break;
                    case ConsoleKey.F8:
                        nomDir = pos;
                        return -8;
                        break;
                    case ConsoleKey.F9:
                        nomDir = pos;
                        return -9;
                        break;
                    case ConsoleKey.F10:                       
                        nomDir = pos;                        
                        return -10;
                        break;
                }
            }
        }
        public static int BigMenu(string[] elements, List<string> menuFullNames,char activePanel, out int nomDir)
        {
            int up = 0;
            int down = elements.Count() - 34;
            int res;
           int pos = 0;
            do
            {
                string[] tempMenu = new string[34];
                Array.Copy(elements, up, tempMenu, 0, 34);
                List<string> tempMenuFullNames = new List<string>();
                for (int i = up; i < up+34; i++)
                {
                    tempMenuFullNames.Add(menuFullNames[i]);
                }
                res = RegularMenuForBig(tempMenu, tempMenuFullNames, activePanel, pos, out nomDir, ref up, ref down);
                switch (res)
                {
                    case -38:
                        {
                            up--;                           
                            down++;
                            pos = 0;
                        };
                        break;
                    case -40:
                        {
                            down--;                           
                            up++;
                            pos = 33;  
                        };
                        break;
                    default:                       
                        break;
                }
            } while (res == -38 || res == -40);
            nomDir += up;
            return res;
        }

        public static int GorizontalMenu(string[] elements, int width)
        {
            int x = Console.CursorLeft;
            int y = Console.CursorTop;
            int pos = 0;
            while (true)
            {
                for (int i = 0; i < elements.Length; i++)
                {
                    Console.SetCursorPosition(x+width*i+width, y);
                    if (i == pos)
                    {
                        Console.BackgroundColor = ConsoleColor.Blue;                       
                    }
                    else
                    {
                        Console.BackgroundColor = ConsoleColor.Gray;                      
                    }
                    Console.Write(elements[i]);
                }
                ConsoleKey consoleKey = Console.ReadKey().Key;
                switch (consoleKey)
                {
                    case ConsoleKey.Enter:
                        return pos;
                        break;
                    case ConsoleKey.Escape:
                        return - 1;
                        break;
                    case ConsoleKey.LeftArrow:
                        if (pos > 0)
                            pos--;
                        break;
                    case ConsoleKey.RightArrow:
                        if (pos < elements.Length - 1)
                            pos++;
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
