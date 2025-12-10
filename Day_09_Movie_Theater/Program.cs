using AdventOfCodeUtilities;
using System;
using System.Diagnostics;
using System.Linq;
using System.Net.Http.Headers;
using System.Runtime.Intrinsics.X86;
using System.Text.RegularExpressions;

List<string> inputList = AoC.GetInputLines();

List<(int X, int Y)> redTiles = inputList.Select(s => { var ints = s.Split(',').Select(int.Parse).ToList(); return (ints[0], ints[1]); }).ToList();
HashSet<(int X, int Y)> redTilesSet = new(redTiles);

void P1()
{
    Int64 maxArea = Int64.MinValue;
    for (int i = 0; i < redTiles.Count; i++)
    {
        var redTile1 = redTiles[i];
        for (int j = i + 1; j < redTiles.Count; j++)
        {
            var redTile2 = redTiles[j];

            int dx = Math.Abs(redTile1.X - redTiles[j].X) + 1;
            int dy = Math.Abs(redTile1.Y - redTiles[j].Y) + 1;

            Int64 area = (Int64)dx * (Int64)dy;

            maxArea = Math.Max(maxArea, area);
        }
    }

    Console.WriteLine(maxArea);
    Console.ReadLine();
}

void P2()
{   
    bool rectangleIntersectsPolygon(List<((int X, int Y) Start, (int X, int Y) End)> polygonLines, (int X, int Y) tl, (int X, int Y) br)
    {
        // Interior only, boundaries are allowed to touch
        foreach (var polygonLine in polygonLines)
        {
            int dx1 = polygonLine.End.X - polygonLine.Start.X;
            int dy1 = polygonLine.End.Y - polygonLine.Start.Y;
            if (dx1 == 0 && dy1 != 0)
            {
                // line vertical
                int x = polygonLine.Start.X;
                int minY = Math.Min(polygonLine.Start.Y, polygonLine.End.Y);
                int maxY = Math.Max(polygonLine.Start.Y, polygonLine.End.Y);

                if (x > tl.X && x < br.X)
                {
                    // If edge strictly inside the rect boundary it definitely intersects
                    if (maxY > tl.Y && minY < br.Y)
                        return true;
                }

            }
            else if (dx1 != 0 && dy1 == 0)
            {
                // line horizontal

                int minX = Math.Min(polygonLine.Start.X, polygonLine.End.X);
                int maxX = Math.Max(polygonLine.Start.X, polygonLine.End.X);
                int y = polygonLine.Start.Y;

                if (y > tl.Y && y < br.Y)
                {
                    // If edge strictly inside the rect boundary it definitely intersects
                    if (maxX > tl.X && minX < br.X)
                        return true;
                }
            }
        }
        return false;
    }

    //int lineCrossesPolygon(List<((int X, int Y) Start, (int X, int Y) End)> polygonLines, ((int X, int Y) Start, (int X, int Y) End) line)
    //{
    //    return polygonLines.Where(l => lineCrossesLine(l, line)).Count();
    //}

    bool pointInPolygon(List<((int X, int Y) Start, (int X, int Y) End)> polygonLines, (int X, int Y) point)
    {
        // Both of these work, note that avoiding double counting vertices results in tr,bl,br points sometimes returning false, only tl to be trusted

        int crosses = 0;
        foreach (var polygonLine in polygonLines)
        {
            int dx1 = polygonLine.End.X - polygonLine.Start.X;
            int dy1 = polygonLine.End.Y - polygonLine.Start.Y;
            if (dx1 == 0 && dy1 != 0)
            {
                // line vertical
                int x = polygonLine.Start.X;
                if (x > point.X)
                {
                    int minY = Math.Min(polygonLine.Start.Y, polygonLine.End.Y);
                    int maxY = Math.Max(polygonLine.Start.Y, polygonLine.End.Y);

                    // <= p < to avoid double counting vertices
                    if (minY <= point.Y && point.Y < maxY)
                        crosses++;
                }
            }
        }

        //int maxX = polygonLines.Select(l => Math.Max(l.Start.X, l.End.X)).Max();
        //((int X, int Y) Start, (int X, int Y) End) raycastLine = ((point.X + 1, point.Y), (maxX + 1, point.Y));
        //int crosses = lineCrossesPolygon(polygonLines, raycastLine);

        return crosses % 2 == 1;
    }

    List<((int StartX, int StartY), (int EndX, int EndY))> polygonLines = new();
    HashSet<(int, int)> permittedTiles = new(redTilesSet);
    for (int i = 0; i < redTiles.Count; i++)
    {
        var redTile1 = redTiles[i];
        (int X, int Y) redTile2;
        if (i != redTiles.Count - 1)
            redTile2 = redTiles[i + 1];
        else
            redTile2 = redTiles[0];
        if (redTile1.X == redTile2.X)
        {
            Debug.Assert(redTile1.Y != redTile2.Y);
            int min = Math.Min(redTile1.Y, redTile2.Y);
            int max = Math.Max(redTile1.Y, redTile2.Y);
            polygonLines.Add(((redTile1.X, min), (redTile1.X, max)));
            for (int y = min; y <= max; y++)
                permittedTiles.Add((redTile1.X, y));
        }
        else if (redTile1.Y == redTile2.Y)
        {
            Debug.Assert(redTile1.X != redTile2.X);
            int min = Math.Min(redTile1.X, redTile2.X);
            int max = Math.Max(redTile1.X, redTile2.X);
            polygonLines.Add(((min, redTile1.Y), (max, redTile1.Y)));
            for (int x = min; x <= max; x++)
                permittedTiles.Add((x, redTile1.Y));
        }
        else
            throw new Exception();
    }

    Int64 maxArea = Int64.MinValue;
    for (int i = 0; i < redTiles.Count; i++)
    {
        //Console.WriteLine($"{i}/{redTiles.Count}");
        var redTile1 = redTiles[i];
        for (int j = i + 1; j < redTiles.Count; j++)
        {
            var redTile2 = redTiles[j];

            int minX = Math.Min(redTile1.X, redTiles[j].X);
            int maxX = Math.Max(redTile1.X, redTiles[j].X);
            int minY = Math.Min(redTile1.Y, redTiles[j].Y);
            int maxY = Math.Max(redTile1.Y, redTiles[j].Y);

            int dx = (maxX - minX) + 1;
            int dy = (maxY - minY) + 1;
            Int64 area = (Int64)dx * (Int64)dy;
            // Optimisation
            if (area < maxArea)
                continue;

            (int X, int Y) tl = (minX, minY);
            //(int X, int Y) tr = (maxX, minY);
            //(int X, int Y) bl = (minX, maxY);
            (int X, int Y) br = (maxX, maxY);
            
            //print(permittedTiles, tl, br);

            // Check c1 is in the polygon
            // Because we use ray casting, c2-4 might return false even though they're on the boundary
            bool c1 = pointInPolygon(polygonLines, tl);
            //bool c2 = pointInPolygon(polygonLines, tr);
            //bool c3 = pointInPolygon(polygonLines, bl);
            //bool c4 = pointInPolygon(polygonLines, br);

            if (c1)
            {
                // Both of these work

                if (!rectangleIntersectsPolygon(polygonLines, tl, br))
                    maxArea = Math.Max(maxArea, area);

                //// Interior only, boundaries are allowed to touch
                //int l1 = lineCrossesPolygon(polygonLines, ((minX + 1, minY + 1), (maxX - 1, minY + 1)));
                //int l2 = lineCrossesPolygon(polygonLines, ((maxX - 1, minY + 1), (maxX - 1, maxY - 1)));
                //int l3 = lineCrossesPolygon(polygonLines, ((minX + 1, maxY - 1), (maxX - 1, maxY - 1)));
                //int l4 = lineCrossesPolygon(polygonLines, ((minX + 1, minY + 1), (minX + 1, maxY - 1)));

                //// Relies on lack of adjacent edges
                //if (l1 == 0 && l2 == 0 && l3 == 0 && l4 == 0)
                //{
                //    int dx = (maxX - minX) + 1;
                //    int dy = (maxY - minY) + 1;
                //    Int64 area = (Int64)dx * (Int64)dy;

                //    maxArea = Math.Max(maxArea, area);
                //}
            }
        }
    }

    Console.WriteLine(maxArea);
    Console.ReadLine();
}

#pragma warning disable 8321
void print(HashSet<(int X, int Y)> permittedTiles, (int X, int Y)? tl = null, (int X, int Y)? br = null)
{
    int width = permittedTiles.Select(t => t.X).Max() + 1;
    int height = permittedTiles.Select(t => t.Y).Max() + 1;

    var permittedTilesSet = permittedTiles.ToHashSet();

    for (int y = 0; y < height; y++)
    {
        for (int x = 0; x < width; x++)
        {
            if (tl is not null && br is not null && x >= tl.Value.X && x <= br.Value.X && y >= tl.Value.Y && y <= br.Value.Y)
            {
                Console.Write('O');
            }
            else
            {
                if (permittedTilesSet.Contains((x, y)))
                    Console.Write('#');
                else
                    Console.Write('.');
            }
        }
        Console.WriteLine();
    }
}
#pragma warning restore 8321

P1();
P2();
