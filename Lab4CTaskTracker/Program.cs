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

            public void Paint(int x, int y, ref List<string> pageTaskStatus, ref int currentPage)
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
                        switch (pageTaskStatus[i])
                        {
                            case "Incomplete":
                                IncompleteTextColor();
                                break;
                            case "Completed":
                                StrikeOutTextColor();
                                break;
                        }
                    }

                    Console.Write($"{i + 1 + currentPage * 15}. ");
                    Console.WriteLine(menu.Items[i]);
                }
            }
        }


        public static void Main(string[] args)
        {
            //   int test = 59 % 15;
            //   int test2 = 59 / 15;
            //   Console.WriteLine($"59 % 15 = {test}");
            //   Console.WriteLine($"59 / 15 = {test2}");
            //               ConsoleColorTextTest();
            //                  Console.ReadLine();


            bool finished = false;

            List<string> taskList = new List<string>();
            List<string> taskStatus = new List<string>();
            string selectionChoice = "";
            finished = false;

            ReadFromFile(ref taskList, "taskList.txt");
            ReadFromFile(ref taskStatus, "taskStatus.txt");

            if (taskList.Count == 0)
            {
                taskList.Add("");
                taskStatus.Add("");
            }

            /* test code
               taskList.Add("Thing 1");
               taskStatus.Add("Completed");
               taskList.Add("Thing 2");
               taskStatus.Add("Incomplete");
               taskList.Add("Thing 3");
               taskStatus.Add("ToDo");
            */

            int currentPage = 0;

            do
            {
                if (taskList[0] == "")
                {
                    Console.Clear();
                    Console.Write("Your Task list is empty!\nPlease enter something to do!: ");
                    taskList[0] = Console.ReadLine();
                    taskStatus[0] = "ToDo";
                }

                int maxPage = taskList.Count() / 15;
                int maxPageAdjust = 0;
                if ((taskList.Count() % 15) == 0)
                {
                    maxPageAdjust--;
                }
                

                List<string> pageTaskList = new List<string>();
                List<string> pageTaskStatus = new List<string>();

                foreach (var item in taskList.Skip(15 * currentPage).Take(15))
                {
                    pageTaskList.Add(item);
                }

                foreach (var item in taskStatus.Skip(15 * currentPage).Take(15))
                {
                    pageTaskStatus.Add(item);
                }

                /*
                if (maxPage > 0)
                {
                    for (int i = 0; i < 15; i++)
                    {
                        if (taskStatus[i + (currentPage * 15)] != null)
                        {
                            pageTaskStatus.Add(taskStatus[i + (currentPage * 15)]);
                            pageTaskList.Add(taskList[i + (currentPage * 15)]);
                        }
                    }
                }
                else
                {
                    pageTaskList = taskList;
                    pageTaskStatus = taskStatus;
                }
                */

                //        foreach (var item in pageTaskList)
                //        {
                //            Console.WriteLine(item);
                //        }

                var menu = new Menu(pageTaskList);
                selectionChoice = CallMenu(menu, ref pageTaskStatus, ref currentPage, ref maxPage, ref maxPageAdjust);
                switch (selectionChoice)
                {
                    case "Add":
                        TextColor();
                        Console.WriteLine();
                        Console.Write("     Enter something to do: ");
                        ToDoTextColor();
                        string input = "";
                        input = Console.ReadLine();
                        TextColor();
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
                        taskStatus[menu.SelectedIndex + (15 * currentPage)] = "Completed";
                        break;
                    case "Incomplete":
                        taskList.Add(taskList[menu.SelectedIndex + (15 * currentPage)]);
                        taskStatus.Add("Incomplete");
                        taskList.RemoveAt(menu.SelectedIndex + (15 * currentPage));
                        taskStatus.RemoveAt(menu.SelectedIndex + (15 * currentPage));
                        break;
                    case "PageDown":
                        currentPage--;
                        break;
                    case "PageUp":
                        if (((taskList.Count() % 15) == 0) && (currentPage == (maxPage - 1)))
                        { }
                        else
                        {
                            currentPage++;
                        }
                        break;
                    case "Quit":
                        WriteToFile(taskList, "taskList.txt");
                        WriteToFile(taskStatus, "taskStatus.txt");
                        Console.WriteLine("That's all folks!\nYou may now close the window.");
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

        public static string CallMenu(Menu menu, ref List<string> pageTaskStatus, ref int currentPage, ref int maxPage, ref int maxPageAdjust)
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
                menuPainter.Paint(5, 5, ref pageTaskStatus, ref currentPage);

                var keyInfo = Console.ReadKey();

                switch (keyInfo.Key)
                {
                    case ConsoleKey.UpArrow:
                        menu.MoveUp();
                        break;
                    case ConsoleKey.DownArrow:
                        menu.MoveDown();
                        break;
                    case ConsoleKey.RightArrow:
                        if (currentPage < maxPage)
                        {
                            selectionChoice = "PageUp";
                            done = true;
                        }
                        break;
                    case ConsoleKey.LeftArrow:
                        if (currentPage > 0)
                        {
                            selectionChoice = "PageDown";
                            done = true;
                        }
                        break;
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


                TextColor();
                ClearCurrentConsoleLine();
                Console.WriteLine();
                ClearCurrentConsoleLine();
                Console.WriteLine("Selection Status:");
                ClearCurrentConsoleLine();
                Console.WriteLine($"{menu.SelectedIndex + 1 + (15 * currentPage)}: " + (menu.SelectedOption ?? "(nothing)"));
                int tempMaxPage = maxPage +  maxPageAdjust;

                Console.WriteLine($"You are on Page {currentPage + 1} of {tempMaxPage + 1}");
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
                }
                //Close the file
                sw.Close();
                Console.WriteLine($"The file {fileName} was written to successfully!");
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e.Message);
            }
            finally
            {
                Console.WriteLine("Hitting the writing finally block");
            }
        }
        private static void ReadFromFile(ref List<string> listToReadFromFile, string fileName)
        {
            string inputString = "";
            try
            {
                using (StreamReader sr = new StreamReader(fileName))
                {
                    while (sr.Peek() >= 0)
                    {
                        inputString = sr.ReadLine();
                        listToReadFromFile.Add(inputString);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("The process failed: {0}", e.ToString());
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
