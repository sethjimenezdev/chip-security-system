using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChipSecuritySystem
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("==== Welcome to the Chip Security System! ====");
            Console.WriteLine("This program generates a series of color chips and validates them against a master chip.");
            ChipService chipService = new ChipService();
            Console.WriteLine("The master chip is: [" + chipService.GetMasterChip() + "]");
            Console.WriteLine("Please enter number chips you want to provide between 2 to 12:");
            int numberOfChips;
            while (true)
            {
                if (int.TryParse(Console.ReadLine(), out numberOfChips) && numberOfChips >= 2 && numberOfChips <= 12)
                {
                    break;
                }
                else
                {
                    Console.WriteLine("Invalid input. Please enter a number between 2 and 12:");
                }
            }
            Console.WriteLine($"You entered: {numberOfChips}");

            chipService.CreateChipSeries(numberOfChips);
            Console.WriteLine("___ Generated Chips:___");
            Console.WriteLine(chipService.GetChipList(numberOfChips));

            Console.WriteLine("___Validating Chip Series:___");
            chipService.ValidateChipSeries();


            Console.WriteLine("=== Do you want to try again? (y/n)");
            string userInput = Console.ReadLine();
            if (userInput.Equals("y", StringComparison.OrdinalIgnoreCase))
            {
                Main(args); // Restart the process
            }
            else
            {
                Console.WriteLine("Exiting the program. Goodbye!");
            }
        }
    }
}
