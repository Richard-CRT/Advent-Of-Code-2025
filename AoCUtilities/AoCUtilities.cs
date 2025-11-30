//#define OVERRIDE

using System.Collections;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;

namespace AdventOfCodeUtilities
{
    // Way to disable warnings in code, e.g. CS8321 (function declared but never used)
    // #pragma warning disable 8321
    // <function>
    // #pragma warning restore 8321

    // Useful characters for debug and visualisation
    // ┘ ┐ └ ┌ │ ─ 
    // ┤ ┴ ├ ┬ ┼
    //
    // ╝ ╗ ╚ ╔ ║ ═
    // ╣ ╩ ╠ ╦ ╬
    //
    // ╛ ╕ ╘ ╒
    // ╡ ╧ ╞ ╤ ╪
    //
    // ╜ ╖ ╙ ╓
    // ╢ ╨ ╟ ╥ ╫
    //
    // █▓▒░
    // ▄▌▀▐
    // ♥♦♣♠

    public static class AoC
    {
        public static Int64 GCF(Int64 a, Int64 b)
        {
            while (b != 0)
            {
                Int64 temp = b;
                b = a % b;
                a = temp;
            }
            return a;
        }

        public static Int64 LCM(Int64 a, Int64 b)
        {
            return (a / GCF(a, b)) * b;
        }

        public static Int64 LCM(params Int64[] nums)
        {
            return nums.Aggregate(1, (Int64 lcm, Int64 num) => LCM(lcm, num));
        }

        static public void DebugClear()
        {
#if DEBUG || OVERRIDE
            Console.Clear();
#endif
        }

        static public string? DebugReadLine()
        {
#if DEBUG || OVERRIDE
            return Console.ReadLine();
#else
            return "";
#endif
        }

        static public void DebugWriteLine()
        {
#if DEBUG || OVERRIDE
            Console.WriteLine();
#endif
        }

        static public void DebugWriteLine(string text, params object[] args)
        {
#if DEBUG || OVERRIDE
            string lineToWrite = string.Format(text, args);
            Console.WriteLine(lineToWrite);
#endif
        }

        static public void DebugWrite(string text, params object[] args)
        {
#if DEBUG || OVERRIDE
            string lineToWrite = string.Format(text, args);
            Console.Write(lineToWrite);
#endif
        }

        static public List<string> GetInputLines(string filename = "input.txt")
        {
            var inputFile = File.ReadAllLines(filename);
            return inputFile.ToList();
        }

        static public string GetInput(string filename = "input.txt")
        {
            var inputFile = File.ReadAllText(filename);
            return inputFile;
        }

        static public MatchCollection RegexMatch(string input, string pattern, bool multiline = false)
        {
            RegexOptions options;
            if (multiline)
                options = RegexOptions.Multiline;
            else
                options = RegexOptions.Singleline;

            return Regex.Matches(input, pattern, options);
        }

        static public string MD5(string input)
        {
            // Use input string to calculate MD5 hash
            using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
            {
                byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                return Convert.ToHexString(hashBytes);
            }
        }

        static public double RunWithStopwatch(Action action, Int64 repeats)
        {
            Stopwatch sw = Stopwatch.StartNew();
            for (Int64 i = 0; i < repeats; i++)
                action();
            sw.Stop();
            double elapsedSeconds = (sw.ElapsedTicks / (double)Stopwatch.Frequency);
            double secondsPerRun = elapsedSeconds / repeats;
            return secondsPerRun;
        }

        /*
        // Example
        list_of_something.OrderByLambda(something => something.interesting_field, (x, y) => {
	        if (x > y)
		        return 1;
	        else if (x < y)
		        return -1;
	        else
		        return 0;
        });
        */
        static public IOrderedEnumerable<TSource> OrderByLambda<TSource, TKey>(
            this IEnumerable<TSource> source,
            Func<TSource, TKey> keySelector,
            Func<TKey?, TKey?, int> compareFunc)
        => source.OrderBy(keySelector, new AoCComparer<TKey>(compareFunc, false));

        static public IOrderedEnumerable<TSource> OrderByLambdaDescending<TSource, TKey>(
            this IEnumerable<TSource> source,
            Func<TSource, TKey> keySelector,
            Func<TKey?, TKey?, int> compareFunc)
        => source.OrderBy(keySelector, new AoCComparer<TKey>(compareFunc, true));

        public static void ForEach<T>(
            this IEnumerable<T> source,
            Action<T> action)
        {
            foreach (T element in source)
                action(element);
        }
    }

    internal class AoCComparer<TKey> : IComparer<TKey>
    {
        private readonly Func<TKey?, TKey?, int> _compareFunc;
        private readonly bool _invert;

        public AoCComparer(Func<TKey?, TKey?, int> compareFunc, bool invert)
        {
            _compareFunc = compareFunc;
            _invert = invert;
        }

        public int Compare(TKey? x, TKey? y)
        {
            int result = _compareFunc(x, y);
            return _invert ? result * -1 : result;
        }
    }
}