using System;

namespace Unclazz.Parsec.Intrinsics
{
    sealed class RepeatConfiguration
    {
        public RepeatConfiguration(int min = 0, int max = -1, int exactly = -1, Parser sep = null)
        {
            // exactlyが明示的に指定されているかチェック
            if (exactly == -1)
            {
                max = max == -1 ? int.MaxValue : max;
                min = min == -1 ? 0 : min;

                if (max < 1) throw new ArgumentOutOfRangeException(nameof(max));
                if (min < 0) throw new ArgumentOutOfRangeException(nameof(max));
                if (max < min) throw new ArgumentOutOfRangeException(nameof(max));

                Minimal = min;
                Maximum = max;
                Breakable = min != max;
            }
            else
            {
                if (exactly <= 1) throw new ArgumentOutOfRangeException(nameof(exactly));
                Minimal = exactly;
                Maximum = exactly;
                Breakable = false;
            }
            Separator = sep;
        }
        public int Minimal { get; }
        public int Maximum { get; }
        public bool Breakable { get; }
        public Parser Separator { get; }
    }
}
