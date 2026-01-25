namespace Bau.Libraries.LibInterpreter.Models.Sentences;

/// <summary>
///		Sentencia para un bucle While
/// </summary>
public class SentenceWhile : SentenceBase
{
	/// <summary>
	///		Genera la cadena de depuración
	/// </summary>
	public override string GetDebugInfo(int indent) 
	{
		string debug = AddDebug(indent, "While");

			// Añade la condición y las sentencias
			debug += Condition.GetDebugInfo(indent + 1);
			debug += Sentences.GetDebugInfo(indent + 1);
			// Devuelve la cadena de depuración
			return debug;
	}

	/// <summary>
	///		Condición
	/// </summary>
	public Expressions.ExpressionsCollection Condition { get; } = [];

	/// <summary>
	///		Sentencias
	/// </summary>
	public SentenceCollection Sentences { get; } = [];
}
