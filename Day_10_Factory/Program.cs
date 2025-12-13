using AdventOfCodeUtilities;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Reflection.PortableExecutable;
using System.Runtime.ExceptionServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;

List<string> inputList = AoC.GetInputLines();
List<Machine> machines = inputList.Select(l => new Machine(l)).ToList();

void P1()
{
    // Path finding!

    Int64 result = 0;

    foreach (var machine in machines)
    {
        MachineState startState = machine.InitialState;
        PriorityQueue<MachineState, Int64> remainingStates = new();
        remainingStates.Enqueue(startState, startState.Cost);

        while (remainingStates.Count > 0)
        {
            MachineState currentState = remainingStates.Dequeue();
            if (!currentState.Visited)
            {
                currentState.Visited = true;

                if (currentState == machine.EndState)
                    break;

                currentState.ProcessNeighbours();
                foreach (MachineState neighbourState in currentState.Neighbours.Where(node => !node.Visited))
                {
                    Int64 newCost = currentState.Cost + 1;
                    if (newCost < neighbourState.Cost)
                    {
                        neighbourState.Cost = newCost;
                        remainingStates.Enqueue(neighbourState, newCost);
                    }
                }
            }
        }

        result += machine.EndState.Cost;
    }

    Console.WriteLine(result);
    Console.ReadLine();
}

void P2()
{
#pragma warning disable 8321
    void printMatrix(int[,] A, int[] b, int[] c)
    {
        int rows = A.GetLength(0);
        int columns = A.GetLength(1);

        int maxNumDigits = Enumerable.Range(0, rows).Select(i => Enumerable.Range(0, columns).Select(j => A[i, j].ToString().Length).Max()).Max();
        maxNumDigits = Math.Max(maxNumDigits, b.Select(val => val.ToString().Length).Max());
        maxNumDigits = Math.Max(maxNumDigits, c.Select(val => val.ToString().Length).Max());

        for (int j = 0; j < columns; j++)
            Console.Write($"{c[j].ToString().PadLeft(maxNumDigits)} ");
        Console.WriteLine();
        Console.WriteLine(string.Concat(Enumerable.Repeat(new String('-', maxNumDigits + 1), columns)));
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                Console.Write($"{A[i, j].ToString().PadLeft(maxNumDigits)} ");
            }
            Console.Write($" | {b[i].ToString().PadLeft(maxNumDigits)}");
            Console.WriteLine();
        }
        Console.WriteLine();
    }
#pragma warning restore 8321

    void swapRow(int[,] A, int[] b, int i, int j)
    {
        if (i != j)
        {
            int columns = A.GetLength(1);
            for (int col = 0; col < columns; col++)
            {
                (A[i, col], A[j, col]) = (A[j, col], A[i, col]);
            }
            (b[i], b[j]) = (b[j], b[i]);
        }
    }
    void swapColumn(int[,] A, int[] c, int i, int j)
    {
        if (i != j)
        {
            int rows = A.GetLength(0);
            for (int row = 0; row < rows; row++)
            {
                (A[row, i], A[row, j]) = (A[row, j], A[row, i]);
            }
            (c[i], c[j]) = (c[j], c[i]);
        }
    }
    void reduceRow(int[,] A, int[] b, int j, int i)
    {
        // Subtract multiples from rows to end up with everything to the left of the diagonal = 0
        if (A[i, i] != 0)
        {
            int columns = A.GetLength(1);

            int x = A[i, i];
            int y = -A[j, i];
            int d = (int)AoC.GCF(x, y);
            for (int n = 0; n < columns; n++)
                A[j, n] = ((y * A[i, n]) + (x * A[j, n])) / d; // Integer division
            b[j] = ((y * b[i]) + (x * b[j])) / d; // Integer division
        }
    }

    (int[,] A, int[] b, int[] c) reduce(int[,] A, int[] b, int[] c)
    {
        int Arows = A.GetLength(0);
        int Acolumns = A.GetLength(1);
        for (int col = 0; col < Acolumns; col++)
        {
            // Swap columns around until there's definitely a non-0 in column col
            //Console.WriteLine($"Swapping columns to ensure there is a non-0 value in column {col}");
            int k = col + 1;
            List<int> rowsWithNon0 = Enumerable.Range(col, Arows - col).Where(i => A[i, col] != 0).ToList();
            while (rowsWithNon0.Count == 0 && k < Acolumns)
            {
                //Console.WriteLine($"Swap... {col} {k}");
                swapColumn(A, c, col, k);
                k++;
                rowsWithNon0 = Enumerable.Range(col, Arows - col).Where(i => A[i, col] != 0).ToList();
            }

            // If there's no column with non-0 values, we're done
            if (rowsWithNon0.Count == 0)
            {
                //Console.WriteLine("No suitable column with non-0 values");
                break;
            }

            // Swap rows so A[col][col] is non-0
            //Console.WriteLine($"Swap rows to put non-0 in A[{col}][{col}]");
            if (col != rowsWithNon0[0])
            {
                //Console.WriteLine($"Swap... {col} {rowsWithNon0[0]}");
                swapRow(A, b, col, rowsWithNon0[0]);
            }

            Debug.Assert(A[col, col] != 0);

            // Reduce other rows
            for (int row = col + 1; row < Arows; row++)
                reduceRow(A, b, row, col);
        }

        List<int> rowsWithNotAll0 = Enumerable.Range(0, Arows).Where(row => Enumerable.Range(0, Acolumns).Select(column => A[row, column]).Any(e => e != 0)).ToList();

        // Remove the empty rows
        int[,] newA = new int[rowsWithNotAll0.Count, Acolumns];
        for (int row = 0; row < rowsWithNotAll0.Count; row++)
            for (int col = 0; col < Acolumns; col++)
                newA[row, col] = A[rowsWithNotAll0[row], col];
        A = newA;
        Arows = A.GetLength(0);
        b = rowsWithNotAll0.Select(row => b[row]).ToArray();

        // Back substitution
        for (int row = Arows - 1; row >= 0; row--)
        {
            for (int col = 0; col < row; col++)
            {
                reduceRow(A, b, col, row);
            }
        }

        return (A, b, c);
    }

    List<List<int>> generateFreeVarValues(int[] c, int numFreeVars)
    {
        if (numFreeVars == 0)
            return new List<List<int>>() { new List<int>() };

        List<List<int>> combinations = new();
        List<List<int>> subCombinations = generateFreeVarValues(c, numFreeVars - 1);
        for (int val = 0; val <= c[^numFreeVars]; val++)
            combinations.AddRange(subCombinations.Select(sc => (new List<int>() { val }).Concat(sc).ToList()));
        return combinations;
    }

    Int64 result = 0;

    // Ax = b
    // A is the coefficient matrix for the linear equations
    // x is the input vector i.e. button pushes
    // b is the target joltages
    foreach (Machine machine in machines)
    {
        Int64 minSolution = Int64.MaxValue;

        int[,] A = new int[machine.TargetLights.Length, machine.Buttons.Count];
        int[] b = machine.TargetJoltages;
        int[] c = machine.Buttons.Select(button => button.Select(j => b[j]).Min()).ToArray();
        for (int buttonIndex = 0; buttonIndex < machine.Buttons.Count; buttonIndex++)
        {
            var button = machine.Buttons[buttonIndex];
            for (int joltageIndex = 0; joltageIndex < machine.TargetJoltages.Length; joltageIndex++)
            {
                if (button.Contains(joltageIndex))
                {
                    A[joltageIndex, buttonIndex] = 1;
                }
            }
        }

        (A, b, c) = reduce(A, b, c);

        //printMatrix(A, b, c);

        int rows = A.GetLength(0);
        int columns = A.GetLength(1);
        int rank = rows;
        int numFreeVars = columns - rows;

        var combinationsOfFreeVarValues = generateFreeVarValues(c, numFreeVars);
        foreach (var combinationOfFreeVarValues in combinationsOfFreeVarValues)
        {
            bool valid = true;
            int solution = combinationOfFreeVarValues.Sum();
            for (int nonFreeVarIndex = 0; nonFreeVarIndex < rank; nonFreeVarIndex++)
            {
                int sumMultipliedFreeVarVals = Enumerable.Range(0, numFreeVars).Select(j => combinationOfFreeVarValues[j] * A[nonFreeVarIndex, rank + j]).Sum();
                int remainingVal = b[nonFreeVarIndex] - sumMultipliedFreeVarVals;
                decimal nonFreeVarVal = (decimal)remainingVal / A[nonFreeVarIndex, nonFreeVarIndex]; // Integer division
                if (nonFreeVarVal < 0 || (nonFreeVarVal % 1) != 0)
                {
                    valid = false;
                    break;
                }
                solution += (int)nonFreeVarVal;
            }
            if (valid)
            {
                minSolution = Math.Min(minSolution, solution);
            }
        }

        result += minSolution;
    }

    Console.WriteLine(result);
    Console.ReadLine();
}

P1();
P2();

public class Machine
{
    public List<int[]> Buttons;
    public bool[] TargetLights;
    public int[] TargetJoltages;

    public MachineState InitialState;
    public MachineState EndState;
    public Dictionary<string, MachineState> StateMap = new();

    public Machine(string line)
    {
        var split = line.Split(' ');
        TargetLights = split[0][1..^1].Select(c => c == '#').ToArray();
        TargetJoltages = split[^1][1..^1].Split(',').Select(int.Parse).ToArray();
        Buttons = split[1..^1].Select(s => s[1..^1].Split(',').Select(int.Parse).ToArray()).ToList();

        InitialState = new MachineState(this, new bool[TargetLights.Length]);
        InitialState.Cost = 0;
        StateMap[InitialState.Key] = InitialState;

        EndState = new MachineState(this, TargetLights);
        StateMap[EndState.Key] = EndState;
    }

    public override string ToString()
    {
        return $"[{string.Concat(TargetLights.Select(l => l ? '#' : '.'))}] {string.Join(' ', Buttons.Select(b => $"({string.Join(',', b)})"))} {{{string.Join(',', TargetJoltages)}}}";
    }
}

public class MachineState
{
    public Machine Machine;
    public bool[] Lights;
    public string Key;

    public Int64 Cost = Int64.MaxValue;
    public bool Visited = false;
    public List<MachineState> Neighbours = new();

    public MachineState(Machine machine, bool[] lights)
    {
        Machine = machine;
        Lights = lights;
        Key = string.Concat(lights.Select(l => l ? '#' : '.'));
    }

    public void ProcessNeighbours()
    {
        foreach (var button in Machine.Buttons)
        {
            bool[] newLights = Lights.ToArray();
            foreach (int index in button)
                newLights[index] = !newLights[index];

            MachineState neighbour = new(Machine, newLights);
            if (Machine.StateMap.TryGetValue(neighbour.Key, out MachineState? value))
                Neighbours.Add(value);
            else
            {
                Machine.StateMap[neighbour.Key] = neighbour;
                Neighbours.Add(Machine.StateMap[neighbour.Key]);
            }
        }
    }

    public override string ToString()
    {
        return $"[{Key}] Cost: {Cost}";
    }
}
