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
using System.IO;

namespace Lab4CTaskTracker
{
    class Program
    {
        static void Main(string[] args)
        {
            string message = "testing";
            string color = "Red";
            string color2 = "Blue";

            //            ConsoleTextTest();
            //WriteToFile();
            Console.WriteLine("Initial colors");
            ConsoleForegroundColor(color);
            ConsoleBackgroundColor(color2);
            DefaultColorColor();
            Console.WriteLine("More text");

            Console.WriteLine(message);
        }

        private static void DefaultColorColor()
        {
            ConsoleBackgroundColor("Black");
            ConsoleForegroundColor("White");
        } // Sets the console text color to Black background and white text
        private static void ConsoleForegroundColor(string color)
        {
            if (color == "Black")
                Console.ForegroundColor = ConsoleColor.Black;
            if (color == "Blue")
                Console.ForegroundColor = ConsoleColor.Blue;
            if (color == "Cyan")
                Console.ForegroundColor = ConsoleColor.Cyan;
            if (color == "DarkGray")
                Console.ForegroundColor = ConsoleColor.DarkGray;
            if (color == "DarkBlue")
                Console.ForegroundColor = ConsoleColor.DarkBlue;
            if (color == "DarkCyan")
                Console.ForegroundColor = ConsoleColor.DarkCyan;
            if (color == "DarkGreen")
                Console.ForegroundColor = ConsoleColor.DarkGreen;
            if (color == "DarkMagenta")
                Console.ForegroundColor = ConsoleColor.DarkMagenta;
            if (color == "DarkRed")
                Console.ForegroundColor = ConsoleColor.DarkRed;
            if (color == "DarkYellow")
                Console.ForegroundColor = ConsoleColor.DarkYellow;
            if (color == "Green")
                Console.ForegroundColor = ConsoleColor.Green;
            if (color == "Gray")
                Console.ForegroundColor = ConsoleColor.Gray;
            if (color == "Magenta")
                Console.ForegroundColor = ConsoleColor.Magenta;
            if (color == "Red")
                Console.ForegroundColor = ConsoleColor.Red;
            if (color == "White")
                Console.ForegroundColor = ConsoleColor.White;
            if (color == "Yellow")
                Console.ForegroundColor = ConsoleColor.Yellow;
        } // Sets the foreground text color
        private static void ConsoleBackgroundColor(string color)
        {
            if (color == "Black")
                Console.BackgroundColor = ConsoleColor.Black;
            if (color == "Blue")
                Console.BackgroundColor = ConsoleColor.Blue;
            if (color == "Cyan")
                Console.BackgroundColor = ConsoleColor.Cyan;
            if (color == "DarkGray")
                Console.BackgroundColor = ConsoleColor.DarkGray;
            if (color == "DarkBlue")
                Console.BackgroundColor = ConsoleColor.DarkBlue;
            if (color == "DarkCyan")
                Console.BackgroundColor = ConsoleColor.DarkCyan;
            if (color == "DarkGreen")
                Console.BackgroundColor = ConsoleColor.DarkGreen;
            if (color == "DarkMagenta")
                Console.BackgroundColor = ConsoleColor.DarkMagenta;
            if (color == "DarkRed")
                Console.BackgroundColor = ConsoleColor.DarkRed;
            if (color == "DarkYellow")
                Console.BackgroundColor = ConsoleColor.DarkYellow;
            if (color == "Green")
                Console.BackgroundColor = ConsoleColor.Green;
            if (color == "Gray")
                Console.BackgroundColor = ConsoleColor.Gray;
            if (color == "Magenta")
                Console.BackgroundColor = ConsoleColor.Magenta;
            if (color == "Red")
                Console.BackgroundColor = ConsoleColor.Red;
            if (color == "White")
                Console.BackgroundColor = ConsoleColor.White;
            if (color == "Yellow")
                Console.BackgroundColor = ConsoleColor.Yellow;
        } // Sets the background text color


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
    }
}
