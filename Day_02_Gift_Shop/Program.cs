using AdventOfCodeUtilities;
using System.Text.RegularExpressions;
using System.Xml.Schema;

List<string> inputList = AoC.GetInputLines();
List<(Int64, Int64)> ranges = inputList[0].Split(',').Select(sr => { var se = sr.Split('-').Select(s => Int64.Parse(s)).ToArray(); return (se[0], se[1]); }).ToList();

void P1()
{
    Int64 sum = 0;

    foreach ((Int64 start, Int64 end) in ranges)
    {
        for (Int64 i = start; i <= end; i++)
        {
            string trialVal = i.ToString();
            bool matches = false;

            if (trialVal.Length % 2 == 0)
            {
                // 2 divides length of val evenly
                int len = trialVal.Length / 2;

                matches = true;
                string pattern = trialVal[0..len];
                for (int j = len; j < trialVal.Length; j += len)
                {
                    if (trialVal[j..(j + len)] != pattern)
                    {
                        matches = false;
                        break;
                    }
                }
            }

            if (matches)
                sum += i;
        }
    }

    Console.WriteLine(sum);
    Console.ReadLine();
}

void P2()
{
    Int64 sum = 0;

    foreach ((Int64 start, Int64 end) in ranges)
    {
        for (Int64 i = start; i <= end; i++)
        {
            string trialVal = i.ToString();

            bool matches = false;

            for (int len = trialVal.Length / 2; len > 0; len--)
            {
                if (trialVal.Length % len == 0)
                {
                    // len divides length of val evenly

                    matches = true;
                    string pattern = trialVal[0..len];
                    for (int j = len; j < trialVal.Length; j += len)
                    {
                        if (trialVal[j..(j + len)] != pattern)
                        {
                            matches = false;
                            break;
                        }
                    }
                    if (matches)
                        break;
                }
            }

            if (matches)
            {
                sum += i;
            }
        }
    }

    Console.WriteLine(sum);
    Console.ReadLine();
}

P1();
P2();
