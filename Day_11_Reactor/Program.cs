using AdventOfCodeUtilities;
using System.Collections.Generic;
using System.Text.RegularExpressions;

List<string> inputList = AoC.GetInputLines();
List<Node> nodes = inputList.Select(l => new Node(l)).ToList();
Node outNode = new("out:");
nodes.Add(outNode);
nodes.ForEach(n => n.LinkToObjects());

void P1()
{
    Int64 recurse(Node n)
    {
        if (n.OutputNodes.Count == 0)
            return 1;
        else
            return n.OutputNodes.Sum(n => recurse(n));
    }

    Console.WriteLine(recurse(Node.NodeMap["you"]));
    Console.ReadLine();
}

void P2()
{
    Dictionary<(bool, bool, string), Int64> cache = new();
    Int64 recurse(Node n, HashSet<string> path)
    {

        (bool, bool, string) cacheKey = (path.Contains("dac"), path.Contains("fft"), n.Name);
        if (cache.TryGetValue(cacheKey, out Int64 result))
            return result;

        if (n.OutputNodes.Count == 0)
        {
            if (cacheKey.Item1 && cacheKey.Item2)
                return 1;
            else
                return 0;
        }
        else
        {
            Int64 val;
            val = n.OutputNodes.Sum(n => recurse(n, new(path.Append(n.Name))));
            cache[cacheKey] = val;
            return val;
        }
    }

    Console.WriteLine(recurse(Node.NodeMap["svr"], new()));
    Console.ReadLine();
}

P1();
P2();

public class Node
{
    public static Dictionary<string, Node> NodeMap = new();

    public List<Node> OutputNodes = new();
    private string[] OutputNodeNames;
    public string Name;

    public Node(string line)
    {
        var split = line.Split(' ');
        Name = split[0][..^1];
        OutputNodeNames = split[1..];
        NodeMap[Name] = this;
    }

    public void LinkToObjects()
    {
        OutputNodes = OutputNodeNames.Select(n => NodeMap[n]).ToList();
    }
}