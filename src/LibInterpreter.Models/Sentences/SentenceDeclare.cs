namespace Bau.Libraries.LibInterpreter.Models.Sentences;

/// <summary>
///		Sentencia para declaración de una variable
/// </summary>
public class SentenceDeclare : SentenceBase
{
	/// <summary>
	///		Genera la cadena de depuración
	/// </summary>
	public override string GetDebugInfo(int indent) 
	{
		string debug = AddDebug(indent, $"Declare {Variable.GetDebugInfo()}");

			// Añade la expresión
			debug += Expressions.GetDebugInfo(indent + 1);
			// Devuelve la cadena de depuración
			return debug;
	}

	/// <summary>
	///		Variable
	/// </summary>
	public Symbols.SymbolModel Variable { get; } = new();

	/// <summary>
	///		Valor de la variable
	/// </summary>
	public Expressions.ExpressionsCollection Expressions { get; } = [];
}
