namespace Bau.Libraries.LibInterpreter.Models.Sentences;

/// <summary>
///		Sentencia de ejecución de una condición
/// </summary>
public class SentenceIf : SentenceBase
{
	/// <summary>
	///		Genera la cadena de depuración
	/// </summary>
	public override string GetDebugInfo(int indent) 
	{
		string debug = AddDebug(indent, "If");

			// Añade las condiciones
			debug += AddDebug(indent + 1, ConditionExpression.GetDebugInfo(indent + 1));
			// Añade las sentencias then / else
			debug += AddDebug(indent + 1, "Then");
			debug += SentencesThen.GetDebugInfo(indent + 2);
			debug += AddDebug(indent + 1, "Else");
			debug += SentencesElse.GetDebugInfo(indent + 2);
			// Devuelve la cadena de depuración
			return debug;
	}

	/// <summary>
	///		Condición
	/// </summary>
	public Expressions.ExpressionsCollection ConditionExpression { get; set; } = [];

	/// <summary>
	///		Sentencias a ejecutar si el resultado de la condición es verdadero
	/// </summary>
	public SentenceCollection SentencesThen { get; } = [];

	/// <summary>
	///		Sentencias a ejecutar si el resultado de la condición es false
	/// </summary>
	public SentenceCollection SentencesElse { get; } = [];
}
