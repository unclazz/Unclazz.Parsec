using System;

namespace Unclazz.Parsec
{
    public struct ResultCore<T>
    {
        public static implicit operator ResultCore<T>(Result<T> res)
        {
            return res.DetachPosition();
        }

        public static ResultCore<T> OfSuccess(T value, bool canBacktrack = true)
        {
            return new ResultCore<T>(true, null, value, canBacktrack);
        }
        public static ResultCore<T> OfFailure(string message, bool canBacktrack = true)
        {
            return new ResultCore<T>(false, message, default(T), canBacktrack);
        }

        ResultCore(bool successful, string message, T value, bool canBacktrack)
        {
            _successful = successful;
            _message = message;
            _value = value;
            _canBacktrack = canBacktrack;
        }

        readonly bool _successful;
        readonly string _message;
        readonly T _value;
        readonly bool _canBacktrack;

        public bool Successful => _successful;
        public bool CanBacktrack => _canBacktrack;
        public string Message => !_successful ? _message : throw new InvalidOperationException();
        public T Value => _successful ? _value : throw new InvalidOperationException();

        public ResultCore<T> AllowBacktrack(bool yesNo)
        {
            return new ResultCore<T>(_successful, _message, _value, yesNo);
        }
        public Result<T> AttachPosition(CharacterPosition start, CharacterPosition end)
        {
            if (_successful)
            {
                return Result<T>.OfSuccess(_value, start, end, _canBacktrack);
            }
            else
            {
                return Result<T>.OfFailure(_message, start, end, _canBacktrack);
            }
        }
        public override string ToString()
        {
            if (_successful)
            {
                return string.Format("ResultCore<{2}>(Successful: {0}, Value: {1})",
                    _successful, _value, ParsecUtility.TypeToString(typeof(T)));
            }
            else
            {
                return string.Format("ResultCore<{2}>(Successful: {0}, Message: {1})",
                    _successful, _message, ParsecUtility.TypeToString(typeof(T)));
            }
        }
    }
    public struct Result<T>
    {
        public static Result<T> OfSuccess(T value,
            CharacterPosition start,
            CharacterPosition end,
            bool canBacktrack = true)
        {
            return new Result<T>(true, start, end, null, value, canBacktrack);
        }
        public static Result<T> OfFailure(string message,
            CharacterPosition start,
            CharacterPosition end,
            bool canBacktrack = true)
        {
            return new Result<T>(false, start, end, message, default(T), canBacktrack);
        }

        Result(bool successful, 
            CharacterPosition start, 
            CharacterPosition end, 
            string message, 
            T value, 
            bool canBacktrack)
        {
            _start = start;
            _end = end;
            _successful = successful;
            _message = message;
            _value = value;
            _canBacktrack = canBacktrack;
        }

        readonly CharacterPosition _start;
        readonly CharacterPosition _end;
        readonly bool _successful;
        readonly string _message;
        readonly T _value;
        readonly bool _canBacktrack;

        public CharacterPosition Start => _start;
        public CharacterPosition End => _end;
        public bool Successful => _successful;
        public bool CanBacktrack => _canBacktrack;
        public string Message => !_successful ? _message : throw new InvalidOperationException();
        public T Value => _successful ? _value : throw new InvalidOperationException();

        public Result<T> AllowBacktrack(bool yesNo)
        {
            return new Result<T>(_successful, _start, _end, _message, _value, yesNo);
        }
        public Result<U> Map<U>(Func<T, U> func)
        {
            return new Result<U>(_successful, _start, _end, _message,
                _successful ? func(_value) : default(U), _canBacktrack);
        }
        public Result DetachValue()
        {
            if (_successful)
            {
                return Result.OfSuccess(_start, _end, _canBacktrack);
            }
            else
            {
                return Result.OfFailure(_message, _start, _end, _canBacktrack);
            }
        }
        public Result<U> Cast<U>()
        {
            return Cast(default(U));
        }
        public Result<U> Cast<U>(U value)
        {
            if (_successful)
            {
                return Result<U>.OfSuccess(value, _start, _end, _canBacktrack);
            }
            else
            {
                return Result<U>.OfFailure(_message, _start, _end, _canBacktrack);
            }
        }
        public void IfSuccessful(Action<T> act)
        {
            if (_successful) act(_value);
        }
        public void IfSuccessful(Action<T> act, Action<string> orElse)
        {
            if (_successful) act(_value);
            else orElse(_message);
        }
        public void IfFailed(Action<string> act)
        {
            if (!_successful) act(_message);
        }
        public ResultCore<T> DetachPosition()
        {
            if (_successful)
            {
                return ResultCore<T>.OfSuccess(_value, _canBacktrack);
            }
            else
            {
                return ResultCore<T>.OfFailure(_message, _canBacktrack);
            }
        }
        public override string ToString()
        {
            if (_successful)
            {
                return string.Format("Result<{4}>(Successful: {0}, Value: {1}, Start: {2}, End: {3})",
                    _successful, _value, _start, _end, ParsecUtility.TypeToString(typeof(T)));
            }
            else
            {
                return string.Format("Result<{4}>(Successful: {0}, Message: {1}, Start: {2}, End: {3})",
                    _successful, _message, _start, _end, ParsecUtility.TypeToString(typeof(T)));
            }
        }
    }
}
