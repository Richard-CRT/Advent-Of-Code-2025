using AdventOfCodeUtilities;
using System.Diagnostics;
using System.Text.RegularExpressions;

List<string> inputList = AoC.GetInputLines();

Dictionary<(Int64, Int64), bool> map = new();

Int64 height = inputList.Count;
Int64 width = inputList[0].Length;

for (int y = 0; y < height; y++)
{
    for (int x = 0; x < width; x++)
    {
        char c = inputList[y][x];
        map[(x, y)] = c == '^';
    }
}

void P1()
{
    Int64 result = 0;

    List<(Int64, Int64)> beamHeads = new();

    HashSet<(Int64, Int64)> beamHeadsSeenCoords = new();
    beamHeads.Add((inputList[0].IndexOf('S'), 0));

    while (beamHeads.Count > 0)
    {
        for (int i = 0; i < beamHeads.Count; i++)
        {
            var beamHead = beamHeads[i];

            (Int64 x, Int64 y) = beamHead;

            (Int64, Int64) trialLocation = (x, y + 1);
            if (map.ContainsKey(trialLocation))
            {
                // if splitter
                if (map[trialLocation])
                {
                    result++;
                    (Int64, Int64) newLocation1 = (x - 1, y + 1);
                    (Int64, Int64) newLocation2 = (x + 1, y + 1);

                    beamHeads.RemoveAt(i);
                    i--;

                    if (!beamHeadsSeenCoords.Contains(newLocation1))
                    {
                        // Insert the new beamhead where the old one was
                        i++;
                        beamHeads.Insert(i, newLocation1);
                        beamHeadsSeenCoords.Add(newLocation1);
                    }
                    if (!beamHeadsSeenCoords.Contains(newLocation2))
                    {
                        // Insert the new beamhead where the old one was
                        i++;
                        beamHeads.Insert(i, newLocation2);
                        beamHeadsSeenCoords.Add(newLocation2);
                    }
                }
                else
                {
                    if (beamHeadsSeenCoords.Contains(trialLocation))
                    {
                        // If we've already visited this location in a beam, get rid of it
                        beamHeads.RemoveAt(i);
                        i--;
                    }
                    else
                    {
                        beamHeads[i] = trialLocation;
                        beamHeadsSeenCoords.Add(trialLocation);
                    }
                }
            }
            else
            {
                beamHeads.RemoveAt(i);
                i--;
            }
        }
    }

    Console.WriteLine(result);
    Console.ReadLine();
}

void P2()
{
    List<BeamHead> beamHeads = new();

    HashSet<(int, int)> beamHeadsSeenCoords = new();

    beamHeads.Add(new BeamHead(inputList[0].IndexOf('S'), 0, 1, new()));
    Int64 beamsLeftToFinish = 1;

    while (beamsLeftToFinish > 0)
    {
        for (int i = 0; i < beamHeads.Count; i++)
        {
            var beamHead = beamHeads[i];
            if (!beamHead.Done)
            {
                (int, int) trialLocation = (beamHead.X, beamHead.Y + 1);
                if (map.ContainsKey(trialLocation))
                {
                    // if splitter
                    if (map[trialLocation])
                    {
                        (int, int) newLocation1 = (beamHead.X - 1, beamHead.Y + 1);
                        (int, int) newLocation2 = (beamHead.X + 1, beamHead.Y + 1);

                        beamsLeftToFinish--;
                        beamHeads.RemoveAt(i);
                        i--;

                        if (!beamHeadsSeenCoords.Contains(newLocation1))
                        {
                            // Insert the new beamhead where the old one was
                            i++;

                            beamHeads.Insert(i, new BeamHead(newLocation1.Item1, newLocation1.Item2, beamHead.Multiplier, new(beamHead.AllLocations.Append(newLocation1))));
                            beamsLeftToFinish++;
                            beamHeadsSeenCoords.Add(newLocation1);
                        }
                        else
                            beamHeads.First(b => b.AllLocations.Contains(newLocation1)).Multiplier += beamHead.Multiplier;

                        if (!beamHeadsSeenCoords.Contains(newLocation2))
                        {
                            // Insert the new beamhead where the old one was
                            i++;

                            beamHeads.Insert(i, new BeamHead(newLocation2.Item1, newLocation2.Item2, beamHead.Multiplier, new(beamHead.AllLocations.Append(newLocation2))));
                            beamsLeftToFinish++;
                            beamHeadsSeenCoords.Add(newLocation2);
                        }
                        else
                            beamHeads.First(b => b.AllLocations.Contains(newLocation2)).Multiplier += beamHead.Multiplier;
                    }
                    else
                    {
                        if (beamHeadsSeenCoords.Contains(trialLocation))
                        {
                            beamsLeftToFinish--;
                            beamHeads.First(b => b.AllLocations.Contains(trialLocation)).Multiplier += beamHead.Multiplier;
                            // If we've already visited this location in a beam, get rid of it
                            beamHeads.RemoveAt(i);
                            i--;
                        }
                        else
                        {
                            beamHead.Y++;
                            beamHead.AllLocations.Add(trialLocation);
                            beamHeadsSeenCoords.Add(trialLocation);
                        }
                    }
                }
                else
                {
                    beamHead.Done = true;
                    beamsLeftToFinish--;
                }
            }
        }
    }

    Int64 result = beamHeads.Sum(b => b.Multiplier);
    Console.WriteLine(result);
    Console.ReadLine();
}

P1();
P2();

public class BeamHead
{
    public int X;
    public int Y;
    public HashSet<(int, int)> AllLocations = new();
    public Int64 Multiplier = 1;
    public bool Done = false;

    public BeamHead(int x, int y, Int64 multiplier, HashSet<(int, int)> previousLocations)
    {
        Multiplier = multiplier;
        X = x;
        Y = y;
        AllLocations = previousLocations;
    }
}
