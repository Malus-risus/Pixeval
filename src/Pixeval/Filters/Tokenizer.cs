using System;
using System.Collections.Generic;

namespace Pixeval.Filters;

public static class Tokenizer
{
    /// <summary>
    /// Tokenize function for tag parser, optimized for performance.
    /// Returns a flow of tokenized nodes carrying data of the input string, structured.
    /// </summary>
    /// <param name="src"></param>
    /// <returns></returns>
    public static IList<IQueryToken> Tokenize(string src)
    {
        var tokens = new List<IQueryToken>();
        var index = 0;

        while (index < src.Length)
        {
            switch (src[index])
            {
                case '+':
                    tokens.Add(new IQueryToken.Plus());
                    index++;
                    break;
                case '-':
                    tokens.Add(new IQueryToken.Dash());
                    index++;
                    break;
                case '.':
                    tokens.Add(new IQueryToken.Dot());
                    index++;
                    break;
                case '@':
                    tokens.Add(new IQueryToken.At());
                    index++;
                    break;
                case '/':
                    tokens.Add(new IQueryToken.Slash());
                    index++;
                    break;
                case ':':
                    tokens.Add(new IQueryToken.Colon());
                    index++;
                    if (index < src.Length)
                    {
                        switch (src[index])
                        {
                            case 'l':
                                tokens.Add(new IQueryToken.Like());
                                index++;
                                break;
                            case 'i':
                                tokens.Add(new IQueryToken.Index());
                                index++;
                                break;
                            case 's':
                                tokens.Add(new IQueryToken.StartDate());
                                index++;
                                break;
                            case 'e':
                                tokens.Add(new IQueryToken.EndDate());
                                index++;
                                break;
                            case 'r':
                                tokens.Add(new IQueryToken.Ratio());
                                index++;
                                break;
                        }
                    }
                    break;
                case ',':
                    tokens.Add(new IQueryToken.Comma());
                    index++;
                    break;
                case '(':
                    tokens.Add(new IQueryToken.LeftParen());
                    index++;
                    break;
                case '[':
                    tokens.Add(new IQueryToken.LeftBracket());
                    index++;
                    break;
                case ')':
                    tokens.Add(new IQueryToken.RightParen());
                    index++;
                    break;
                case ']':
                    tokens.Add(new IQueryToken.RightBracket());
                    index++;
                    break;
                case '#':
                    tokens.Add(new IQueryToken.Hashtag());
                    index++;
                    break;
                case '!':
                    tokens.Add(new IQueryToken.Not());
                    index++;
                    break;
                case 'a':
                    if (src.Substring(index, 3) == "and")
                    {
                        tokens.Add(new IQueryToken.And());
                        index += 3;
                    }
                    break;
                case 'o':
                    if (src.Substring(index, 2) == "or")
                    {
                        tokens.Add(new IQueryToken.Or());
                        index += 2;
                    }
                    break;
                case '\"':
                    var endIndex = src.IndexOf('\"', index + 1);
                    if (endIndex != -1)
                    {
                        tokens.Add(new IQueryToken.Data(src.Substring(index + 1, endIndex - index - 1)));
                        index = endIndex + 1;
                    }
                    else
                    {
                        index = src.Length;
                    }
                    break;
                default:
                    if (char.IsWhiteSpace(src[index]))
                    {
                        index++;
                    }
                    else if (char.IsDigit(src[index]))
                    {
                        var startIndex = index;
                        while (index < src.Length && char.IsDigit(src[index]))
                        {
                            index++;
                        }
                        tokens.Add(new IQueryToken.Number(src.Substring(startIndex, index - startIndex)));
                    }
                    else
                    {
                        var startIndex = index;
                        while (index < src.Length && !char.IsWhiteSpace(src[index]) && src[index] != '\"')
                        {
                            index++;
                        }
                        tokens.Add(new IQueryToken.Data(src.Substring(startIndex, index - startIndex)));
                    }
                    break;
            }
        }

        return tokens;
    }
}
