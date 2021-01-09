using System;

namespace Brains
{
    public class InputSizeMismatchException : Exception
    {
        public InputSizeMismatchException(string message) : base(message) { }
    }
}