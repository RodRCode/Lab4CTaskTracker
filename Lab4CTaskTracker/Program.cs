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
            //            ConsoleTextTest();
            WriteToFile();
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
    }
}
