namespace Bau.Libraries.LibInterpreter.Models.Sentences;

/// <summary>
///		Sentencia para declaración de una variable
/// </summary>
public class SentenceDeclare : SentenceBase
{
	/// <summary>
	///		Variable
	/// </summary>
	public Symbols.SymbolModel Variable { get; } = new Symbols.SymbolModel();

	/// <summary>
	///		Expresión (antes de interpretar)
	/// </summary>
	public string ExpressionString { get; set; }

	/// <summary>
	///		Valor de la variable
	/// </summary>
	public Expressions.ExpressionsCollection Expressions { get; } = new Expressions.ExpressionsCollection();
}
