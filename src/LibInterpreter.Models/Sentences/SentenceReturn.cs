namespace Bau.Libraries.LibInterpreter.Models.Sentences;

/// <summary>
///		Sentencia de retorno de una expresión para una función
/// </summary>
public class SentenceReturn : SentenceBase
{
	/// <summary>
	///		Genera la cadena de depuración
	/// </summary>
	public override string GetDebugInfo(int indent) 
	{
		string debug = AddDebug(indent, $"Return");

			// Añade las expresiones
			debug += Expression.GetDebugInfo(indent + 1);
			// Devuelve la cadena de depuración
			return debug;
	}

	/// <summary>
	///		Expresión
	/// </summary>
	public Expressions.ExpressionsCollection Expression { get; } = [];
}