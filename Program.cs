using AngleSharp;
using AngleSharp.Dom;

namespace Relaxator;

internal static class Program
{
    private static double _totalEarned;
    private static double _currencyRate;

    private static async Task Main(string[] args)
    {
        Console.CancelKeyPress += ConsoleOnCancelKeyPress;
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        int salary = 0;
        double delay = 0;
        
        if (ParseOptions(args, ref salary, ref delay) == false) return;

        _currencyRate = await GetCurrencyRate();
        
        const int secondsPerMinute = 60;
        const int minutesPerHour = 60;
        const int hoursPerDay = 9;
        const double averageWorkingDaysPerMonth = 21;

        double earningsPerDay = salary / averageWorkingDaysPerMonth;
        double earningsPerHour = earningsPerDay / hoursPerDay;
        double earningsPerMinute = earningsPerHour / minutesPerHour;
        double earningsPerSecond = earningsPerMinute / secondsPerMinute;
        
        double earnedPerDelay = earningsPerSecond * delay;
        _totalEarned = 0;

        string prevStrValue = "";

        Console.Clear();
        Console.WriteLine($"Current currency rate: {_currencyRate} RUB for 1 USD");

        while (true)
        {
            await Task.Delay(TimeSpan.FromSeconds(delay));
            
            _totalEarned += earnedPerDelay;
            string strValue = $"\rYou've earned: {Math.Round(_totalEarned, 2)}\ud83d\udcb2{(_currencyRate > 0 ? $" ({Math.Round(_totalEarned * _currencyRate, 2)}\u20bd)" : "")}";
            if (prevStrValue.Length > strValue.Length)
            {
                int delta = prevStrValue.Length - strValue.Length;
                for (int i = 0; i < delta; i++)
                {
                    strValue += " ";
                }
            }
            Console.Write(strValue);
            prevStrValue = strValue;
        }
    }

    private static void ConsoleOnCancelKeyPress(object? sender, ConsoleCancelEventArgs e)
    {
        Console.Clear();
        string strValue = $"\rYou've made: {Math.Round(_totalEarned, 2)}\ud83d\udcb2{(_currencyRate > 0 ? $" ({Math.Round(_totalEarned * _currencyRate, 2)}\u20bd)" : "")} \ud83c\udf89";
        Console.WriteLine(strValue);
        Environment.Exit(0);
    }

    private static bool ParseOptions(string[] args, ref int salary, ref double delay)
    {
        if (args.Length < 4)
        {
            PrintUsage();
            return false;
        }
        
        string[][] options = args.Chunk(2).ToArray();
        foreach (string[] option in options)
        {
            string name = option[0];
            string value = option[1];
            if (name is not ("-s" or "-d"))
            {
                PrintUsage();
                return false;
            }

            switch (name)
            {
                case "-s":
                {
                    Int32.TryParse(value, out salary);
                    break;
                }
                case "-d":
                {
                    Double.TryParse(value, out delay);
                    break;
                }
            }
        }

        if (salary > 0 && delay > 0) return true;
        
        PrintUsage();
        return false;
    }

    private static async Task<double> GetCurrencyRate()
    {
        try
        {
            IConfiguration config = Configuration.Default.WithDefaultLoader();
            const string address = "https://www.profinance.ru/currency_usd.asp";
            IBrowsingContext context = BrowsingContext.New(config);
            IDocument document = await context.OpenAsync(address);
            const string selector = "body > table:nth-child(2) > tbody > tr:nth-child(2) > td > table > tbody > tr:nth-child(2) > td.news > table:nth-child(4) > tbody > tr > td > table.curs > tbody > tr:nth-child(3) > td > table > tbody > tr:nth-child(2) > td:nth-child(1) > b > font";
            IElement? element = document.QuerySelector<IElement>(selector);
            string elementTextContent = element?.TextContent ?? "";
            bool tryParse = Double.TryParse(elementTextContent, out double currencyRate);
            if (tryParse == false)
            {
                return 0;
            }
        
            return currencyRate;
        }
        catch (Exception)
        {
            return 0;
        }
    }

    private static void PrintUsage()
    {
        Console.Write("USAGE: ./relaxator -s 100000 -d 1\nWHERE\ns - your salary in USD\nd - delay in seconds");
    }
}