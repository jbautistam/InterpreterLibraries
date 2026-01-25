namespace Bau.Libraries.LibInterpreter.Models.Sentences;

/// <summary>
///		Sentencia de ejecución de un bucle 
/// </summary>
public class SentenceFor : SentenceBase
{
	/// <summary>
	///		Genera la cadena de depuración
	/// </summary>
	public override string GetDebugInfo(int indent) 
	{
		string debug = AddDebug(indent, $"For {Variable.GetDebugInfo()}");

			// Añade las expresiones de inicio y fin
			debug += AddDebug(indent + 1, "Start");
			debug += StartExpression.GetDebugInfo(indent + 1);
			debug += AddDebug(indent + 1, "End");
			debug += EndExpression.GetDebugInfo(indent + 1);
			debug += AddDebug(indent + 1, "Step");
			debug += StepExpression.GetDebugInfo(indent + 1);
			// Añade las sentencias
			debug += Sentences.GetDebugInfo(indent + 1);
			// Devuelve la cadena de depuración
			return debug;
	}

	/// <summary>
	///		Nombre de variable
	/// </summary>
	public Symbols.SymbolModel Variable { get; } = new();

	/// <summary>
	///		Expresión para el valor de inicio
	/// </summary>
	public Expressions.ExpressionsCollection StartExpression { get; set; } = [];

	/// <summary>
	///		Expresión para el valor de fin
	/// </summary>
	public Expressions.ExpressionsCollection EndExpression { get; set; } = [];

	/// <summary>
	///		Expresión para el valor del paso
	/// </summary>
	public Expressions.ExpressionsCollection StepExpression { get; set; } = [];

	/// <summary>
	///		Sentencias a ejecutar
	/// </summary>
	public SentenceCollection Sentences { get; } = [];
}
