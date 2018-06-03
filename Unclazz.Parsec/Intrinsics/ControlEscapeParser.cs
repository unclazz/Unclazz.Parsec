using System.Text;
using System.Threading.Tasks;

namespace Unclazz.Parsec.Intrinsics
{
    sealed class ControlEscapeParser : Parser<char>
    {
        internal ControlEscapeParser(char prefix = '\\') : base("ControlEscape")
        {
            _control = Char(prefix) & CharIn("0abtnvfre").Capture().Map(Unescape);
        }
        readonly Parser<char> _control;
        protected override ResultCore<char> DoParse(Reader src)
        {
            return _control.Parse(src);
        }
        char Unescape(char escape)
        {
            switch (escape)
            {
                case '0': return '\0';
                case 'a': return '\a';
                case 'b': return '\b';
                case 't': return '\t';
                case 'n': return '\n';
                case 'v': return '\v';
                case 'f': return '\f';
                case 'r': return '\r';
                case 'e': return (char)27;
                default: return escape;
            }
        }
    }
}
