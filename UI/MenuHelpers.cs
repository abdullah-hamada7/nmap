namespace NmapCli.UI;

public static class MenuHelpers
{
    public static void ShowHeader(string title)
    {
        Console.Clear();
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("╔" + "═".PadRight(78, '═') + "╗");
        Console.WriteLine("║" + title.PadLeft((78 + title.Length) / 2).PadRight(78) + "║");
        Console.WriteLine("╚" + "═".PadRight(78, '═') + "╝");
        Console.ResetColor();
        Console.WriteLine();
    }

    public static int GetMenuChoice(string prompt, int min, int max)
    {
        while (true)
        {
            Console.Write($"{prompt} [{min}-{max}]: ");
            if (int.TryParse(Console.ReadLine(), out int choice) && choice >= min && choice <= max)
            {
                return choice;
            }
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Invalid choice. Please enter a number between {min} and {max}.");
            Console.ResetColor();
        }
    }

    public static string GetInput(string prompt, bool allowEmpty = false)
    {
        while (true)
        {
            Console.Write($"{prompt}: ");
            var input = Console.ReadLine() ?? string.Empty;
            
            if (!string.IsNullOrWhiteSpace(input) || allowEmpty)
            {
                return input.Trim();
            }

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Input cannot be empty. Please try again.");
            Console.ResetColor();
        }
    }

    public static bool GetYesNo(string prompt, bool defaultValue = false)
    {
        var defaultText = defaultValue ? "Y/n" : "y/N";
        Console.Write($"{prompt} [{defaultText}]: ");
        var input = Console.ReadLine()?.Trim().ToLower();

        if (string.IsNullOrWhiteSpace(input))
            return defaultValue;

        return input == "y" || input == "yes";
    }

    public static void ShowError(string message)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"ERROR: {message}");
        Console.ResetColor();
    }

    public static void ShowSuccess(string message)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"SUCCESS: {message}");
        Console.ResetColor();
    }

    public static void ShowWarning(string message)
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine($"WARNING: {message}");
        Console.ResetColor();
    }

    public static void PressAnyKey(string message = "Press any key to continue...")
    {
        Console.WriteLine();
        Console.WriteLine(message);
        Console.ReadKey(true);
    }
}
