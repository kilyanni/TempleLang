namespace TempleLang.Lexer
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Runtime.CompilerServices;
    using System.Text;
    using TempleLang.Lexer.Abstractions;
    using TempleLang.Lexer.Abstractions.Exceptions;

    /// <summary>
    /// Represents a lexer, able to lex the input from a <see cref="TextReader"/>
    /// </summary>
    public class Lexer : ILexer<Token>
    {
        private static readonly Dictionary<string, Token> _keywords = new Dictionary<string, Token>
        {
            ["if"] = Token.If,
            ["else"] = Token.Else,
            ["for"] = Token.For,
            ["while"] = Token.While,
            ["break"] = Token.Break,
            ["continue"] = Token.Continue,
            ["do"] = Token.Do,
            ["let"] = Token.Let,
            ["return"] = Token.Return,
            ["ref"] = Token.Reference,
            ["deref"] = Token.Dereference,
            ["using"] = Token.Using,
            ["import"] = Token.Import,
            ["namespace"] = Token.Namespace,
            ["proc"] = Token.Proc
        };

        public Lexer(TextReader textReader, SourceFile sourceFile)
        {
            TextReader = textReader;
            SourceFile = sourceFile;

            TokenBuffer = new StringBuilder();
        }

        private int TokenIndex, TokenCharStartIndex, CurrentCharIndex;
        private readonly StringBuilder TokenBuffer;
        private readonly TextReader TextReader;
        private readonly SourceFile SourceFile;

        /// <summary>
        /// Lexes the text far enough to generate one lexeme
        /// </summary>
        /// <returns>The topmost token from the text</returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public Lexeme<Token> LexOne()
        {
            int characterInt;
            char character;

            Start:

                do
                {
                    characterInt = AdvanceChar();

                    if (characterInt == -1)
                        {
                            return MakeLexeme(Token.EoF);
                        }

                    character = (char)characterInt;
                }
                while (char.IsWhiteSpace(character));

            if (character == '/' && PeekChar() == '/')
            {
                characterInt = AdvanceChar();

                while (characterInt != -1 && characterInt != '\n') characterInt = AdvanceChar();

                if (characterInt == -1)
                {
                    return MakeLexeme(Token.EoF);
                }

                goto Start;
            }

            TokenBuffer.Append(character);

            switch (character)
            {
                case '+':
                    return MakeLexeme(SwitchOnNextCharacter(
                        Token.Add,
                        ('+', Token.Increment),
                        ('=', Token.AddCompoundAssign)));

                case '-':
                    return MakeLexeme(SwitchOnNextCharacter(
                        Token.Subtract,
                        ('-', Token.Decrement),
                        ('=', Token.SubtractCompoundAssign)));

                case '*':
                    return MakeLexeme(SwitchOnNextCharacter(
                        Token.Multiply,
                        ('=', Token.MultiplyCompoundAssign)));

                case '/':
                    return MakeLexeme(SwitchOnNextCharacter(
                        Token.Divide,
                        ('=', Token.DivideCompoundAssign)));

                case '%':
                    return MakeLexeme(SwitchOnNextCharacter(
                        Token.Remainder,
                        ('=', Token.RemainderCompoundAssign)));

                case '!':
                    return MakeLexeme(
                        SwitchOnNextCharacter(Token.LogicalNot,
                        ('=', Token.ComparisonNotEqual)));

                case '~':
                    return MakeLexeme(
                        Token.BitwiseNot);

                case '^':
                    return MakeLexeme(SwitchOnNextCharacter(
                        Token.BitwiseXor,
                        ('=', Token.BitwiseXorCompoundAssign)));

                case '=':
                    return MakeLexeme(SwitchOnNextCharacter(
                        Token.Assign,
                        ('=', Token.ComparisonEqual),
                        ('>', Token.Arrow)));

                case ':':
                    return MakeLexeme(SwitchOnNextCharacter(
                        Token.Colon,
                        (':', Token.StaticAccessor)));

                case '&':
                    var andType = SwitchOnNextCharacter(
                        Token.BitwiseAnd,
                        ('=', Token.BitwiseAndCompoundAssign),
                        ('&', Token.LogicalAnd));
                    if (andType == Token.LogicalAnd)
                    {
                        andType = SwitchOnNextCharacter(
                            Token.LogicalAnd,
                            ('=', Token.LogicalAndCompoundAssign));
                    }
                    return MakeLexeme(andType);

                case '|':
                    var orType = SwitchOnNextCharacter(
                        Token.BitwiseOr,
                        ('=', Token.BitwiseOrCompoundAssign),
                        ('|', Token.LogicalOr));
                    if (orType == Token.LogicalOr)
                    {
                        orType = SwitchOnNextCharacter(
                            Token.LogicalOr,
                            ('=', Token.LogicalOrCompoundAssign));
                    }
                    return MakeLexeme(orType);

                case '<':
                    var lessType = SwitchOnNextCharacter(
                        Token.ComparisonLessThan,
                        ('=', Token.ComparisonLessThanOrEqual),
                        ('-', Token.ReferenceAssign),
                        ('<', Token.BitshiftLeft));
                    if (lessType == Token.BitshiftLeft)
                    {
                        lessType = SwitchOnNextCharacter(
                            Token.BitshiftLeft,
                            ('=', Token.BitshiftLeftCompoundAssign));
                    }
                    return MakeLexeme(lessType);

                case '>':
                    var greaterType = SwitchOnNextCharacter(
                        Token.ComparisonGreaterThan,
                        ('=', Token.ComparisonGreaterThanOrEqual),
                        ('>', Token.BitshiftRight));
                    if (greaterType == Token.BitshiftRight)
                    {
                        greaterType = SwitchOnNextCharacter(
                            Token.BitshiftRight,
                            ('=', Token.BitshiftRightCompoundAssign));
                    }
                    return MakeLexeme(greaterType);

                case '\'':
                    LexEscapableChar(Token.CharacterLiteral);
                    LexChar('\'', Token.CharacterLiteral);
                    return MakeLexeme(Token.CharacterLiteral);

                case '\"':
                    while (true)
                    {
                        int nextCharacterInt = PeekChar();

                        if (nextCharacterInt == -1)
                        {
                            throw UnexpectedCharException.Create(null, Token.StringLiteral, "a char or a string delimiter");
                        }

                        char nextCharacter = (char)nextCharacterInt;

                        if (nextCharacter == '\"')
                        {
                            AdvanceChar();
                            TokenBuffer.Append(nextCharacter);
                            break;
                        }

                        LexEscapableChar(Token.StringLiteral);
                    }
                    return MakeLexeme(Token.StringLiteral);

                case '{':
                    return MakeLexeme(Token.LBraces);

                case '}':
                    return MakeLexeme(Token.RBraces);

                case '[':
                    return MakeLexeme(Token.LBrackets);

                case ']':
                    return MakeLexeme(Token.RBrackets);

                case '(':
                    return MakeLexeme(Token.LParens);

                case ')':
                    return MakeLexeme(Token.RParens);

                case ';':
                    return MakeLexeme(Token.Semicolon);

                case '?':
                    return MakeLexeme(Token.QuestionMark);

                case ',':
                    return MakeLexeme(Token.Comma);
            }

            // Identifier or keyword
            if (char.IsLetter(character) || character == '_')
            {
                while (true)
                {
                    int nextCharacterInt = PeekChar();

                    if (nextCharacterInt == -1) break;

                    char nextCharacter = (char)nextCharacterInt;

                    if (!char.IsLetterOrDigit(nextCharacter) && nextCharacter != '_') break;

                    AdvanceChar();

                    TokenBuffer.Append(nextCharacter);
                }

                return MakeLexeme(_keywords.TryGetValue(TokenBuffer.ToString(), out Token keyword) ? keyword : Token.Identifier);
            }

            // Numerical literal
            {
                bool reachedDot = false;

                if (character == '.')
                {
                    reachedDot = true;

                    int nextCharacterInt = PeekChar();

                    if (nextCharacterInt == -1)
                    {
                        throw UnexpectedCharException.Create(null, Token.FloatLiteral, "Digit");
                    }

                    character = (char)nextCharacterInt;

                    if (!char.IsDigit(character))
                    {
                        return MakeLexeme(Token.Accessor);
                        //throw UnexpectedCharException.Create(character, Token.FloatLiteral, "Digit");
                    }
                }

                if (char.IsDigit(character))
                {
                    LexNumber(ref reachedDot);

                    return reachedDot
                        ? MakeLexeme(Token.FloatLiteral)
                        : MakeLexeme(Token.IntegerLiteral);
                }
            }

            throw UnexpectedCharException.Create(character, Token.EoF, "EoF");
        }

        private void LexNumber(ref bool reachedDot)
        {
            while (true)
            {
                int nextCharacterInt = PeekChar();

                if (nextCharacterInt == -1) break;

                char nextCharacter = (char)nextCharacterInt;

                if (!char.IsDigit(nextCharacter))
                {
                    if (!reachedDot && nextCharacter == '.')
                    {
                        reachedDot = true;
                    }
                    else
                    {
                        break;
                    }
                }

                AdvanceChar();

                TokenBuffer.Append(nextCharacter);
            }
        }

        private int PeekChar() => TextReader.Peek();

        private int AdvanceChar()
        {
            CurrentCharIndex++;
            return TextReader.Read();
        }

        private void LexChar(char characterToMatch, Token context)
        {
            int characterInt = PeekChar();

            if (characterInt == -1) return;

            char character = (char)characterInt;

            if (character != characterToMatch)
            {
                throw UnexpectedCharException.Create(character, context, characterToMatch);
            }

            AdvanceChar();
        }

        private Lexeme<Token> MakeLexeme(Token tokenType)
        {
            var token = new Lexeme<Token>(TokenBuffer.ToString(), tokenType, SourceFile, TokenIndex, TokenCharStartIndex, CurrentCharIndex);

            TokenCharStartIndex = CurrentCharIndex + 1;
            TokenIndex++;
            TokenBuffer.Clear();

            return token;
        }

        private Token SwitchOnNextCharacter(Token fallback, params (char character, Token tokenType)[] options)
        // TODO: Get around params array heap allocation (C#9 params Span?)
        {
            int characterInt = PeekChar();
            if (characterInt == -1) return fallback;

            var match = Array.Find(options, x => x.character == (char)characterInt);

            if (match != default)
            {
                TokenBuffer.Append((char)characterInt);
                AdvanceChar();
                return match.tokenType;
            }

            return fallback;
        }

        private void LexEscapableChar(Token context)
        {
            int characterInt = PeekChar();

            if (characterInt == -1) return;

            char character = (char)characterInt;

            if (character == '\\')
            {
                TokenBuffer.Append(character);
                AdvanceChar();

                int nextCharacterInt = PeekChar();

                if (nextCharacterInt == -1) throw UnexpectedCharException.Create(null, context, "escapable char");

                char nextCharacter = (char)nextCharacterInt;

                switch (nextCharacter)
                {
                    case '\'':
                    case '"':
                    case '\\':
                    case 'n':
                    case 'r':
                    case 't':
                    case '0':
                        AdvanceChar();
                        break;

                    case 'u':
                        AdvanceChar();
                        LexHexDigit(context);
                        LexHexDigit(context);
                        LexHexDigit(context);
                        LexHexDigit(context);
                        break;
                }

                TokenBuffer.Append(nextCharacter);
            }
            else
            {
                AdvanceChar();
                TokenBuffer.Append(character);
            }
        }

        private void LexHexDigit(Token context)
        {
            int characterInt = PeekChar();

            if (characterInt == -1) return;

            char character = (char)characterInt;

            if (!char.IsDigit(character)
                && character != 'a' && character != 'A'
                && character != 'b' && character != 'B'
                && character != 'c' && character != 'C'
                && character != 'd' && character != 'D'
                && character != 'e' && character != 'E'
                && character != 'f' && character != 'F')
            {
                throw UnexpectedCharException.Create(character, context, "Hex digit");
            }

            AdvanceChar();
        }
    }
}