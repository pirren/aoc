namespace aoc_runner.Y2023.Day15;

[PuzzleName("Lens Library")]
class Solution : ISolver
{
    public object PartOne(string input) 
    {
        return Solve(input, SequenceSum);
    }

    public object PartTwo(string input) 
    {
        return Solve(input, FocusingPower);
    }

    int Solve(string input, Func<Step[], Box[], int> parse)
    {
        var steps = GetSteps(input).ToArray();
        var boxes = GetEmptyBoxes().ToArray();

        return parse(steps, boxes);
    }

    int SequenceSum(Step[] steps, Box[] boxes)
    {
        var sequenceSum = 0;
        foreach (var step in steps)
        {
            sequenceSum += GetHash(step.ToString());
        }
        
        return sequenceSum;
    }

    int FocusingPower(Step[] steps, Box[] boxes)
    {
        foreach (var step in steps)
        {
            var boxNumber = GetHash(step.Label);
            var relevantBox = boxes[boxNumber];
            var existingContent = relevantBox.Contents.FirstOrDefault(bc => bc.Label == step.Label);

            if (step.Op == '=')
            {
                if (existingContent != null)
                    existingContent.Value = step.Value;
                else
                    relevantBox.Contents.Add(new BoxContent(step.Label, step.Value));
            }
            if (step.Op == '-' && existingContent != null)
            {
                relevantBox.Contents.Remove(existingContent);
            }
        }

        return boxes.SelectMany((box, boxNumber) => box.Contents.Select((content, slotNumber) => 
            (boxNumber + 1) * (slotNumber + 1) * content.Value!.Value)).Sum();
    }


    int GetHash(string value) =>
        value.Aggregate(0, (result, ch) => (result + ch) * 17 % 256);

    IEnumerable<Step> GetSteps(string indata)
    {
        foreach (var line in indata.Split(','))
        {
            char op = line.Contains('-') ? '-' : '=';
            var parts = line.Split(op);
            var label = parts[0];
            yield return new Step(label, op, int.TryParse(parts[^1], out var value) ? value : null);
        }
    }

    IEnumerable<Box> GetEmptyBoxes() =>
        Enumerable.Range(0, 255).Select(_ => new Box());

    record Step(string Label, char Op, int? Value)
    {
        public override string ToString() => 
            $"{Label}{Op}{Value}";
    }

    class Box
    {
        public List<BoxContent> Contents = [];
    }

    class BoxContent(string Label, int? Value)
    {
        public string Label { get; set; } = Label;
        public int? Value { get; set; } = Value;
    }
}
