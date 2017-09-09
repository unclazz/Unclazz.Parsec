using System;
using System.Linq;
using Unclazz.Parsec;

namespace Example.Unclazz.Parsec
{
    class Program : Parser<string>
    {
        static void Main(string[] args)
        {
            // 何らかの入力データソースを用意
            var text = args.Aggregate((a, b) => a + ' ' + b);
            // 文字列、ファイル、その他ストリームからReaderオブジェクトを生成
            var input = Reader.From(text);
            // パースを行う
            var result = new Program().Parse(input);

            // 成功したら結果をSTDOUT、さもなくばメッセージをSTDERRに出力
            result.IfSuccessful(Console.WriteLine,　Console.Error.WriteLine);
        }

        readonly Parser _hello;
        readonly Parser<Seq<string>> _helloHello;
        readonly Parser _spEof;
        readonly Parser<string> _helloX;

        Program()
        {
            // "hello"というキーワードにマッチするパーサー（キャプチャなし）
            _hello = Keyword("hello");
            // キャプチャありに切替え、1回以上の繰返し、かつセパレータとして空白文字を指定
            _helloHello = _hello.Capture().Repeat(min:1, sep: WhileSpaceAndControls);
            // 空白文字とEOFにマッチするパーサー（キャプチャなし）
            _spEof = WhileSpaceAndControls & EndOfFile;
            // "hello"の繰返しをintに、その後さらにstringに変換、後続にはEOF
            _helloX = _helloHello.Map(a => a.Count).Map(a => string.Format("hello x {0}", a)) & _spEof;
        }

        protected override ResultCore<string> DoParse(Reader input)
        {
            return _helloX.Parse(input);
        }
    }
}
