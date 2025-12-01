using AdventOfCodeUtilities;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;
using System.Security;
using System.Text.RegularExpressions;

List<string> inputList = AoC.GetInputLines();


void P1()
{
    int val = 50;
    int result = 0;
    foreach (var line in inputList)
    {
        char direction = line[0];
        int inc = int.Parse(line[1..]);
        inc = inc % 100;
        if (direction == 'L')
            val = (val - inc);
        else
            val = (val + inc);
        if (val < 0)
            val += 100;
        else if (val >= 100)
            val -= 100;

        if (val == 0)
            result++;
    }

    Console.WriteLine(result);
    Console.ReadLine();
}

void P2()
{
    int val = 50;
    int result = 0;
    foreach (var line in inputList)
    {
        char direction = line[0];
        int inc = int.Parse(line[1..]);

        result += inc / 100;
        inc = inc % 100;
        
        if (inc > 0)
        {
            if (direction == 'R')
            {
                val += inc;
                if (val >= 100)
                {
                    val -= 100;
                    result++;
                }
            }
            else
            {
                int newVal = val - inc;
                if (newVal == 0)
                    result++;
                else if (newVal < 0)
                {
                    if (val != 0)
                        result++;
                    newVal += 100;
                    
                }
                val = newVal;
            }
        }
    }

    Console.WriteLine(result);
    Console.ReadLine();
}

P1();
P2();
