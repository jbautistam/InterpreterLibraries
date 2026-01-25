namespace Bau.Libraries.LibInterpreter.Models.Sentences;

/// <summary>
///		Sentencia de declaración de una función
/// </summary>
public class SentenceFunction : SentenceBase
{
	/// <summary>
	///		Genera la cadena de depuración
	/// </summary>
	public override string GetDebugInfo(int indent) 
	{
		string debug = AddDebug(indent, $"Function {Definition.GetDebugInfo()}");

			// Añade los argumentos de la función
			debug += AddDebug(indent + 1, "Arguments");
			foreach (Symbols.SymbolModel argument in Arguments)
				debug += AddDebug(indent + 2, argument.GetDebugInfo());
			// Añade las sentencias
			debug += Sentences.GetDebugInfo(indent + 1);
			// Devuelve la cadena de depuración
			return debug;
	}

	/// <summary>
	///		Nombre y tipo de la función
	/// </summary>
	public Symbols.SymbolModel Definition { get; set; } = new();

	/// <summary>
	///		Argumentos de la función
	/// </summary>
	public List<Symbols.SymbolModel> Arguments { get; } = [];

	/// <summary>
	///		Contenido de la función
	/// </summary>
	public SentenceCollection Sentences { get; } = [];
}
