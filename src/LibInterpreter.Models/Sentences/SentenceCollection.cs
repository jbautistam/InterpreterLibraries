namespace Bau.Libraries.LibInterpreter.Models.Sentences;

/// <summary>
///		Colección de sentencias
/// </summary>
public class SentenceCollection : List<SentenceBase>
{
	/// <summary>
	///		Indica si la colección está vacía
	/// </summary>
	public bool Empty => Count == 0;
}
