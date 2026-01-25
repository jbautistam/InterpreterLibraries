namespace Bau.Libraries.LibInterpreter.Lexer.Rules;

/// <summary>
///		Definición de reglas
/// </summary>
public class RulesDefinition
{
	/// <summary>
	///		Separadores
	/// </summary>
	public string? Separators { get; set; }

	/// <summary>
	///		Reglas
	/// </summary>
	public RuleCollection Rules { get; } = [];
}
