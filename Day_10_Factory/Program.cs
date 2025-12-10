using AdventOfCodeUtilities;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Reflection.PortableExecutable;
using System.Runtime.ExceptionServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.Z3;

List<string> inputList = AoC.GetInputLines();
List<Machine> machines = inputList.Select(l => new Machine(l)).ToList();

// Path finding!

void P1()
{
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
    Int64 result = 0;

    ////This solution works but takes too long on the hardest input lines
    //bool check(List<(List<int> ButtonIndexes, int TargetSum)> simultaneousEquations, int[] buttonPushesByButtonIndex)
    //{
    //    // Check simulateneous equations of input
    //    bool allSimultaneousEquationsPassed = true;
    //    foreach (var equation in simultaneousEquations)
    //    {
    //        bool allButtonPushValuesKnown = true;
    //        int sum = 0;
    //        foreach (var buttonInEquationIndex in equation.ButtonIndexes)
    //        {
    //            if (buttonPushesByButtonIndex[buttonInEquationIndex] != -1)
    //                sum += buttonPushesByButtonIndex[buttonInEquationIndex];
    //            else
    //            {
    //                allButtonPushValuesKnown = false;
    //                break;
    //            }
    //        }
    //        if (allButtonPushValuesKnown)
    //        {
    //            if (sum == equation.TargetSum)
    //            {
    //                // This equation is fulfilled
    //            }
    //            else
    //            {
    //                // Fails equation - no way to recover
    //                allSimultaneousEquationsPassed = false;
    //                return false;
    //            }
    //        }
    //        else
    //        {
    //            // Can't check this equation as we haven't picked a value for this button
    //            //allSimultaneousEquationsPassed = false;
    //        }
    //    }

    //    return allSimultaneousEquationsPassed;
    //}

    //bool extrapolateFromSimultaneousEquations(List<(List<int> ButtonIndexes, int TargetSum)> simultaneousEquations, int[] newButtonPushesByButtonIndex)
    //{
    //    bool valueCalculated = true;
    //    bool calculatedValueInvalid = false;
    //    while (valueCalculated)
    //    {
    //        valueCalculated = false;
    //        foreach (var equation in simultaneousEquations)
    //        {
    //            int sum = equation.TargetSum;
    //            List<int> unknownButtonIndexes = new();
    //            foreach (var buttonInEquationIndex in equation.ButtonIndexes)
    //            {
    //                int currentPushesValue = newButtonPushesByButtonIndex[buttonInEquationIndex];
    //                if (currentPushesValue == -1)
    //                    unknownButtonIndexes.Add(buttonInEquationIndex);
    //                else
    //                {
    //                    sum -= currentPushesValue;
    //                }
    //            }
    //            if (unknownButtonIndexes.Count == 1)
    //            {
    //                // We can calculate the last value
    //                valueCalculated = true;
    //                if (sum < 0)
    //                    calculatedValueInvalid = true;
    //                else
    //                    newButtonPushesByButtonIndex[unknownButtonIndexes[0]] = sum;
    //                break;
    //            }
    //        }
    //        if (calculatedValueInvalid)
    //            break;
    //    }
    //    return !calculatedValueInvalid;
    //}

    //int recurse(Machine machine, List<(List<int> ButtonIndexes, int TargetSum)> simultaneousEquations, int[] buttonPushesByButtonIndex, int[] joltages, int buttonIndex)
    //{
    //    Debug.Assert(buttonPushesByButtonIndex.Count(bp => bp == -1) != 1);

    //    //// Check simulateneous equations of input
    //    //bool allSimultaneousEquationsPassed = true;
    //    //foreach (var equation in simultaneousEquations)
    //    //{
    //    //    bool allButtonPushValuesKnown = true;
    //    //    int sum = 0;
    //    //    foreach (var buttonInEquationIndex in equation.ButtonIndexes)
    //    //    {
    //    //        if (buttonPushesByButtonIndex[buttonInEquationIndex] != -1)
    //    //            sum += buttonPushesByButtonIndex[buttonInEquationIndex];
    //    //        else
    //    //        {
    //    //            allButtonPushValuesKnown = false;
    //    //            break;
    //    //        }
    //    //    }
    //    //    if (allButtonPushValuesKnown)
    //    //    {
    //    //        if (sum == equation.TargetSum)
    //    //        {
    //    //            // This equation is fulfilled
    //    //        }
    //    //        else
    //    //        {
    //    //            // Fails equation - no way to recover
    //    //            allSimultaneousEquationsPassed = false;
    //    //            return false;
    //    //        }
    //    //    }
    //    //    else
    //    //    {
    //    //        // Can't check this equation as we haven't picked a value for this button
    //    //        allSimultaneousEquationsPassed = false;
    //    //    }
    //    //}

    //    //if (allSimultaneousEquationsPassed)
    //    //    return buttonPushesByButtonIndex.Sum();

    //    // Check joltages equal maybe? Possibly more efficient than simultaneous equations
    //    if (buttonIndex == machine.Buttons.Count)
    //    {
    //        //Console.WriteLine(string.Join(',', buttonPushesByButtonIndex));
    //        if (joltages.SequenceEqual(machine.TargetJoltages))
    //        {
    //            Debug.Assert(buttonPushesByButtonIndex.All(bp => bp >= 0));
    //            return buttonPushesByButtonIndex.Sum();
    //        }
    //        else
    //        {
    //            return int.MaxValue;
    //        }
    //    }

    //    // At this point we satisfy the known simultaneous equations but not all of them
    //    var button = machine.Buttons[buttonIndex];

    //    int maxPushes = int.MaxValue;
    //    foreach (int index in button)
    //        maxPushes = Math.Min(maxPushes, machine.TargetJoltages[index] - joltages[index]);

    //    int minButtonPushes = int.MaxValue;
    //    for (int pushes = 0; pushes <= maxPushes; pushes++)
    //    {
    //        var newButtonPushesByButtonIndex = buttonPushesByButtonIndex.ToArray();
    //        newButtonPushesByButtonIndex[buttonIndex] = pushes;

    //        // Do simultaneous equations where possible
    //        bool valid = extrapolateFromSimultaneousEquations(simultaneousEquations, newButtonPushesByButtonIndex);
    //        if (!valid) continue;

    //        if (check(simultaneousEquations, newButtonPushesByButtonIndex))
    //        {
    //            int[] newJoltages = new int[machine.TargetJoltages.Length];
    //            for (int i = 0; i < newButtonPushesByButtonIndex.Length; i++)
    //            {
    //                if (newButtonPushesByButtonIndex[i] != -1)
    //                {
    //                    foreach (int joltageIndex in machine.Buttons[i])
    //                        newJoltages[joltageIndex] += newButtonPushesByButtonIndex[i];
    //                }
    //            }

    //            // Find the first unknown buttonIndex
    //            int nextButtonIndex;
    //            for (nextButtonIndex = 0; nextButtonIndex < machine.Buttons.Count && newButtonPushesByButtonIndex[nextButtonIndex] != -1; nextButtonIndex++) { }
    //            minButtonPushes = Math.Min(minButtonPushes, recurse(machine, simultaneousEquations, newButtonPushesByButtonIndex, newJoltages, nextButtonIndex));
    //        }
    //    }
    //    return minButtonPushes;
    //}

    //int i = 0;

    //foreach (var machine in machines)
    //{
    //    Console.WriteLine($"{machine.Buttons.Count} buttons {machine.TargetJoltages.Length} target joltages");

    //    // Construct simultaneous equations
    //    List<(List<int>, int)> simultaneousEquations = new();
    //    for (int joltageIndex = 0; joltageIndex < machine.TargetJoltages.Length; joltageIndex++)
    //    {
    //        // The target sum is machine.TargetJoltages[joltageIndex]
    //        // Need to figure out which buttons contribute to this sum
    //        List<int> buttonIndexesInEquation = new();
    //        for (int buttonIndex = 0; buttonIndex < machine.Buttons.Count; buttonIndex++)
    //        {
    //            var button = machine.Buttons[buttonIndex];
    //            if (button.Contains(joltageIndex))
    //            {
    //                // Contributes to the simultaneous equation
    //                buttonIndexesInEquation.Add(buttonIndex);
    //            }
    //        }
    //        simultaneousEquations.Add((buttonIndexesInEquation, machine.TargetJoltages[joltageIndex]));
    //    }

    //    int minButtonPushes = recurse(machine, simultaneousEquations, Enumerable.Repeat(-1, machine.Buttons.Count).ToArray(), new int[machine.TargetJoltages.Length], 0);
    //    Console.WriteLine($"{i}/{machines.Count} {minButtonPushes}");
    //    //Console.ReadLine();
    //    result += minButtonPushes;

    //    i++;
    //}

    foreach (Machine machine in machines)
    {
        using (var z3Context = new Microsoft.Z3.Context())
        {
            var z3AllVariables = machine.Buttons.Select(b => z3Context.MkIntConst(string.Join(',', b))).ToList();
            var z3Optimiser = z3Context.MkOptimize();

            // Direct Z3 that it needs to minimise the variables (button presses)
            z3Optimiser.MkMinimize(z3Context.MkAdd(z3AllVariables));
            // Constrain all the variables (button presses) to >= 0
            z3AllVariables.ForEach(z3Var => z3Optimiser.Add(z3Context.MkGe(z3Var, z3Context.MkInt(0))));

            for (int joltageIndex = 0; joltageIndex < machine.TargetJoltages.Length; joltageIndex++)
            {
                List<IntExpr> z3Variables = new();
                for (int buttonIndex = 0; buttonIndex < machine.Buttons.Count; buttonIndex++)
                {
                    if (machine.Buttons[buttonIndex].Contains(joltageIndex))
                    {
                        z3Variables.Add(z3AllVariables[buttonIndex]);
                    }
                }
                // Constrain that the sum of the relevant variables must be the target joltage
                ArithExpr z3SumExpr = z3Context.MkAdd(z3Variables);
                Expr z3SumTarget = z3Context.MkInt(machine.TargetJoltages[joltageIndex]);
                z3Optimiser.Add(z3Context.MkEq(z3SumExpr, z3SumTarget));
            }

            z3Optimiser.Check();
            // Extract the values from the check call
            result += z3AllVariables.Sum(z3Var => ((IntNum)z3Optimiser.Model.Eval(z3Var)).Int);
        }
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
