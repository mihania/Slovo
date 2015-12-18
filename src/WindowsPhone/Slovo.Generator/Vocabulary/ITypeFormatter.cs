namespace Slovo.Generator
{
    interface ITypeFormatter
    {
        string OutputPath { get; }

        string AlternateColorBegin { get; }

        string AlternateColorEnd { get; }

        string AccentColorBegin { get; }

        string NewLine { get; }

        string Escape(string text);

        bool IsValid(string definition, string word);
    }
}