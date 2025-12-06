using AdventOfCodeUtilities;
using System.Diagnostics;
using System.Text.RegularExpressions;

List<string> inputList = AoC.GetInputLines();

List<(Int64, Int64)> ranges = new();
int i;
for (i = 0; inputList[i] != ""; i++)
{
    var vals = inputList[i].Split('-').Select(s => Int64.Parse(s)).ToList();
    Debug.Assert(vals[1] >= vals[0]);
    ranges.Add((vals[0], vals[1]));
}
i++;
List<Int64> trialVals = new();
for (; i < inputList.Count; i++)
{
    trialVals.Add(Int64.Parse(inputList[i]));
}

void P1()
{
    int result = 0;

    foreach (Int64 trialVal in trialVals)
    {
        foreach ((Int64 lower, Int64 higher) in ranges)
        {
            if (trialVal >= lower && trialVal <= higher)
            {
                result++;
                break;
            }
        }
    }

    Console.WriteLine(result);
    Console.ReadLine();
}

void P2()
{
    Int64 result = 0;

    ranges = ranges.OrderBy(r => r.Item1).ToList();

    for (int i = 0; i < ranges.Count; i++)
    {
        (Int64 range1Lower, Int64 range1Higher) = ranges[i];
        for (int j = i + 1; j < ranges.Count; j++)
        {
            (Int64 range2Lower, Int64 range2Higher) = ranges[j];

            // j range is completely encompassed by i range, get rid of it
            if (range2Lower <= range1Higher && range2Higher <= range1Higher)
            {
                ranges.RemoveAt(j);
                j--;
            }
            else if (range2Lower <= range1Higher)
                ranges[j] = (range1Higher + 1, range2Higher);
        }
    }

    foreach ((Int64 lower, Int64 higher) in ranges)
    {
        result += (higher - lower) + 1;
    }

    Console.WriteLine(result);
    Console.ReadLine();
}

P1();
P2();
