using Bau.Libraries.LibInterpreter.Models.Tokens;

namespace Bau.Libraries.LibInterpreter.Lexer;

/// <summary>
///		Manager para el proceso de obtener los tokens de un texto
/// </summary>
public class LexerManager(Rules.RulesDefinition rules)
{
	/// <summary>
	///		Interpreta un texto
	/// </summary>
	public TokenCollection Parse(string source) => new Parser.StringTokenSeparator(source, Rules).Parse();

	/// <summary>
	///		Reglas para obtener tokens
	/// </summary>
	public Rules.RulesDefinition Rules { get; } = rules;
}
