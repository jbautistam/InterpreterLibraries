namespace Bau.Libraries.LibInterpreter.Models.Sentences;

/// <summary>
///		Sentencia de llamada a una subrutina
/// </summary>
public class SentenceCallFunction : SentenceBase
{
	/// <summary>
	///		Genera la cadena de depuración
	/// </summary>
	public override string GetDebugInfo(int indent)
	{
		string debug = AddDebug(indent, $"Call {Function}");

			// Argumentos
			debug = AddDebug(indent, "Arguments");
			foreach (Expressions.ExpressionsCollection argument in Arguments)
				debug = AddDebug(0, argument.GetDebugInfo(indent + 1));
			// Devuelve la cadena de depuración
			return debug;
	}

	/// <summary>
	///		Nombre de la función a la que se llama
	/// </summary>
	public string Function { get; set; }

	/// <summary>
	///		Parámetros de llamada
	/// </summary>
	public List<Expressions.ExpressionsCollection> Arguments { get; } = [];
}
