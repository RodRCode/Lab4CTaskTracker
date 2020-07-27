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

            public int SelectedIndex { get; private set; } = -1; // nothing selected

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

            public void Paint(int x, int y)
            {
                for (int i = 0; i < menu.Items.Count; i++)
                {
                    Console.SetCursorPosition(x, y + i);

                    //                  var color = menu.SelectedIndex == i ? ConsoleColor.Yellow : ConsoleColor.Gray;
                    if (menu.SelectedIndex == i)
                    {
                        TextColor(11, 1);
                    }
                    else
                    {
                        TextColor();
                    }

                    //                    Console.ForegroundColor = color;
                    Console.Write($"{i + 1}. ");
                    Console.WriteLine(menu.Items[i]);
                }
            }
        }


        public static void Main(string[] args)
        {

            var menu = new Menu(new string[] { "John", "Bill", "Janusz", "Grażyna", "1500", ":)" });
            var toDo = new { Task = "Thing 1", Status = "G" };
            //toDo[] tasks = new toDo[10];

            string result = "";

            result = CallMenu(menu);

            menu = new Menu(new string[] { "Luke", "Leia", "C3PO", "R2D2", "Darth Vader", "Yoda", "Boba Fett", "Chewbacca", "Old Ben Kenobi", "Red 5", "Han Solo", "The Emperor", "Admiral Ackbar", "Mace Windu", "The quick brown fox jumps over the lazy dog" });

            result = CallMenu(menu);

            Console.WriteLine($"You selected {menu.SelectedOption}, it was item number {menu.SelectedIndex + 1}");
            Console.ReadKey();

            Console.WriteLine($"Here are the items from the anonymous class object, task: {toDo.Task}, and status: {toDo.Status}");
        }

        private static string CallMenu(Menu menu)
        {
            Console.Clear();
            var menuPainter = new ConsoleMenuPainter(menu);

            bool done = false;

            do
            {
                menuPainter.Paint(8, 5);

                var keyInfo = Console.ReadKey();

                switch (keyInfo.Key)
                {
                    case ConsoleKey.UpArrow:
                        menu.MoveUp();
                        break;
                    case ConsoleKey.DownArrow:
                        menu.MoveDown();
                        break;
                    case ConsoleKey.Enter:
                        done = true;
                        break;
                }

                TextColor(11, 0);
                ClearCurrentConsoleLine();
                Console.WriteLine($"Selected option #{menu.SelectedIndex + 1}: " + (menu.SelectedOption ?? "(nothing)"));
            }
            while (!done);
            return (menu.SelectedOption);
        }

        private static void ClearCurrentConsoleLine()
        {
            int currentLineCursor = Console.CursorTop;
            Console.SetCursorPosition(0, Console.CursorTop);
            Console.Write(new string(' ', Console.WindowWidth));
            Console.SetCursorPosition(8, currentLineCursor);
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
        private static void StrikeOutTextColor() // Sets the console text color to Black background and Dark Gray text
        {
            TextColor(8);
        }
        private static void WriteToFile()
        {
            try
            {
                //Pass the filepath and filename to the StreamWriter Constructor
                StreamWriter sw = new StreamWriter("Test.txt");
                //Write a line of text
                sw.WriteLine("Hello World!!");
                //Write a second line of text
                sw.WriteLine("From the StreamWriter class");
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
