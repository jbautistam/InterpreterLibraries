
namespace Bau.Libraries.LibInterpreter.Models.Sentences;

/// <summary>
///		Colección de sentencias
/// </summary>
public class SentenceCollection : List<SentenceBase>
{
	/// <summary>
	///		Genera la cadena de depuración
	/// </summary>
	public string GetDebugInfo(int indent)
	{
		string debug = string.Empty;

			// Sentencias
			foreach (SentenceBase sentence in this)
				debug += sentence.GetDebugInfo(indent + 1);
			// Devuelve la cadena de depuración
			return debug;
	}

	/// <summary>
	///		Indica si la colección está vacía
	/// </summary>
	public bool Empty => Count == 0;
}
