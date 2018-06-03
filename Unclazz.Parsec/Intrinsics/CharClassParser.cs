﻿using System;

namespace Unclazz.Parsec.Intrinsics
{
    sealed class CharClassParser : CharParser
    {
        internal CharClassParser(CharClass clazz) : base("CharClass")
        {
            _clazz = clazz ?? throw new ArgumentNullException(nameof(clazz));
        }

        readonly CharClass _clazz;

        protected override ResultCore DoParse(Reader src)
        {
            var ch = src.Read();
            if (0 <= ch && _clazz.Contains((char)ch))
            {
                return Success();
            }
            else
            {
                return Failure(string.Format("a member of {0} expected but {1} found.", 
                    _clazz, ParsecUtility.CharToString(ch)));
            }
        }
        public override string ToString()
        {
            return string.Format("CharClass({0})", _clazz);
        }
    }
}
