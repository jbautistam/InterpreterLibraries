using Bau.Libraries.LibInterpreter.Lexer.Rules;

namespace Bau.Libraries.LibInterpreter.Lexer.Builder;

/// <summary>
///		Generador de <see cref="RuleBase"/>
/// </summary>
public class RuleBuilder
{
	/// <summary>
	///		Añade una regla <see cref="RuleDelimited"/> para el mismo separador de inicio y fin
	/// </summary>
	public RuleBuilder WithDelimited(string token, string separator)
	{
		// Añade una regla delimitada
		RulesDefinition.Rules.Add(new RuleDelimited(token, [ separator ], [ separator ], false, false, false, false, false));
		// Devuelve el generador
		return this;
	}

	/// <summary>
	///		Añade una regla <see cref="RuleDelimited"/> para un separador de inicio y otro de fin
	/// </summary>
	public RuleBuilder WithDelimited(string token, string start, string end)
	{
		// Añade una regla delimitada
		RulesDefinition.Rules.Add(new RuleDelimited(token, [ start ], [ end ], false, false, false, false, false));
		// Devuelve el generador
		return this;
	}

	/// <summary>
	///		Añade una regla <see cref="RuleWordFixed"/> indicando si se debe buscar hasta uno de los separadores definidos en <see cref="RuleCollection"/>
	/// </summary>
	public RuleBuilder WithWord(string token, string word, bool toSeparator)
	{
		// Añade una regla asociada a una palabra clave
		RulesDefinition.Rules.Add(new RuleWordFixed(token, word, toSeparator));
		// Devuelve el generador
		return this;
	}

	/// <summary>
	///		Añade una regla <see cref="RuleWordFixed"/> indicando si se debe buscar hasta uno de los separadores definidos en <see cref="RuleCollection"/>
	/// </summary>
	public RuleBuilder WithWords(string token, params string[] words)
	{
		// Añade una regla asociada a una palabra clave
		foreach (string word in words)
			RulesDefinition.Rules.Add(new RuleWordFixed(token, word, false));
		// Devuelve el generador
		return this;
	}

	/// <summary>
	///		Añade una regla <see cref="RulePattern"/> con patrones
	/// </summary>
	public RuleBuilder WithPattern(string token, string patternStart, string patternContent)
	{
		// Añade el patrón
		RulesDefinition.Rules.Add(new RulePattern(token, patternStart, patternContent));
		// Devuelve el generador
		return this;
	}

	/// <summary>
	///		Añade una regla predefinida para números
	/// </summary>
	public RuleBuilder WithDefaultNumbers(string token) => WithPattern(token, "9", "9.");

	/// <summary>
	///		Añade una regla para los operadores matemáticos (paréntesis de apertura y cierre, +, -, *, /, = y %)
	/// </summary>
	public RuleBuilder WithDefaultMathOperators(string token) => WithWords(token, "(", ")", "+", "-", "*", "/", "%", "=");

	/// <summary>
	///		Añade una regla para los operadores relacionales (||, &&, !)
	/// </summary>
	public RuleBuilder WithDefaultRelationalOperators(string token) => WithWords(token, "||", "&&", "!");

	/// <summary>
	///		Añade una regla para los operadores lógicos (<, >, >=, <=, ==, !=)
	/// </summary
	public RuleBuilder WithDefaultLogicalOperators(string token) => WithWords(token, ">=", "<=", "==", "<", ">", "!=");

	/// <summary>
	///		Asigna el separador de expresiones predeterminado (()+=-.><)
	/// </summary>
	public RuleBuilder WithDefaultSeparators() => WithSeparators("()+=-.><");

	/// <summary>
	///		Asigna el separador de expresiones
	/// </summary>
	public RuleBuilder WithSeparators(string separators, bool removeDefault = false)
	{
		// Vacía el separador de expresiones
		if (removeDefault)
			RulesDefinition.Separators = string.Empty;
		// Añade los separadores de expresiones
		RulesDefinition.Separators += separators;
		// Devuelve el generador
		return this;
	}

	/// <summary>
	///		Añade una regla para los patrones desde un inicio a un fin de línea (por ejemplo, ## para comentarios hasta el final de línea)
	/// </summary>
	public RuleBuilder WithToEnd(string token, string start)
	{
		// Añade el patrón
		RulesDefinition.Rules.Add(new RuleDelimited(token, start, string.Empty, true, false, true, false, true));
		// Devuelve el generador
		return this;
	}

	/// <summary>
	///		Genera las reglas
	/// </summary>
	public RulesDefinition Build() => RulesDefinition;

	/// <summary>
	///		Reglas generadas
	/// </summary>
	private RulesDefinition RulesDefinition { get; } = new();
}
