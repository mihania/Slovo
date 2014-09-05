using System;

namespace Slovo.Core
{
    public class VocabularyNotFoundException : Exception
    {
        public VocabularyNotFoundException(string message) : base(message) {}
    }
}
