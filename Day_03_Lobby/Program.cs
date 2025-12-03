using AdventOfCodeUtilities;
using System.Text.RegularExpressions;

List<string> inputList = AoC.GetInputLines();

void P1()
{
    int result = 0;

    foreach (string bank in inputList)
    {
        int tensMax = -1;
        int tensMaxIndex = -1;
        for (int i = 0; i < bank.Length - 1; i++)
        {
            int trial = bank[i] - '0';
            if (trial > tensMax)
            {
                tensMaxIndex = i;
                tensMax = trial;
            }
        }
        int unitsMax = -1;
        int unitsMaxIndex = -1;
        for (int i = tensMaxIndex + 1; i < bank.Length; i++)
        {
            int trial = bank[i] - '0';
            if (trial > unitsMax)
            {
                unitsMaxIndex = i;
                unitsMax = trial;
            }
        }
        result += (tensMax * 10) + unitsMax;
    }

    Console.WriteLine(result);
    Console.ReadLine();
}

void P2()
{
    Int64 result = 0;

    foreach (string bank in inputList)
    {
        int[] digits = new int[12];
        int[] digitIndexes = new int[12];

        for (int digitIndex = digits.Length - 1; digitIndex >= 0; digitIndex--)
        {
            int digitMax = -1;
            int digitMaxIndex = -1;
            for (int i = digitIndex < (digits.Length - 1) ? digitIndexes[digitIndex + 1] + 1 : 0; i < bank.Length - digitIndex; i++)
            {
                int trial = bank[i] - '0';
                if (trial > digitMax)
                {
                    digitMaxIndex = i;
                    digitMax = trial;
                }
            }
            digits[digitIndex] = digitMax;
            digitIndexes[digitIndex] = digitMaxIndex;
        }

        Int64 increment = 0;
        for (int i = 0; i < digits.Length; i++)
        {
            increment += digits[i] * (Int64)(Math.Pow(10, i));
        }
        result += increment;
    }

    Console.WriteLine(result);
    Console.ReadLine();
}

P1();
P2();
