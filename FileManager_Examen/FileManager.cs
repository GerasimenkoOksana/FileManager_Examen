using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Services;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FileManager_Examen
{
    class FileManager
    {
        public static bool showHidden = false;
        
        /// <summary>
        /// формирует строку для меню - обрезает символы имени после 50
        /// </summary>
        /// <param name="name"></param>
        /// <returns> возвращает полное имя, если его длина <=50 или обрезанное до 47 символов c троеточием в конце</returns>
        public static string GetNameForMenu(string name)
        {
            if (name.Length <= 50)
                return name;
            else return name.Substring(0, 47)+"...";
        }

        /// <summary>
        /// формирует массив строк для меню - содержимое каталога
        /// </summary>
        /// <param name="dir">переменная типа DirectoryInfo, указывающая выбранный каталог</param></param>
        /// <param name="showHidden">переменная типа bool - показывать ли скрытые файлы и папки</param>
        /// <param name="menuFullNames">переменная типа List<string> содержит список полных имен папок и файлов</param>
        /// <returns></returns>
        public static string[] GetMenu(DirectoryInfo dir, bool showHidden, out List<string>menuFullNames )
        {
            DirectoryInfo[] dirs = dir.GetDirectories();
            FileInfo[] files = dir.GetFiles();
            menuFullNames = new List<string>();
            if (showHidden)
            {
                string[] menu;
                int i = 0;
                if (dir.Root.Name != dir.Name)
                {
                    menu = new string[dirs.Length + files.Length + 1];
                    menu[0] = " [..]";
                    menuFullNames.Add("[..]");
                    i = 1;
                }
                else
                {
                    menu = new string[dirs.Length + files.Length];
                }  
                foreach (var item in dirs)
                { 
                   menu[i++] = $" {GetNameForMenu(item.Name).PadRight(50)} {"DIR".PadLeft(10)}";
                   menuFullNames.Add(item.FullName);
                }
                foreach (var item in files)
                {
                        menu[i++] = $" {GetNameForMenu(item.Name).PadRight(50)} {item.Length.ToString().PadLeft(10)}";
                        menuFullNames.Add(item.FullName);
                }
                return menu;
            }
            else
            {
                int kolDir = 0, kolFile = 0;
                foreach (var item in dirs)
                {
                    if (!item.Attributes.ToString().Contains("Hidden"))
                        kolDir++;
                }
                foreach (var item in files)
                {
                    if (!item.Attributes.ToString().Contains("Hidden"))
                        kolFile++;
                }
                string[] menu;
                int i=0;
                if (dir.Root.Name != dir.Name)
                {
                    menu = new string[kolDir + kolFile + 1];
                    i = 1;
                    menu[0] = " [..]";
                    menuFullNames.Add("[..]");
                }
                else
                {
                    menu = new string[kolDir + kolFile];
                }
                foreach (var item in dirs)
                {
                    if (!item.Attributes.ToString().Contains("Hidden"))
                    {
                        menu[i++] = $" {GetNameForMenu(item.Name).PadRight(50)} {"DIR".PadLeft(10)}";
                        menuFullNames.Add(item.FullName);
                    }
                }
                foreach (var item in files)
                {
                    if (!item.Attributes.ToString().Contains("Hidden"))
                    {
                        menu[i++] = $" {GetNameForMenu(item.Name).PadRight(50)} {item.Length.ToString().PadLeft(10)}";
                        menuFullNames.Add(item.FullName);
                    }
                }
                return menu;
            }
        }

        public static void Shapka(string dirLeftName, string dirRightName, char activePanel)
        {
           
            
            Console.BackgroundColor = ConsoleColor.DarkBlue;
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine();
            Console.WriteLine(" F1 Info     F2 Drive     F3 View     F4 Edit     F5 Copy     F6 Move     F7 Mkdir     F8 Del     F9 Rename      F10 ShowHidden");
            Console.WriteLine(" -------------------------------------------------------------------------------------------------------------------------------");
            Console.SetCursorPosition(20, 3);
            if (activePanel == 'l')
                Console.WriteLine($"Drive: {dirLeftName}");
            else Console.WriteLine($"Drive: {dirRightName}");
            Console.SetCursorPosition(80, 3);
            if (activePanel == 'l')
                Console.WriteLine($"Drive: {dirRightName}");
            else Console.WriteLine($"Drive: {dirLeftName}");
            Console.SetCursorPosition(0, 39);
            Console.WriteLine(" -------------------------------------------------------------------------------------------------------------------------------");
            Console.SetCursorPosition(0, 4);
        }

        /// <summary>
        /// выводит неактивную панель - список папок и файлов неактивного каталога
        /// </summary>
        /// <param name="elements">массив из строк - названия папок и файлов, содержащихся в неактивном каталоге</param>
        /// <param name="activePanel">переменная типа char - маркер активной панели, если активна левая панель, равен 'l', если правая - 'r' </param>
        public static void PanelDir(string[] elements, char activePanel)
        {
            Console.CursorVisible = false;
            int x=66, y=4;           
            if (activePanel == 'r')
            {
                x = 0;
                y = 4;
            };

            for (int i = 0; i < elements.Length; i++)
            {
                Console.SetCursorPosition(x, y + i);
                Console.Write(elements[i]);
            }
        }
        public static void CopyDir(string FromDir, string ToDir)
        { 
            foreach (string s1 in Directory.GetFiles(FromDir))
            {
                string s2 = ToDir + @"\" + Path.GetFileName(s1);
                File.Copy(s1, s2);
            }
            foreach (string s in Directory.GetDirectories(FromDir))
            {

                Directory.CreateDirectory(ToDir + @"\" + Path.GetFileName(s));
                CopyDir(s, ToDir + @"\" + Path.GetFileName(s));
            }
        }
       public static void DelDir(DirectoryInfo dir)
        {
            foreach (FileInfo item in dir.GetFiles())
            {
                item.Delete();
            }
            foreach (DirectoryInfo item in dir.GetDirectories())
            {
                DelDir(item);
            }
            dir.Delete();
        }
        public static void App()
        {
            Console.SetWindowSize(130, 40);
            DriveInfo[] drives = DriveInfo.GetDrives();            
            DirectoryInfo dirActive = new DirectoryInfo(@"C:\");
            if (!dirActive.Exists) dirActive = new DirectoryInfo(drives[0].Name);
            DirectoryInfo dirInActive = new DirectoryInfo(@"D:\");
            if (!dirInActive.Exists) dirInActive = new DirectoryInfo(@"C:\");
            if (!dirInActive.Exists) dirInActive = new DirectoryInfo(drives[0].Name);
            char activePanel = 'l';  
            List<string> menuFullNames = null;
            List<string> menuFullNamesInactive = null;
           
            while (true)
            {
                int pos = 0;
                Shapka(dirActive.FullName, dirInActive.FullName, activePanel);
                string[] menu = GetMenu(dirActive, showHidden, out menuFullNames);
                string[] menuInActive = GetMenu(dirInActive, showHidden, out menuFullNamesInactive);
                PanelDir(menuInActive, activePanel);                
                int nomDir;
                int vibor = Menu.GeneralMenu(menu, menuFullNames, activePanel, out nomDir);
                string type;
                if (menu[nomDir].Contains("DIR"))
                    type = "DIR";
                else type = "file";
                string name = menuFullNames[nomDir];
                switch (vibor)
                {
                    case -1:
                        if (name != "[..]")
                            F1Info(type, name, activePanel, ref nomDir);
                        break;
                    case -2:
                        F2Drive(ref dirActive, activePanel);
                        break;
                    case -3:
                        if (type == "file")
                        F3View(name);
                        break;
                    case -4:
                        if (type == "file")
                            F4Edit(name);
                        break;
                    case -5:
                        if (name != "[..]")
                            F5Copy(type, name, dirInActive );
                        break;
                    case -6:
                        if (name != "[..]")
                            F6Move(type, name, dirInActive);
                        break;
                    case -7:
                        F7MkDir(dirActive);
                        break;
                    case -8:
                        if (name != "[..]")
                            F8Del(type,name);
                        break;
                    case -9:
                        if (name != "[..]")
                            F9Rename(type, name);
                        break;
                    case -10:
                        F10ShowHidden(nomDir, activePanel);
                        break;
                    case -11:                           
                        FTab(ref dirActive, ref dirInActive, ref activePanel);
                        break;
                    default:
                          if (name.EndsWith(".exe"))
                                try
                                {
                                    Process.Start(name);
                                }
                                catch { };
                      
                        if (menuFullNames[nomDir] == "[..]")
                            dirActive = dirActive.Parent;
                        else
                        {
                            if (type == "DIR")
                                dirActive = new DirectoryInfo(menuFullNames[nomDir]);
                        }
                        break;
                }
            }
        } 
        public static void FTab(ref DirectoryInfo dirActive, ref DirectoryInfo dirInActive, ref char activePanel)
        {
            if (activePanel == 'l')
                activePanel = 'r';
            else activePanel = 'l';
            DirectoryInfo temp = dirActive;
            dirActive = dirInActive;
            dirInActive = temp;
        }

        public static void F10ShowHidden(int nomDir, char activePanel)
        {
            showHidden = !showHidden;
        }

        public static void F9Rename(string type, string name)
        {
            int x = 30, y = 10;
            DrawRect(x, y, 70, 5);
            if (type == "file")
            {
                FileInfo file = new FileInfo(name);
                string warning = "";
                if (file.Attributes.ToString().Contains("Hidden") || file.Attributes.ToString().Contains("System")
                    || file.IsReadOnly)
                    warning = "!!! File has attributes:" + file.Attributes;
                Console.SetCursorPosition(x, y + 1);
                if (warning != "")
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write(warning);
                    Console.ForegroundColor = ConsoleColor.Black;
                }
                Console.SetCursorPosition(x, y + 2);
                Console.Write(" Rename file: ");
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.Write(GetNameForMenu(file.Name));
                Console.ForegroundColor = ConsoleColor.Black;
                Console.SetCursorPosition(30, y + 3);
                Console.Write(" Enter name: ");
                Console.CursorVisible = true;
                string nameFile = Console.ReadLine();
                Console.CursorVisible = false;
                Regex regex = new Regex(@"^[a-zA-Z0-9_.]+$");
                if (string.IsNullOrEmpty(nameFile) || !regex.IsMatch(nameFile))
                {
                    Console.SetCursorPosition(x + 20, y + 4);
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write("Invalid name!");
                    Console.ForegroundColor = ConsoleColor.Black;
                    Console.ReadKey();
                }
                else
                {
                    Console.SetCursorPosition(x, y + 4);
                    Console.Write("      Are you sure?");
                    int width = 8;
                    string[] menuOkCancel = new string[2] { " Cancel ", "   OK   " };
                    int nom = Menu.GorizontalMenu(menuOkCancel, width);
                    switch (nom)
                    {
                        case -1:
                            return;
                            break;
                        case 0:
                            return;
                            break;
                        case 1:
                            file.MoveTo(file.DirectoryName + @"\" + nameFile);
                            break;
                        default:
                            break;
                    }
                }
            }
            else
            {
                DirectoryInfo dir = new DirectoryInfo(name);
                string warning = "";
                if (dir.Attributes.ToString().Contains("Hidden") || dir.Attributes.ToString().Contains("System"))
                    warning = "!!! Directory has attributes:" + dir.Attributes;
                Console.SetCursorPosition(x, y + 1);
                if (warning != "")
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write(warning);
                    Console.ForegroundColor = ConsoleColor.Black;
                }
                Console.SetCursorPosition(x, y + 2);
                Console.Write(" Rename directory: ");
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.Write(GetNameForMenu(dir.Name));
                Console.ForegroundColor = ConsoleColor.Black;
                Console.SetCursorPosition(30, y + 3);
                Console.Write(" Enter name: ");
                Console.CursorVisible = true;
                string nameDir = Console.ReadLine();
                Console.CursorVisible = false;
                Regex regex = new Regex(@"^[a-zA-Z0-9_]+$");
                if (string.IsNullOrEmpty(nameDir) || !regex.IsMatch(nameDir))
                {
                    Console.SetCursorPosition(x + 20, y + 4);
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write("Invalid name!");
                    Console.ForegroundColor = ConsoleColor.Black;
                    Console.ReadKey();
                }
                else
                {
                    Console.SetCursorPosition(x, y + 4);
                    Console.Write("      Are you sure?");
                    int width = 8;
                    string[] menuOkCancel = new string[2] { " Cancel ", "   OK   " };
                    int nom = Menu.GorizontalMenu(menuOkCancel, width);
                    switch (nom)
                    {
                        case -1:
                            return;
                            break;
                        case 0:
                            return;
                            break;
                        case 1:                           
                            dir.Parent.CreateSubdirectory(nameDir);
                            DirectoryInfo dirNew = new DirectoryInfo(dir.Parent.FullName + @"\" + nameDir);
                            foreach (DirectoryInfo item in dir.GetDirectories())
                            {
                                MoveDir(item.FullName, dirNew);
                            }
                            foreach (FileInfo item in dir.GetFiles())
                            {
                                MoveFile(item.FullName, dirNew);
                            }
                            dir.Delete();
                            break;
                        default:
                            break;
                    }
                }
            }
           
             
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                
        }

        public static void F8Del(string type, string name)
        {
            if (type == "file")
            {
                FileInfo file = new FileInfo(name);    
                string warning = "";
                if (file.Attributes.ToString().Contains("Hidden") || file.Attributes.ToString().Contains("System")
                    || file.IsReadOnly)
                    warning = "!!! File has attributes:" + file.Attributes;                                
                int x = 30, y = 10;
                Console.BackgroundColor = ConsoleColor.Gray;
                Console.ForegroundColor = ConsoleColor.Black;
                Console.SetCursorPosition(x, y++);
                Console.Write(" ---------------------------------------------------------------------- ");
                if (warning != "")
                {
                    Console.SetCursorPosition(x, y++);
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    Console.Write(warning.PadRight(72));
                    Console.ForegroundColor = ConsoleColor.Black;
                }
                Console.SetCursorPosition(x, y);
                Console.Write("                                                                        ");
                Console.SetCursorPosition(x, y++);
                Console.Write(" Delete file ");
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.Write(GetNameForMenu(file.Name));
                Console.SetCursorPosition(x, y++);
                Console.Write("                                                                        ");
                Console.SetCursorPosition(x, y-1);                
                Console.ForegroundColor = ConsoleColor.Black;
                Console.Write("                      Are you sure?".PadRight(72));               
                Console.SetCursorPosition(x, y++);
                Console.Write("                                                                        ");
                Console.SetCursorPosition(x, y++);
                Console.Write("                                                                        ");
                Console.SetCursorPosition(x, y);
                Console.Write(" ---------------------------------------------------------------------- ");
                Console.SetCursorPosition(x + 20, y-1);
                int width = 8;
                string[] menuOkCancel = new string[2] { " Cancel ", "   OK   " };
                int nom = Menu.GorizontalMenu(menuOkCancel, width);
                switch (nom)
                {
                    case -1:
                        return;
                        break;
                    case 0:
                        return;
                        break;
                    case 1:
                        file.Delete();
                        break;
                    default:
                        break;
                }
            }
            else
            {
                DirectoryInfo dir = new DirectoryInfo(name);
                string warning = "";
                if (dir.Attributes.ToString().Contains("Hidden") || dir.Attributes.ToString().Contains("System"))
                    warning = "!!! File has attributes:" + dir.Attributes;
                int x = 30, y = 10;
                Console.BackgroundColor = ConsoleColor.Gray;
                Console.ForegroundColor = ConsoleColor.Black;
                Console.SetCursorPosition(x, y++);
                Console.Write(" ---------------------------------------------------------------------- ");
                if (warning != "")
                {
                    Console.SetCursorPosition(x, y++);
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    Console.Write(warning.PadRight(72));
                    Console.ForegroundColor = ConsoleColor.Black;
                }
                Console.SetCursorPosition(x, y);
                Console.Write("                                                                        ");
                Console.SetCursorPosition(x, y++);
                Console.Write(" Delete directory ");
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.Write(GetNameForMenu(dir.Name));
                Console.SetCursorPosition(x, y++);
                Console.Write("                                                                        ");
                Console.SetCursorPosition(x, y);
                Console.Write("                                                                        ");
                Console.SetCursorPosition(x, y-1);
                Console.ForegroundColor = ConsoleColor.Black;
                Console.Write("                      Are you sure?".PadRight(72));
                Console.SetCursorPosition(x, y+1);
                Console.Write(" ---------------------------------------------------------------------- ");
                Console.SetCursorPosition(x + 20, y);
                int width = 8;
                string[] menuOkCancel = new string[2] { " Cancel ", "   OK   " };
                int nom = Menu.GorizontalMenu(menuOkCancel, width);
                switch (nom)
                {
                    case -1:
                        return;
                        break;
                    case 0:
                        return;
                        break;
                    case 1:                        
                        DelDir(dir);                       
                        break;
                    default:
                        break;
                }
            }
        }

        public static void DrawRect(int x, int y, int w, int h)
        {
            Console.BackgroundColor = ConsoleColor.Gray;
            Console.ForegroundColor = ConsoleColor.Black;
            Console.SetCursorPosition(x, y);
            for (int i = 0; i < w; i++)
            {
                Console.Write("-");
            }            
            for (int i = 1; i < h; i++)
            {
                Console.SetCursorPosition(x, y+i);
                for (int j = 0; j < w; j++)
                {
                    Console.Write(" ");
                }
            }
            Console.SetCursorPosition(x, y + h);
            for (int i = 0; i < w; i++)
            {
                Console.Write("-");
            }
        }
        public static void F7MkDir(DirectoryInfo dirActive)
        {           
            DrawRect(30, 10, 70, 5);
            Console.SetCursorPosition(30, 11);
            Console.Write(" Create SUBdirectory  in: ");
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.Write(GetNameForMenu(dirActive.Name));
            Console.ForegroundColor = ConsoleColor.Black;
            Console.SetCursorPosition(30, 12);
            Console.Write(" Enter name: ");            
            Console.CursorVisible = true;
            string nameDir =  Console.ReadLine();
            Console.CursorVisible = false;
            Regex regex = new Regex(@"^[a-zA-Z0-9_]+$");
            if (string.IsNullOrEmpty(nameDir) || !regex.IsMatch(nameDir))
            {
                Console.SetCursorPosition(50, 14);
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write("Invalid name!");
                Console.ForegroundColor = ConsoleColor.Black;
                Console.ReadKey();
            }
            else
            {
                Console.SetCursorPosition(30, 14);
                Console.Write("      Are you sure?");
                int width = 8;
                string[] menuOkCancel = new string[2] { " Cancel ", "   OK   " };
                int nom = Menu.GorizontalMenu(menuOkCancel, width);
                switch (nom)
                {
                    case -1:
                        return;
                        break;
                    case 0:
                        return;
                        break;
                    case 1:
                        dirActive.CreateSubdirectory(nameDir);
                        break;
                    default:
                        break;
                }
            }
        }

        public static void MoveDir(string name, DirectoryInfo toDir)
        {
            DirectoryInfo dir = new DirectoryInfo(name); 
            toDir.CreateSubdirectory(dir.Name);
            DirectoryInfo dirNew = new DirectoryInfo(toDir.FullName + @"\" + dir.Name);
            CopyDir(name, dirNew.FullName);
            DelDir(dir);
                 
        }
        public static void MoveFile(string name, DirectoryInfo toDir)
        {
            FileInfo file = new FileInfo(name);
            FileInfo fileNew = new FileInfo(toDir.FullName + @"\" + file.Name);
            file.MoveTo(toDir.FullName + @"\" + file.Name);
        }
        public static void F6Move(string type, string name, DirectoryInfo dirInActive)
        {

            if (type == "file")
            {
                FileInfo file = new FileInfo(name);                
                string warning = "";
                if (file.Attributes.ToString().Contains("Hidden") || file.Attributes.ToString().Contains("System")
                    || file.IsReadOnly)
                    warning = "!!! File has attributes:" + file.Attributes;
                int x = 30, y = 10;
                Console.BackgroundColor = ConsoleColor.Gray;
                Console.ForegroundColor = ConsoleColor.Black;
                Console.SetCursorPosition(x, y++);
                Console.Write(" ---------------------------------------------------------------------- ");
                if (warning != "")
                {
                    Console.SetCursorPosition(x, y++);
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    Console.Write(warning.PadRight(72));
                    Console.ForegroundColor = ConsoleColor.Black;
                }
                Console.SetCursorPosition(x, y);
                Console.Write("                                                                        ");
                Console.SetCursorPosition(x, y++);
                Console.Write(" Move file ");
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.Write(file.Name);
                Console.SetCursorPosition(x, y++);
                Console.ForegroundColor = ConsoleColor.Black;
                Console.Write(" to directory: ");
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.Write(dirInActive.FullName.PadRight(57));
                Console.ForegroundColor = ConsoleColor.Black;
                Console.SetCursorPosition(x, y++);
                Console.Write("                      Are you sure?".PadRight(72));
                Console.SetCursorPosition(x, y++);
                Console.Write("                                                                        ");
                Console.SetCursorPosition(x, y++);
                Console.Write("                                                                        ");
                Console.SetCursorPosition(x, y);
                Console.Write(" ---------------------------------------------------------------------- ");
                Console.SetCursorPosition(x + 20, y - 2);
                int width = 8;
                string[] menuOkCancel = new string[2] { " Cancel ", "   OK   " };
                int nom = Menu.GorizontalMenu(menuOkCancel, width);
                switch (nom)
                {
                    case -1:
                        return;
                        break;
                    case 0:
                        return;
                        break;
                    case 1:
                        FileInfo fileNew = new FileInfo(dirInActive.FullName + @"\" + file.Name);
                        if (fileNew.Exists)
                        {
                            Console.SetCursorPosition(x + 20, y - 1);
                            Console.BackgroundColor = ConsoleColor.Gray;
                            Console.ForegroundColor = ConsoleColor.DarkRed;
                            Console.Write("File with name already exists");
                            Console.ReadKey();
                        }
                        else                      
                        file.MoveTo(dirInActive.FullName + @"\" + file.Name);
                        break;
                    default:
                        break;
                }
            }
            else
            {
                DirectoryInfo dir = new DirectoryInfo(name);
                string warning = "";
                if (dir.Attributes.ToString().Contains("Hidden") || dir.Attributes.ToString().Contains("System"))
                    warning = "!!! File has attributes:" + dir.Attributes;
                int x = 30, y = 10;
                Console.BackgroundColor = ConsoleColor.Gray;
                Console.ForegroundColor = ConsoleColor.Black;
                Console.SetCursorPosition(x, y++);
                Console.Write(" ---------------------------------------------------------------------- ");
                if (warning != "")
                {
                    Console.SetCursorPosition(x, y++);
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    Console.Write(warning.PadRight(72));
                    Console.ForegroundColor = ConsoleColor.Black;
                }
                Console.SetCursorPosition(x, y);
                Console.Write("                                                                        ");
                Console.SetCursorPosition(x, y++);
                Console.Write(" Move directory ");
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.Write(dir.Name);
                Console.SetCursorPosition(x, y++);
                Console.ForegroundColor = ConsoleColor.Black;
                Console.Write(" to directory: ");
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.Write(dirInActive.FullName.PadRight(57));
                Console.ForegroundColor = ConsoleColor.Black;
                Console.SetCursorPosition(x, y++);
                Console.Write("                      Are you sure?".PadRight(72));
                Console.SetCursorPosition(x, y++);
                Console.Write("                                                                        ");
                Console.SetCursorPosition(x, y++);
                Console.Write("                                                                        ");
                Console.SetCursorPosition(x, y);
                Console.Write(" ---------------------------------------------------------------------- ");
                Console.SetCursorPosition(x + 20, y - 2);
                int width = 8;
                string[] menuOkCancel = new string[2] { " Cancel ", "   OK   " };
                int nom = Menu.GorizontalMenu(menuOkCancel, width);
                switch (nom)
                {
                    case -1:
                        return;
                        break;
                    case 0:
                        return;
                        break;
                    case 1:
                        DirectoryInfo dirNew = new DirectoryInfo(dirInActive.FullName + @"\" + dir.Name);
                        if (dirNew.Exists)
                        {
                            Console.SetCursorPosition(x + 20, y - 1);
                            Console.BackgroundColor = ConsoleColor.Gray;
                            Console.ForegroundColor = ConsoleColor.DarkRed;
                            Console.Write("Directory with name already exists");
                            Console.ReadKey();
                        }
                        else
                        {
                            dirInActive.CreateSubdirectory(dir.Name);
                            CopyDir(name, dirNew.FullName);
                            DelDir(dir);
                        }
                        break;
                    default:
                        break;
                }
            }
        }

        public static void F5Copy(string type, string name, DirectoryInfo dirInActive )
        {
           
            if (type == "file")
            {
                FileInfo file = new FileInfo(name);
                string warning =""; 
                if (file.Attributes.ToString().Contains("Hidden") || file.Attributes.ToString().Contains("System")
                    || file.IsReadOnly)
                    warning = "!!! File has attributes:" + file.Attributes;
                int x=30, y = 10;                
                Console.BackgroundColor = ConsoleColor.Gray;
                Console.ForegroundColor = ConsoleColor.Black;
                Console.SetCursorPosition(x, y++);
                Console.Write(" ---------------------------------------------------------------------- ");
                if (warning != "")
                {
                    Console.SetCursorPosition(x, y++);
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    Console.Write(warning.PadRight(72));
                    Console.ForegroundColor = ConsoleColor.Black;
                }
                Console.SetCursorPosition(x, y);
                Console.Write("                                                                        ");
                Console.SetCursorPosition(x, y++);
                Console.Write(" Copy file ");
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.Write(file.Name);
                Console.SetCursorPosition(x, y++);
                Console.ForegroundColor = ConsoleColor.Black;
                Console.Write(" to directory: ");
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.Write(dirInActive.FullName.PadRight(57));
                Console.ForegroundColor = ConsoleColor.Black;                
                Console.SetCursorPosition(x, y++);
                Console.Write("                      Are you sure?".PadRight(72));
                Console.SetCursorPosition(x, y++);
                Console.Write("                                                                        ");
                Console.SetCursorPosition(x, y++);
                Console.Write("                                                                        ");
                Console.SetCursorPosition(x, y);
                Console.Write(" ---------------------------------------------------------------------- ");
                Console.SetCursorPosition(x + 20, y-2);
                int width = 8;
                string[] menuOkCancel = new string[2] { " Cancel ", "   OK   " };
                int nom = Menu.GorizontalMenu(menuOkCancel, width);
                switch (nom)
                {
                    case -1:
                        return;
                        break;
                    case 0:
                        return;
                        break;
                    case 1:
                        FileInfo fileNew = new FileInfo(dirInActive.FullName + @"\"+file.Name);
                        if (fileNew.Exists)
                        {
                            Console.SetCursorPosition(x + 20, y-1);
                            Console.BackgroundColor = ConsoleColor.Gray;
                            Console.ForegroundColor = ConsoleColor.DarkRed;
                            Console.Write("File with name already exists");
                            Console.ReadKey();
                        }
                        else
                        {
                            if (dirInActive.FullName.EndsWith(@"\"))
                                file.CopyTo(dirInActive.FullName + file.Name);
                            else
                            file.CopyTo(dirInActive.FullName + @"\" + file.Name);
                        }
                        
                        break;
                    default:                 
                        break;
                }
            }
            else
            {
                DirectoryInfo dir = new DirectoryInfo(name);
                string warning = "";
                if (dir.Attributes.ToString().Contains("Hidden") || dir.Attributes.ToString().Contains("System"))
                    warning = "!!! File has attributes:" + dir.Attributes;
                int x = 30, y = 10;
                Console.BackgroundColor = ConsoleColor.Gray;
                Console.ForegroundColor = ConsoleColor.Black;
                Console.SetCursorPosition(x, y++);
                Console.Write(" ---------------------------------------------------------------------- ");
                if (warning != "")
                {
                    Console.SetCursorPosition(x, y++);
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    Console.Write(warning.PadRight(72));
                    Console.ForegroundColor = ConsoleColor.Black;
                }
                Console.SetCursorPosition(x, y);
                Console.Write("                                                                        ");
                Console.SetCursorPosition(x, y++);
                Console.Write(" Copy directory ");
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.Write(dir.Name);
                Console.SetCursorPosition(x, y++);
                Console.ForegroundColor = ConsoleColor.Black;
                Console.Write(" to directory: ");
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.Write(dirInActive.FullName.PadRight(57));
                Console.ForegroundColor = ConsoleColor.Black;
                Console.SetCursorPosition(x, y++);
                Console.Write("                      Are you sure?".PadRight(72));
                Console.SetCursorPosition(x, y++);
                Console.Write("                                                                        ");
                Console.SetCursorPosition(x, y++);
                Console.Write("                                                                        ");
                Console.SetCursorPosition(x, y);
                Console.Write(" ---------------------------------------------------------------------- ");
                Console.SetCursorPosition(x + 20, y - 2);
                int width = 8;
                string[] menuOkCancel = new string[2] { " Cancel ", "   OK   " };
                int nom = Menu.GorizontalMenu(menuOkCancel, width);
                switch (nom)
                {
                    case -1:
                        return;
                        break;
                    case 0:
                        return;
                        break;
                    case 1:
                        DirectoryInfo dirNew = new DirectoryInfo(dirInActive.FullName + @"\"+dir.Name);
                        if (dirNew.Exists)
                        {
                            Console.SetCursorPosition(x + 20, y - 1);
                            Console.BackgroundColor = ConsoleColor.Gray;
                            Console.ForegroundColor = ConsoleColor.DarkRed;
                            Console.Write("Directory with name already exists");
                            Console.ReadKey();
                        }
                        else
                        {
                            dirInActive.CreateSubdirectory(dir.Name);
                            CopyDir(name, dirNew.FullName);
                        }   
                        break;
                    default:
                        break;
                }
            }
           
        }

        public static void F4Edit(string name)
        {
          if(!name.EndsWith(".exe"))
            try
           {
                Process.Start(name);
           }
            catch { };
           
        }

        public static void F3View(string name)
        {
          if (name.EndsWith(".txt") || name.EndsWith(".html")|| name.EndsWith(".log")|| name.EndsWith(".ini")|| name.EndsWith(".inf"))
            Process.Start("NotePad.exe", name);
        }

        public static void F2Drive(ref DirectoryInfo dirActive, char activePanel)
        {
            DriveInfo[] drives = DriveInfo.GetDrives();
            int kol = 0;
            foreach (DriveInfo item in drives)
            {
                if (item.IsReady) kol++;
            }
            string[] menuDrives = new string[kol];
            int j = 0;
            for (int i = 0; i < drives.Length; i++)
            {
                if (drives[i].IsReady)
                  menuDrives[j++] = " " + drives[i].Name[0].ToString() + " ";               
            }
            int x, y = 10;
            string panel;
            if (activePanel == 'r')
            {
                x = 85;
                panel = "right";
            }
            else
            {
                x = 20;
                panel = "left";
            }
            Console.BackgroundColor = ConsoleColor.Gray;
            Console.ForegroundColor = ConsoleColor.Black;
            Console.SetCursorPosition(x, y);
            Console.Write(" -------------------- ");
            Console.SetCursorPosition(x, y+1);
            Console.Write("  Choose ");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write(panel);
            Console.ForegroundColor = ConsoleColor.Black;          
            Console.Write(" drive: ");
            if (activePanel == 'l')
                Console.Write(" ");
                Console.SetCursorPosition(x, y+2);
            Console.Write("                      ");
            Console.SetCursorPosition(x, y+3);
            Console.Write("                      ");
            Console.SetCursorPosition(x, y + 4);
            Console.Write(" -------------------- ");
            Console.SetCursorPosition(x + 2, y + 3);
            int width=3;
            int nom = Menu.GorizontalMenu(menuDrives,width);
            switch (nom)
            {
                case -1:
                    return;
                    break;
                default:
                    dirActive = new DirectoryInfo(menuDrives[nom][1].ToString() + @":\");
                    break;
            }
        }

        public static void F1Info(string type, string name, char activePanel, ref int pos)
        {
            int x;
            if (activePanel == 'l')
                x = 66; 
            else x = 0;
            Console.SetCursorPosition(x, 3);          
            Console.BackgroundColor = ConsoleColor.DarkBlue;          
            Console.ForegroundColor = ConsoleColor.White;
           int y=3;
            Console.CursorVisible = false;
            for (int i = 0; i < 35; i++)
            {
                Console.SetCursorPosition(x, y+i);
                Console.Write("                                                              ");
            }
            Console.SetCursorPosition(x+20, 5);
            Console.Write(" Info");
            Console.SetCursorPosition(x, 6);
            Console.Write(" ----------------------------------------------------");
            DriveInfo drive = new DriveInfo(name[0].ToString());
            Console.SetCursorPosition(x+20, 7);
            Console.Write(drive.Name);
            Console.SetCursorPosition(x + 1, 7);
            Console.Write($"{drive.TotalSize:### ### ### ### ###} total bytes on dive {drive.Name}");
            Console.SetCursorPosition(x + 1, 8);
            Console.Write($"{drive.TotalFreeSpace:### ### ### ### ###} total bytes on dive {drive.Name}");
            Console.SetCursorPosition(x + 1, 9);
            Console.Write($"Volume Label: {drive.VolumeLabel}");
            Console.SetCursorPosition(x, 10);
            Console.Write(" ----------------------------------------------------");
            Console.SetCursorPosition(x + 1, 11);
            if (type == "DIR")
            {                
                DirectoryInfo dir = new DirectoryInfo(name);
                Console.Write(GetNameForMenu(dir.Name));
                y = 12;
                Console.SetCursorPosition(x + 1, y);
                if (name.Length <= 50)
                    Console.Write($"Full Name: {name}");
                else
                {
                    Console.Write($"Full Name: ");
                    int i = 0;
                    string temp = name;
                    do
                    {                        
                        Console.SetCursorPosition(x + 12, y+i);
                        i++;
                        string s = temp.Substring(0,50);
                        Console.Write(s);
                        temp = temp.Remove(0, 50);
                    } while (temp.Length > 50);
                    Console.SetCursorPosition(x + 12, y + i);
                    Console.Write(temp);
                }
                y= Console.CursorTop;
                Console.SetCursorPosition(x + 1, y + 1); 
                Console.Write($"Attributes: {dir.Attributes}");
                Console.SetCursorPosition(x + 1, y + 2);
                Console.Write($"Time of creation: {dir.CreationTime}");
                Console.SetCursorPosition(x + 1, y + 3);
                Console.Write($"Time of last write: {dir.LastWriteTime}");
                Console.SetCursorPosition(x + 1, y + 4);
                DirectoryInfo[] dirs = dir.GetDirectories();
                FileInfo[] files = dir.GetFiles();
                Console.Write($"Directory contains: {dirs.Count()} SUB directory");
                Console.SetCursorPosition(x + 21, y + 5);
                Console.Write($"{files.Count()} files");               
            }
            else
            {
               FileInfo file = new FileInfo(name);
                Console.Write(file.Name);
                y = 12;
                Console.SetCursorPosition(x + 1, y);
                if (name.Count() <= 53)
                    Console.Write($"Full Name: {name}");
                else
                {
                    Console.Write("Full Name: ");
                    int i = 0;
                    string temp = name;
                    do
                    {
                        Console.SetCursorPosition(x + 12, y + i);
                        i++;
                        string s = temp.Substring(0, 50);
                        Console.Write(s);
                        temp = temp.Remove(0, 50);
                    } while (temp.Count() > 50);
                    Console.SetCursorPosition(x + 11, y + i);
                    Console.Write(temp);
                }
                y = Console.CursorTop;
                Console.SetCursorPosition(x + 1, y + 1);
                Console.Write($"Extention of file: {file.Extension}");
                Console.SetCursorPosition(x + 1, y + 2);
                Console.Write($"Attributes: {file.Attributes}");
                Console.SetCursorPosition(x + 1, y + 3);
                Console.Write($"Time of creation: {file.CreationTime}");
                Console.SetCursorPosition(x + 1, y + 4);
                Console.Write($"Time of last change: {file.LastWriteTime}");
                Console.SetCursorPosition(x + 1, y + 5);
                Console.Write($"Size: {file.Length:### ### ### ###} bytes");
            }
            Console.ReadKey();
        }


        
    }

}
