using AdventOfCodeUtilities;
using System.Collections.Generic;
using System.Text.RegularExpressions;

List<string> inputList = AoC.GetInputLines();

void P1()
{
    List<List<int>> operandLists = new();
    for (int i = 0; i < inputList.Count - 1; i++)
    {
        operandLists.Add(inputList[i].Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(s => int.Parse(s)).ToList());
    }
    List<char> operations = inputList.Last().Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(s => s[0]).ToList();

    Int64 result = 0;

    for (int op = 0; op < operations.Count; op++)
    {
        Int64 intermediateResult;
        if (operations[op] == '*')
            intermediateResult = 1;
        else
            intermediateResult = 0;
        for (int i = 0; i < operandLists.Count; i++)
        {
            if (operations[op] == '*')
                intermediateResult *= operandLists[i][op];
            else
                intermediateResult += operandLists[i][op];
        }

        result += intermediateResult;
    }

    Console.WriteLine(result);
    Console.ReadLine();
}

void P2()
{
    Int64 result = 0;

    List<Int64> nums = new();
    for (int column = inputList[0].Length - 1; column >= 0; column--)
    {
        List<int> digits = new();
        for (int y = 0; y < inputList.Count - 1; y++)
        {
            if (inputList[y][column] != ' ')
            {
                digits.Add((inputList[y][column] - '0'));
            }
        }
        int verticalNum = 0;
        for (int i = 0; i < digits.Count; i++)
        {
            verticalNum += digits[i] * (int)Math.Pow(10, (digits.Count - i) - 1);
        }

        nums.Add(verticalNum);

        if (inputList[inputList.Count - 1][column] != ' ')
        {
            char operation = inputList[inputList.Count - 1][column];
            if (operation == '*')
            {
                result += nums.Aggregate(1L, (acc, val) => (long)(acc * val));
            }
            else
            {
                result += nums.Sum();
            }

            column--;
            nums = new();
        }
    }

    Console.WriteLine(result);
    Console.ReadLine();
}

P1();
P2();
