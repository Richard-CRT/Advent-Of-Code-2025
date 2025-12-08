using AdventOfCodeUtilities;
using System.Diagnostics;
using System.Text.RegularExpressions;

List<string> inputList = AoC.GetInputLines();

List<(int, int, int)> junctionBoxes = inputList.Select(s => { var foo = s.Split(',').Select(s => int.Parse(s)).ToList(); return (foo[0], foo[1], foo[2]); }).ToList();

Int64 distanceSqr((int, int, int) node1, (int, int, int) node2)
{
    Int64 dx = Math.Abs(node1.Item1 - node2.Item1);
    Int64 dy = Math.Abs(node1.Item2 - node2.Item2);
    Int64 dz = Math.Abs(node1.Item3 - node2.Item3);
    return dx * dx + dy * dy + dz * dz;
}

void P1()
{
    List<HashSet<(int, int, int)>> circuits = new(junctionBoxes.Select(jb => new HashSet<(int, int, int)>() { jb }).ToList());
    HashSet<(int, int)> junctionBoxIndexCombinationsAlreadyDone = new();

    List<(long, int, int)> distanceAndJBPairs = new();
    for (int i = 0; i < junctionBoxes.Count; i++)
    {
        for (int j = i + 1; j < junctionBoxes.Count; j++)
        {
            distanceAndJBPairs.Add((distanceSqr(junctionBoxes[i], junctionBoxes[j]), i, j));
        }
    }
    distanceAndJBPairs = distanceAndJBPairs.OrderBy(t => t.Item1).ToList();

    const int cycles = 1000;
    for (int num = 0; num < cycles; num++)
    {
        /*
        // Slower brute force
        // This is not the most efficient brute force
        // I tried optimisations by comparing every pair in each cicuit, then comparing every pair in distinct circuits
        // It worked, a bit slower for pt1 and a bit faster for pt2, but much more code, so sticking with this one
        Int64 minDistanceSqr = Int64.MaxValue;
        int minDistJB1Index = -1;
        int minDistJB2Index = -1;
        for (int i = 0; i < junctionBoxes.Count; i++)
        {
            (int, int, int) jb1 = junctionBoxes[i];
            for (int j = i + 1; j < junctionBoxes.Count; j++)
            {
                (int, int, int) jb2 = junctionBoxes[j];

                Int64 dist = distanceSqr(jb1, jb2);
                // Faster to do this dist check than the HashSet check
                if (dist < minDistanceSqr)
                {
                    if (!junctionBoxIndexCombinationsAlreadyDone.Contains((i, j)))
                    {
                        minDistanceSqr = dist;
                        minDistJB1Index = i;
                        minDistJB2Index = j;
                    }
                }
            }
        }
        */

        var pairWithShortestDistance = distanceAndJBPairs[0];
        distanceAndJBPairs.RemoveAt(0);
        int minDistJB1Index = pairWithShortestDistance.Item2;
        int minDistJB2Index = pairWithShortestDistance.Item3;

        if (!junctionBoxIndexCombinationsAlreadyDone.Contains((minDistJB1Index, minDistJB2Index)))
        {
            junctionBoxIndexCombinationsAlreadyDone.Add((minDistJB1Index, minDistJB2Index));

            (int, int, int) minDistJB1 = junctionBoxes[minDistJB1Index];
            (int, int, int) minDistJB2 = junctionBoxes[minDistJB2Index];

            int jBCircuit1Index = -1;
            int jBCircuit2Index = -1;
            for (int i = 0; i < circuits.Count; i++)
            {
                if (circuits[i].Contains(minDistJB1))
                {
                    jBCircuit1Index = i;
                }
                if (circuits[i].Contains(minDistJB2))
                {
                    jBCircuit2Index = i;
                }
            }
            Debug.Assert(jBCircuit1Index != -1 && jBCircuit2Index != -1);
            if (jBCircuit1Index != jBCircuit2Index)
            {
                HashSet<(int, int, int)>? jBCircuit1 = circuits[jBCircuit1Index];
                HashSet<(int, int, int)>? jBCircuit2 = circuits[jBCircuit2Index];

                jBCircuit1.UnionWith(jBCircuit2);
                circuits.RemoveAt(jBCircuit2Index);
            }
        }
    }

    Console.WriteLine(circuits.Select(c => c.Count).OrderByDescending(c => c).Take(3).Aggregate(1, (acc, val) => (acc * val)));
    Console.ReadLine();
}

void P2()
{
    List<HashSet<(int, int, int)>> circuits = new(junctionBoxes.Select(jb => new HashSet<(int, int, int)>() { jb }).ToList());
    HashSet<(int, int)> junctionBoxIndexCombinationsAlreadyDone = new();

    List<(long, int, int)> distanceAndJBPairs = new();
    for (int i = 0; i < junctionBoxes.Count; i++)
    {
        for (int j = i + 1; j < junctionBoxes.Count; j++)
        {
            distanceAndJBPairs.Add((distanceSqr(junctionBoxes[i], junctionBoxes[j]), i, j));
        }
    }
    distanceAndJBPairs = distanceAndJBPairs.OrderBy(t => t.Item1).ToList();

    int minDistJB1Index = -1;
    int minDistJB2Index = -1;
    while (circuits.Count > 1)
    {
        var pairWithShortestDistance = distanceAndJBPairs[0];
        distanceAndJBPairs.RemoveAt(0);

        minDistJB1Index = pairWithShortestDistance.Item2;
        minDistJB2Index = pairWithShortestDistance.Item3;

        if (!junctionBoxIndexCombinationsAlreadyDone.Contains((minDistJB1Index, minDistJB2Index)))
        {
            junctionBoxIndexCombinationsAlreadyDone.Add((minDistJB1Index, minDistJB2Index));
            (int, int, int) minDistJB1 = junctionBoxes[minDistJB1Index];
            (int, int, int) minDistJB2 = junctionBoxes[minDistJB2Index];

            int jBCircuit1Index = -1;
            int jBCircuit2Index = -1;
            for (int i = 0; i < circuits.Count; i++)
            {
                if (circuits[i].Contains(minDistJB1))
                {
                    jBCircuit1Index = i;
                }
                if (circuits[i].Contains(minDistJB2))
                {
                    jBCircuit2Index = i;
                }
            }
            Debug.Assert(jBCircuit1Index != -1 && jBCircuit2Index != -1);
            if (jBCircuit1Index != jBCircuit2Index)
            {
                HashSet<(int, int, int)>? jBCircuit1 = circuits[jBCircuit1Index];
                HashSet<(int, int, int)>? jBCircuit2 = circuits[jBCircuit2Index];

                jBCircuit1.UnionWith(jBCircuit2);
                circuits.RemoveAt(jBCircuit2Index);
            }
        }
    }

    Console.WriteLine((Int64)junctionBoxes[minDistJB1Index].Item1 * (Int64)junctionBoxes[minDistJB2Index].Item1);
    Console.ReadLine();
}

P1();
P2();
