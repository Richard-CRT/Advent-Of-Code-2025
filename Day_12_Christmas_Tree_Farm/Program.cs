using AdventOfCodeUtilities;
using System.Text.RegularExpressions;

List<string> inputList = AoC.GetInputLines();
int i;
List<Shape> shapes = new List<Shape>();
for (i = 0; !inputList[i].Contains('x'); i++)
{
    Shape workingShape = new();
    i++;
    for (; inputList[i] != ""; i++)
        workingShape.NumOccupiedCoordinates += inputList[i].Count(c => c == '#');
    shapes.Add(workingShape);
}
List<Region> regions = inputList[i..].Select(l => new Region(shapes, l)).ToList();

void P1()
{
    Console.WriteLine(regions.Count(r => r.NaiveFits()));
    Console.ReadLine();
}

P1();

public class Shape
{
    public Int64 NumOccupiedCoordinates;
}

public class Region
{
    Int64 Width;
    Int64 Height;
    Dictionary<Shape, int> Shapes = new();

    public Region(List<Shape> allShapes, string line)
    {
        var split = line.Split(' ');
        var split1 = split[0][..^1].Split('x');
        Width = Int64.Parse(split1[0]);
        Height = Int64.Parse(split1[1]);

        for (int i = 0; i < allShapes.Count; i++)
        {
            Shape shape = allShapes[i];
            Shapes[shape] = int.Parse(split[1 + i]);
        }
    }

    public bool NaiveFits()
    {
        Int64 shapesArea = Shapes.Sum(kvp => kvp.Key.NumOccupiedCoordinates * kvp.Value);
        return shapesArea <= Width * Height;
    }
}
