using AdventOfCodeUtilities;
using System.Text.RegularExpressions;
using System.Transactions;

List<string> inputList = AoC.GetInputLines();

Dictionary<(int, int), bool> map = new();

int height = inputList.Count;
int width = inputList[0].Length;

for (int y = 0; y < height; y++)
{
    for (int x = 0; x < width; x++)
    {
        if (inputList[y][x] == '.')
            map[(x, y)] = false;
        else
            map[(x, y)] = true;
    }
}

void P1()
{
    int result = 0;

    for (int y = 0; y < height; y++)
    {
        for (int x = 0; x < width; x++)
        {
            if (map[(x, y)])
            {
                int adjacentRolls = 0;
                for (int _y = -1; _y <= 1; _y++)
                {
                    for (int _x = -1; _x <= 1; _x++)
                    {
                        if (_y != 0 || _x != 0)
                        {
                            var key = (x + _x, y + _y);
                            if (map.TryGetValue(key, out bool value) && value)
                                adjacentRolls++;
                        }
                    }
                }
                if (adjacentRolls < 4)
                    result++;
            }
        }
    }

    Console.WriteLine(result);
    Console.ReadLine();
}

void P2()
{
    int result = 0;

    bool changed = true;
    while (changed)
    {
        changed = false;
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                if (map[(x, y)])
                {
                    int adjacentRolls = 0;
                    for (int _y = -1; _y <= 1; _y++)
                    {
                        for (int _x = -1; _x <= 1; _x++)
                        {
                            if (_y != 0 || _x != 0)
                            {
                                var key = (x + _x, y + _y);
                                if (map.TryGetValue(key, out bool value) && value)
                                    adjacentRolls++;
                            }
                        }
                    }
                    if (adjacentRolls < 4)
                    {
                        changed = true;
                        map[(x, y)] = false;
                        result++;
                    }
                }
            }
        }
    }

    Console.WriteLine(result);
    Console.ReadLine();
}

P1();
P2();
