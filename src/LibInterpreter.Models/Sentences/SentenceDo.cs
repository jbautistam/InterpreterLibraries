namespace Bau.Libraries.LibInterpreter.Models.Sentences;

/// <summary>
///		Sentencia para un bucle do ... while
/// </summary>
public class SentenceDo : SentenceBase
{
	/// <summary>
	///		Genera la cadena de depuración
	/// </summary>
	public override string GetDebugInfo(int indent)
	{
		string debug = AddDebug(indent, $"While");

			// Sentencias
			debug += AddDebug(indent, Condition.GetDebugInfo(indent));
			debug += AddDebug(indent, Sentences.GetDebugInfo(indent + 1));
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
