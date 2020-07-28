/*
 * You have been hired by a firm to provide a console-based prototype of a task tracking application. Based on product owner requirements, spend the next few days implementing a task tracking application.
   
Things to do so far:
done!    Need to figure out how to manipulate highlighted text on the console
done!    (stretch) get string commands for up arrows, down arrows and such.
done!    Do I need to rewrite the console text every time?
done!    highlight text
Done!    strike through text
Done!    read and write to data file
stretch  data file for when an item has been actioned or deleted or pushed off to the future
Done!    move between pages on the screen (15 items at a go)
*/
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
        public class Menu  //basic menu logic gotten from https://codereview.stackexchange.com/questions/198153/navigation-with-arrow-keys and then modified for my task
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
        public class ConsoleMenuPainter //drawing menu list
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
                        TextColor(11, 1); //Good old C=64 colors, they make me happy
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
        public static void Main(string[] args) //Main menu, I didn't break this out further, but I probably should have
        {
            bool finished = false;

            List<string> taskList = new List<string>();  //creating my two lists, one for the tasks and the other
            List<string> taskStatus = new List<string>(); //for the status of the items
            string selectionChoice = "";
            finished = false;

            ReadFromFile(ref taskList, ref taskStatus, "taskTracker.txt");  //pulls data from the drive if it is there

            if (taskList.Count == 0) //case for when this is a new list
            {
                taskList.Add("");
                taskStatus.Add("");
            }

            int currentPage = 0;  //Variable to keep track of which page we are on so I know which parts of the data to display

            do
            {
                if (taskList[0] == "")  //case for when the list is empty (or the first time for a new list)
                {
                    Console.Clear();
                    Console.Write("Your Task list is empty!\nPlease enter something to do!: ");
                    taskList[0] = Console.ReadLine();
                    taskStatus[0] = "ToDo";
                }

                int maxPage = taskList.Count() / 15;  //set up variable to make sure we don't exceed the maximum possible total of possible pages
                int maxPageAdjust = 0;  //initialize and set correction variable for edge case to report the proper number of pages when the number of pages is divisible by 15.  Damn you off by one errors!
                if ((taskList.Count() % 15) == 0)
                {
                    maxPageAdjust--;
                }


                List<string> pageTaskList = new List<string>();  //lists for the pages in case there are more than 15 tasks
                List<string> pageTaskStatus = new List<string>();

                foreach (var item in taskList.Skip(15 * currentPage).Take(15)) //only pull the part of the larger list that we need
                {
                    pageTaskList.Add(item);
                }

                foreach (var item in taskStatus.Skip(15 * currentPage).Take(15)) //same, but for the status list
                {
                    pageTaskStatus.Add(item);
                }

                var menu = new Menu(pageTaskList);  //sends the page list to be displayed
                selectionChoice = CallMenu(menu, ref pageTaskStatus, ref currentPage, ref maxPage, ref maxPageAdjust);
                switch (selectionChoice) //what do we do based on the user's input?
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
                        WriteToFile(taskList, taskStatus, "taskTracker.txt");
                        //                 WriteToFile(taskStatus, "taskStatus.txt");
                        Console.WriteLine("That's all folks!\nYou may now close the window.");
                        finished = true;
                        break;
                }
                bool firstItemComplete = true; //Here is where we clean the list of the completed items at the top
                while (firstItemComplete)
                {
                    if (taskStatus[0] == "Completed" && (taskStatus.Count > 1))
                    {
                        taskList.RemoveAt(0);
                        taskStatus.RemoveAt(0);
                    }
                    else if (taskStatus[0] == "Completed" && (taskStatus.Count == 1))  // edge case for compeleting the last item on the list, set it as a new list
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
        //this does the work of setting up the menu display and instructions to the user
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

                var keyInfo = Console.ReadKey();  //read inputs and sends back the response
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

                //text at the bottom of the display, shows what item is selected, what its
                //status is, and what page you are on out of x number of possible 15 item pages
                TextColor();
                ClearCurrentConsoleLine();
                Console.WriteLine();
                ClearCurrentConsoleLine();
                Console.WriteLine("Selection Status:");
                ClearCurrentConsoleLine();
                Console.WriteLine($"{menu.SelectedIndex + 1 + (15 * currentPage)}: " + (menu.SelectedOption ?? "(nothing)"));
                int tempMaxPage = maxPage + maxPageAdjust;
                Console.WriteLine($"\n     You are on Page {currentPage + 1} of {tempMaxPage + 1}");
            }
            while (!done);
            return selectionChoice;
        }

        private static void ClearCurrentConsoleLine() //handy to clear just a line, not the entire screen
        {
            int currentLineCursor = Console.CursorTop;
            Console.SetCursorPosition(0, Console.CursorTop);
            Console.Write(new string(' ', Console.WindowWidth));
            Console.SetCursorPosition(5, currentLineCursor);
        }

        private static void TextColor(int fore = 15, int back = 0) //main way I change text color, set for overloading
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
        private static void IncompleteTextColor() //sets the console text to yellow with a black background
        {
            TextColor(14);
        }
        private static void StrikeOutTextColor() // Sets the console text color to Black background and Dark Gray text
        {
            TextColor(8);
        }
        private static void WriteToFile(List<string> listToWriteToFile1, List<string> listToWriteToFile2, string fileName) //writes data to a file on the disk
        {
            try
            {
                //Pass the filepath and filename to the StreamWriter Constructor
                StreamWriter sw = new StreamWriter(fileName);
                //Write a line of text
                int i = 0;
                foreach (var item in listToWriteToFile1)
                {
                    sw.WriteLine(item);
                    sw.WriteLine(listToWriteToFile2[i]);
                    i++;
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
        private static void ReadFromFile(ref List<string> listToReadFromFile1, ref List<string> listToReadFromFile2, string fileName) //reads the data from a file on the disk
        {
            string inputString1 = "";
            string inputString2 = "";
            try
            {
                int i = 0;
                using (StreamReader sr = new StreamReader(fileName))
                {
                    while (sr.Peek() >= 0)
                    {
                        inputString1 = sr.ReadLine();
                        inputString2 = sr.ReadLine();
                        listToReadFromFile1.Add(inputString1);
                        listToReadFromFile2.Add(inputString2);
                        i++;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("The process failed: {0}", e.ToString());
            }
        }
    }
}
