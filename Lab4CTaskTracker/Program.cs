/*
 * You have been hired by a firm to provide a console-based prototype of a task tracking application. Based on product owner requirements, spend the next few days implementing a task tracking application.
   
Things to do so far:
    Need to figure out how to manipulate highlighted text on the console
    (stretch) get string commands for up arrows, down arrows and such.
    Do I need to rewrite the console text every time?
    highlight text
    strike through text
    read and write to data file
    How to keep track of data file and if an item has been actioned or deleted or pushed off to the future
    move between pages on the screen (15 items at a go)
    I am thinking the pointer to use is the one that has a pointer at the head and tail of the string to go to the next and previous items

 * */
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Lab4CTaskTracker
{
    class Program
    {
        public class Menu  //basic menu logic gotten from https://codereview.stackexchange.com/questions/198153/navigation-with-arrow-keys
        {
            public Menu(IEnumerable<string> items)
            {
                Items = items.ToArray();
            }


            public IReadOnlyList<string> Items { get; }

            public int SelectedIndex { get; private set; } = 0; // nothing selected

            public string SelectedOption => SelectedIndex != -1 ? Items[SelectedIndex] : null;

            public void MoveUp() => SelectedIndex = Math.Max(SelectedIndex - 1, 0);

            public void MoveDown() => SelectedIndex = Math.Min(SelectedIndex + 1, Items.Count - 1);

        }


        // logic for drawing menu list
        public class ConsoleMenuPainter
        {
            Menu menu;

            public ConsoleMenuPainter(Menu menu)
            {
                this.menu = menu;
            }

            public void Paint(int x, int y, ref List<string> taskStatus)
            {
                for (int i = 0; i < menu.Items.Count; i++)
                {
                    Console.SetCursorPosition(x, y + i);

                    if (menu.SelectedIndex == i)
                    {
                        TextColor(11, 1);
                    }
                    else
                    {
                        ToDoTextColor();
                        switch (taskStatus[i])
                        {
                            case "Incomplete":
                                IncompleteTextColor();
                                break;
                            case "Completed":
                                StrikeOutTextColor();
                                break;
                        }
                    }

                    Console.Write($"{i + 1}. ");
                    Console.WriteLine(menu.Items[i]);
                }
            }
        }


        public static void Main(string[] args)
        {
            //            ConsoleColorTextTest();
            //            Console.ReadLine();
            bool finished = false;

            List<string> taskList = new List<string>();
            List<string> taskStatus = new List<string>();
            string selectionChoice = "";
            finished = false;

            taskList.Add("");
            taskStatus.Add("");

            /* test code
               taskList.Add("Thing 1");
               taskStatus.Add("Completed");
               taskList.Add("Thing 2");
               taskStatus.Add("Incomplete");
               taskList.Add("Thing 3");
               taskStatus.Add("ToDo");
            */

            do
            {
                if (taskList[0] == "")
                {
                    Console.Clear();
                    Console.Write("Your Task list is empty!\nPlease enter something to do!: ");
                    taskList[0] = Console.ReadLine();
                    taskStatus[0] = "ToDo";
                }

                var menu = new Menu(taskList);
                selectionChoice = CallMenu(menu, ref taskStatus);
                switch (selectionChoice)
                {
                    case "Add":
                        Console.WriteLine();
                        Console.Write("     Enter something to do: ");
                        string input = "";
                        input = Console.ReadLine();
                        taskList.Add(input);
                        taskStatus.Add("ToDo");
                        Console.WriteLine("Items so far on the list");
                        int j = 0;
                        foreach (var task in taskList)
                        {
                            j++;
                            Console.WriteLine($"{j}. {task}");
                        }
                        break;
                    case "Completed":
                        taskStatus[menu.SelectedIndex] = "Completed";
                        break;
                    case "Incomplete":
                        taskList.Add(taskList[menu.SelectedIndex]);
                        taskStatus.Add("Incomplete");
                        taskList.RemoveAt(menu.SelectedIndex);
                        taskStatus.RemoveAt(menu.SelectedIndex);
                        break;

                    case "Quit":
                        WriteToFile(taskList, "taskList.txt");
                        WriteToFile(taskStatus, "taskStatus.txt");
                        Console.WriteLine("That's all folks!");
                        finished = true;
                        break;
                }
                bool firstItemComplete = true;
                while (firstItemComplete)
                {
                    if (taskStatus[0] == "Completed" && (taskStatus.Count > 1))
                    {
                        taskList.RemoveAt(0);
                        taskStatus.RemoveAt(0);
                    }
                    else if (taskStatus[0] == "Completed" && (taskStatus.Count == 1))
                    {
                        taskList[0] = "";
                        taskStatus[0] = "";
                    }
                    else
                    {
                        firstItemComplete = false;
                    }
                }
            } while (finished == false);
        }

        public static string CallMenu(Menu menu, ref List<string> taskStatus)
        {
            Console.Clear();
            var menuPainter = new ConsoleMenuPainter(menu);
            string selectionChoice = "";
            bool done = false;

            Console.WriteLine("Enter 'a' to add a task, 'c' to mark it complete");
            Console.WriteLine("'i' marks it incomplete, 'q' or 'Enter' exits");
            Console.WriteLine("You can use the up and down arrow keys to move through your list!");

            do
            {
                menuPainter.Paint(5, 7, ref taskStatus);

                var keyInfo = Console.ReadKey();

                switch (keyInfo.Key)
                {
                    case ConsoleKey.UpArrow:
                        menu.MoveUp();
                        break;
                    case ConsoleKey.DownArrow:
                        menu.MoveDown();
                        break;
                    //TODO         case ConsoleKey.RightArrow:
                    //             menu.PageRight();
                    //             break;
                    //TODO         case ConsoleKey.LeftArrow:
                    //             menu.PageLeft();
                    //             break;
                    case ConsoleKey.A:
                        selectionChoice = "Add";
                        done = true;
                        break;
                    case ConsoleKey.C:
                        selectionChoice = "Completed";
                        done = true;
                        break;
                    case ConsoleKey.I:
                        selectionChoice = "Incomplete";
                        done = true;
                        break;
                    case ConsoleKey.Q:
                        selectionChoice = "Quit";
                        done = true;
                        break;
                    case ConsoleKey.Enter:
                        selectionChoice = "Quit";
                        done = true;
                        break;
                }


                TextColor(11, 0);
                ClearCurrentConsoleLine();
                Console.WriteLine();
                ClearCurrentConsoleLine();
                Console.WriteLine("Selection Status:");
                ClearCurrentConsoleLine();
                Console.WriteLine($"{ taskStatus[menu.SelectedIndex]}: " + (menu.SelectedOption ?? "(nothing)"));
            }
            while (!done);
            return selectionChoice;
        }

        private static void ClearCurrentConsoleLine()
        {
            int currentLineCursor = Console.CursorTop;
            Console.SetCursorPosition(0, Console.CursorTop);
            Console.Write(new string(' ', Console.WindowWidth));
            Console.SetCursorPosition(5, currentLineCursor);
        }

        private static void TextColor(int fore = 15, int back = 0)
        {
            Console.ForegroundColor = (ConsoleColor)(fore);
            Console.BackgroundColor = (ConsoleColor)(back);
        }
        private static void ConsoleColorTextTest() //Test to see all the different colors available on the console
        {
            ConsoleColor[] colors = (ConsoleColor[])ConsoleColor.GetValues(typeof(ConsoleColor));

            Console.ResetColor();
            int i = 0;
            foreach (var color in colors)
            {
                Console.BackgroundColor = color;
                if (Console.BackgroundColor == Console.ForegroundColor)
                {
                    Console.ForegroundColor = colors[i + 3];
                }
                Console.WriteLine($"This background is color #{i} {color}");
                i++;
                Console.ResetColor();
            }
            i = 0;
            foreach (var color in colors)
            {
                Console.ForegroundColor = color;
                if (Console.BackgroundColor == Console.ForegroundColor)
                {
                    Console.BackgroundColor = colors[i + 3];
                }
                Console.WriteLine($"This text is color #{i} {color}");
                i++;
                Console.ResetColor();
            }
        }
        private static void ToDoTextColor() //Sets the console text to black background and Green Foreground
        {
            TextColor(10);
        }
        private static void IncompleteTextColor()
        {
            TextColor(14);
        }
        private static void StrikeOutTextColor() // Sets the console text color to Black background and Dark Gray text
        {
            TextColor(8);
        }
        private static void WriteToFile(List<string> listToWriteToFile, string fileName)
        {
            try
            {
                //Pass the filepath and filename to the StreamWriter Constructor
                StreamWriter sw = new StreamWriter(fileName);
                //Write a line of text
                foreach (var item in listToWriteToFile)
                {
                    sw.WriteLine(item);
                    Console.WriteLine(item);
                }
                //Close the file
                sw.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e.Message);
            }
            finally
            {
                Console.WriteLine("Executing finally block.");
            }
        }

        private static void ConsoleTextTest()
        {
            // setting the window size 
            Console.SetWindowSize(40, 40);

            // setting buffer size of console 
            Console.SetBufferSize(80, 80);

            // using the method 
            Console.SetCursorPosition(20, 20);
            Console.WriteLine("Hello GFG!");
            Console.Write("Press any key to continue . . . ");

            Console.ReadKey(true);
        }

        public static void ResizeStringArrayToArrayWithoutNulls(string[] oldArrays)
        {
            int nonNullInString = 0;
            foreach (var oldArray in oldArrays)
            {
                if (oldArray != null)
                    nonNullInString++;
            }

            string[] newArrays = new string[nonNullInString];
            nonNullInString = 0;
            foreach (var oldArray in oldArrays)
            {
                if (oldArray != null)
                {
                    newArrays[nonNullInString++] = oldArray;
                }
            }

            Array.Resize(ref oldArrays, nonNullInString);
            oldArrays = newArrays;
        }

    }
}
