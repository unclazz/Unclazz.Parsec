using System;

namespace Unclazz.Parsec
{
    public struct ResultCore
    {
        public static implicit operator ResultCore(Result res)
        {
            return res.DetachPosition();
        }

        public static ResultCore OfSuccess(bool canBacktrack = true)
        {
            return new ResultCore(true, null, canBacktrack);
        }
        public static ResultCore OfFailure(string message, bool canBacktrack = true)
        {
            return new ResultCore(false, message, canBacktrack);
        }

        ResultCore(bool successful, string message, bool canBacktrack)
        {
            _successful = successful;
            _message = message;
            _canBacktrack = canBacktrack;
        }

        readonly bool _successful;
        readonly string _message;
        readonly bool _canBacktrack;

        public bool Successful => _successful;
        public bool CanBacktrack => _canBacktrack;
        public string Message => !_successful ? _message : throw new InvalidOperationException();

        public ResultCore AllowBacktrack(bool yesNo)
        {
            return new ResultCore(_successful, _message, yesNo);
        }
        public Result AttachPosition(CharacterPosition start, CharacterPosition end)
        {
            if (_successful)
            {
                return Result.OfSuccess(start, end, _canBacktrack);
            }
            else
            {
                return Result.OfFailure(_message, start, end, _canBacktrack);
            }
        }
        public override string ToString()
        {
            if (_successful)
            {
                return string.Format("ResultCore(Successful: {0})", _successful);
            }
            else
            {
                return string.Format("ResultCore(Successful: {0}, Message: {1})", _successful, _message);
            }
        }
    }
    public struct Result
    {
        public static Result operator |(Result left, Result right)
        {
            return left.Or(right);
        }

        public static Result OfSuccess(
            CharacterPosition start,
            CharacterPosition end,
            bool canBacktrack = true)
        {
            return new Result(true, start, end, null, canBacktrack);
        }
        public static Result OfFailure(
            string message,
            CharacterPosition start,
            CharacterPosition end,
            bool canBacktrack = true)
        {
            return new Result(false, start, end, message, canBacktrack);
        }

        Result(bool successful,
            CharacterPosition start,
            CharacterPosition end,
            string message,
            bool canBacktrack)
        {
            _start = start;
            _end = end;
            _successful = successful;
            _message = message;
            _canBacktrack = canBacktrack;
        }

        readonly CharacterPosition _start;
        readonly CharacterPosition _end;
        readonly bool _successful;
        readonly string _message;
        readonly bool _canBacktrack;

        public CharacterPosition Start => _start;
        public CharacterPosition End => _end;
        public bool Successful => _successful;
        public bool CanBacktrack => _canBacktrack;
        public string Message => !_successful ? _message : throw new InvalidOperationException();

        public Result AllowBacktrack(bool yesNo)
        {
            return new Result(_successful, _start, _end, _message, yesNo);
        }
        public Result<T> AttachValue<T>(T value)
        {
            if (_successful)
            {
                return Result<T>.OfSuccess(value, _start, _end, _canBacktrack);
            }
            else
            {
                return Result<T>.OfFailure(_message, _start, _end, _canBacktrack);
            }
        }
        public Result<T> Cast<T>()
        {
            return Cast(default(T));
        }
        public Result<T> Cast<T>(T value)
        {
            if (_successful)
            {
                return Result<T>.OfSuccess(value, _start, _end, _canBacktrack);
            }
            else
            {
                return Result<T>.OfFailure(_message, _start, _end, _canBacktrack);
            }
        }
        public Result Or(Result other)
        {
            return _successful ? this : other;
        }
        public void IfSuccessful(Action act)
        {
            if (_successful) act();
        }
        public void IfSuccessful(Action act, Action<string> orElse)
        {
            if (_successful) act();
            else orElse(_message);
        }
        public void IfFailed(Action<string> act)
        {
            if (!_successful) act(_message);
        }
        public ResultCore DetachPosition()
        {
            if (_successful)
            {
                return ResultCore.OfSuccess(_canBacktrack);
            }
            else
            {
                return ResultCore.OfFailure(_message, _canBacktrack);
            }
        }
        public override string ToString()
        {
            if (_successful)
            {
                return string.Format("Result(Successful: {0}, Start: {1}, End: {2})",
                    _successful, _start, _end);
            }
            else
            {
                return string.Format("Result(Successful: {0}, Message: {1}, Start: {2}, End: {3})",
                    _successful, _message, _start, _end);
            }
        }
    }
}
