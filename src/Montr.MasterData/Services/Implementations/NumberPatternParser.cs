using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Montr.MasterData.Models;

namespace Montr.MasterData.Services.Implementations
{
	public class NumberPatternParser
	{
		public ParsedNumberPattern Parse(string source)
		{
			if (source == null) throw new ArgumentNullException(nameof(source));

			var pattern = source.Trim();

			var tokens = new List<Token>();

			var buffer = new StringBuilder();
			var tag = new List<string>();

			var currentToken = TokenType.Text;

			for (var index = 0; index < pattern.Length; index++)
			{
				var c = pattern[index];

				if (c == '{')
				{
					if (currentToken == TokenType.Text)
					{
						if (buffer.Length > 0)
						{
							tokens.Add(new TextToken { Content = buffer.ToString() });
							buffer.Clear();
						}
					}
					else
					{
						throw new NumberPatternParseException($"Invalid start of tag at position {index}.")
						{
							Position = index, TokenType =  TokenType.TagBegin
						};
					}

					tag.Clear();

					currentToken = TokenType.TagBegin;
				}
				else if (c == '}')
				{
					if (currentToken == TokenType.TagName || currentToken == TokenType.TagArg)
					{
						tag.Add(buffer.ToString());
						buffer.Clear();
					}
					else
					{
						throw new NumberPatternParseException($"Invalid end of tag at position {index}.")
						{
							Position = index, TokenType = TokenType.TagEnd
						};
					}

					tokens.Add(new TagToken { Name = tag[0], Args = tag.Skip(1).ToArray() });

					tag.Clear();

					currentToken = TokenType.TagEnd;
				}
				else if (c == ':')
				{
					if (currentToken == TokenType.TagName || currentToken == TokenType.TagArg)
					{
						tag.Add(buffer.ToString());
						buffer.Clear();
						currentToken = TokenType.TagArgSeparator;
					}
					else if (currentToken == TokenType.Text || currentToken == TokenType.TagEnd)
					{
						buffer.Append(c);
						currentToken = TokenType.Text;
					}
					else
					{
						throw new NumberPatternParseException($"Invalid separator of tag argument at position {index}.")
						{
							Position = index, TokenType = TokenType.TagArgSeparator
						};
					}
				}
				else
				{
					if (currentToken == TokenType.TagBegin)
					{
						currentToken = TokenType.TagName;
					}
					else if (currentToken == TokenType.TagEnd)
					{
						currentToken = TokenType.Text;
					}
					else if (currentToken == TokenType.TagArgSeparator)
					{
						currentToken = TokenType.TagArg;
					}

					buffer.Append(c);
				}
			}

			if (currentToken == TokenType.TagEnd)
			{
				// noop
			}
			else if (currentToken == TokenType.Text)
			{
				if (buffer.Length > 0)
				{
					tokens.Add(new TextToken { Content = buffer.ToString() });
					buffer.Clear();
				}
			}
			else
			{
				throw new NumberPatternParseException("Invalid end of pattern.")
				{
					Position = pattern.Length, TokenType = TokenType.End
				};
			}

			return new ParsedNumberPattern
			{
				Tokens = tokens
			};
		}
	}

	public class ParsedNumberPattern
	{
		public IList<Token> Tokens { get; set; }

		public string Format(IDictionary<string, string> values)
		{
			var result = new StringBuilder();

			foreach (var token in Tokens)
			{
				if (token is TextToken textToken)
				{
					result.Append(textToken.Content);
				}
				else if (token is TagToken tagToken)
				{
					result.Append(values[tagToken.Name]);
				}
			}

			return result.ToString();
		}
	}
}
