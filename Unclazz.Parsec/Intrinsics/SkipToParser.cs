using System;
namespace Unclazz.Parsec.Intrinsics
{
    sealed class SkipToParser : Parser
    {
        internal SkipToParser(Parser end)
        {
            _end = end ?? throw new ArgumentNullException(nameof(end));
        }

        readonly Parser _end;

        protected override ResultCore DoParse(Context ctx)
        {
            var src = ctx.Source;

            while (!src.EndOfFile)
            {
                src.Mark();
                if (_end.Parse(ctx).Successful)
                {
                    return Success();
                }
                src.Reset(true);
                src.Read();
            }
            return Failure("end pattern not found.");
        }
    }
}
