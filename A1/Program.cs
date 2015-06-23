using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using A1ClassLibrary;

namespace A1
{
    class Program
    {
        static void Main(string[] args)
        {
            Facade app = new Facade();

            string selectionString;
            int menuSelect = 0;

            // Entering main program menu loop
            do
            {
                Facade.DisplayMainMenu();
                selectionString = Console.ReadLine();

                try
                {
                    menuSelect = Int32.Parse(selectionString);
                }
                catch (FormatException)
                {
                    Console.WriteLine("\nInvalid option entered. Please make sure you are entering an integer.\n");
                    continue;
                }

                switch (menuSelect)
                {
                    case 1:
                        if (app.AddToCart())
                            Console.WriteLine("\nItems(s) successfully added to the cart.");
                        break;
                    case 2:
                        app.RemoveFromCart();
                        break;
                    case 3:
                        app.DisplayCartSummary();
                        break;
                    case 4:
                        app.ProcessCart();
                        break;
                    default:
                        Console.WriteLine("\nInvalid option entered. Valid menu options are 1,2,3,4 and 5\n");
                        break;
                }
            } while (menuSelect != 5);

            Console.WriteLine("Program is exiting......");
            Environment.Exit(0);
        }
    }
}
