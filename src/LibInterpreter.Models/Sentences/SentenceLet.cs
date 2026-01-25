namespace Bau.Libraries.LibInterpreter.Models.Sentences;

/// <summary>
///		Sentencia para ejecución de una expresión
/// </summary>
public class SentenceLet : SentenceBase
{
	/// <summary>
	///		Genera la cadena de depuración
	/// </summary>
	public override string GetDebugInfo(int indent)
	{
		string debug = AddDebug(indent, $"Let {Variable} (Type: {Type.ToString()})");

			// Añade las condiciones
			debug += AddDebug(indent + 1, Expressions.GetDebugInfo(indent + 1));
			// Devuelve la cadena de depuración
			return debug;
	}

	/// <summary>
	///		Tipo de la variable de salida: sólo es necesario cuando no está definida
	/// </summary>
	public Symbols.SymbolModel.SymbolType Type { get; set; }

	/// <summary>
	///		Nombre de variable
	/// </summary>
	public string Variable { get; set; } = default!;

	/// <summary>
	///		Expresiones a ejecutar
	/// </summary>
	public Expressions.ExpressionsCollection Expressions { get; set; } = [];
}