using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Keyboard;

namespace NoAFKPlease
{
    public class Program
    {
        public const uint minWaitTimeSeconds = 30;
        public const uint maxWaitTimeSeconds = 59*4;
        
        private static List<char> keys = new List<char> {
            'W',
            'A',
            'S',
            'D',
            ' '
        };

        private static Random random = new Random();

        public static void Main(string[] args)
        {
            Console.WriteLine("Starting anti-afk (to stop, focus this window and press Ctrl+C)");

            CancellationTokenSource cts = new CancellationTokenSource();
            Task loopTask = Task.Run(() => Loop(cts.Token));

            var key = Console.ReadKey();
            while (!key.Modifiers.HasFlag(ConsoleModifiers.Control))
            {
                key = Console.ReadKey();
            }
            cts.Cancel();

            Console.WriteLine("Program stopped, press any button to close this window");
            Console.ReadKey();
        }

        private static async Task Loop(CancellationToken cancellationToken)
        {
            Console.WriteLine("Will press space and then start in 5 seconds...");

            await Task.Delay(TimeSpan.FromSeconds(5), cancellationToken);
            await KeyboardEmulator.HoldKey(' ', HumanKeyHoldTime());
            
            Console.WriteLine("Pressed space, starting now...");

            while (!cancellationToken.IsCancellationRequested)
            {
                var waitTime = minWaitTimeSeconds+random.NextDouble()*(maxWaitTimeSeconds-minWaitTimeSeconds);
                var span = TimeSpan.FromSeconds(waitTime);
                Console.WriteLine($"Waiting ({NumberTimeFormat(span.Minutes)}:{NumberTimeFormat(span.Seconds)})");
                Console.WriteLine();

                await Task.Delay(span, cancellationToken);

                var keyToPress = keys[random.Next(0, keys.Count)];
                Console.WriteLine($"Performing an anti-afk action |{keyToPress}|");
                await KeyboardEmulator.HoldKey(keyToPress, HumanKeyHoldTime());

                
            }
        }

        private static TimeSpan HumanKeyHoldTime(Random rng = null)
        {
            if (rng == null)
            {
                rng = new Random();
            }

            var ms = Math.Round(30 + rng.NextDouble() * 150);
            return TimeSpan.FromMilliseconds((int)ms);
        }

        private static string NumberTimeFormat(double numberToBeRounded)
        {
            var str = ((int)Math.Round(numberToBeRounded)).ToString();
            if (str.Length == 2)
            {
                return str;
            }

            if (str.Length == 1)
            {
                return $"0{str}";
            }

            return str;
        }
    }
}

// dotnet build
// dotnet publish -c Release