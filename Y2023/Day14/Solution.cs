namespace aoc_runner.Y2023.Day14;

[PuzzleName("Parabolic Reflector Dish")]
class Solution : ISolver
{
    public object PartOne(string input) =>
        Grid.Parse(input).Tilt().Weight();

    public object PartTwo(string input) =>
        Grid.Parse(input).FindRepeatingCycles().Weight();

    class Grid(char[,] grid, int cols, int rows)
    {
        public char[,] Matrix { get; private set; } = grid;
        public int Cols { get; private set; } = cols;
        public int Rows { get; private set; } = rows;

        public static Grid Parse(string indata)
        {
            var lines = indata.Split('\n');
            var rows = lines.Length;
            var cols = lines[0].Length;
            var grid = new char[cols, rows];

            for (var y = 0; y < lines.Length; y++)
                for (var x = 0; x < lines[0].Length; x++)
                    grid[x, y] = lines[y][x];

            return new Grid(grid, rows, cols);
        }

        public Grid Tilt()
        {
            var newMatrix = new Grid(Matrix, Cols, Rows);

            for (var r = 1; r < Rows; r++)
            {
                for (var c = 0; c < Cols; c++)
                {
                    if (Matrix[c, r] != 'O')
                        continue;

                    var cr = r;
                    while (cr > 0 && newMatrix.Matrix[c, --cr] == '.')
                    {
                        newMatrix.Matrix[c, cr + 1] = '.';
                        newMatrix.Matrix[c, cr] = 'O';
                    }
                }
            }

            return newMatrix;
        }

        public int Weight() => 
            Enumerable.Range(0, Rows).SelectMany(r => Enumerable.Range(0, Cols).Select(c => Matrix[c, r] == 'O' ? Rows - r : 0)).Sum();

        public Grid FindRepeatingCycles()
        {
            Dictionary<string, int> grids = [];
            var i = 1;
            var spinIndex = -1;
            var cycleLength = -1;

            while (true)
            {
                Cycle();
                var hash = Hash();

                if (grids.TryGetValue(hash, out int value))
                {
                    spinIndex = i;
                    cycleLength = i - value;
                    break;
                }

                grids[hash] = i++;
            }

            var remaining = 1_000_000_000 - spinIndex;
            var fullCycles = remaining / cycleLength;
            remaining -= fullCycles * cycleLength;

            while (remaining-- > 0)
                Cycle();

            return this;
        }

        void Rotate()
        {
            char[,] newMatrix = new char[Matrix.GetLength(1), Matrix.GetLength(0)];
            int newColumn, newRow = 0;
            for (int oldColumn = Matrix.GetLength(1) - 1; oldColumn >= 0; oldColumn--)
            {
                newColumn = 0;
                for (int oldRow = 0; oldRow < Matrix.GetLength(0); oldRow++)
                {
                    newMatrix[newRow, newColumn] = Matrix[oldRow, oldColumn];
                    newColumn++;
                }
                newRow++;
            }

            Matrix = newMatrix;
            Cols = newMatrix.GetLength(1);
            Rows = newMatrix.GetLength(0);
        }

        void Cycle()
        {
            for (var i = 0; i < 4; i++)
            {
                Tilt();
                Rotate();
            }
        }

        string Hash() =>
            string.Concat(Enumerable.Range(0, Rows)
                .SelectMany(row => Enumerable.Range(0, Cols).Select(col => Matrix[row, col])));
    }
}
