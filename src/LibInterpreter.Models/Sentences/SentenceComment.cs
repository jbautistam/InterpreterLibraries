namespace Bau.Libraries.LibInterpreter.Models.Sentences;

/// <summary>
///		Sentencia de un comentario
/// </summary>
public class SentenceComment : SentenceBase
{
	/// <summary>
	///		Genera la cadena de depuración
	/// </summary>
	public override string GetDebugInfo(int indent) => AddDebug(indent, $"Comment {Content}");

	/// <summary>
	///		Contenido de la sentencia
	/// </summary>
	public string? Content { get; set; }
}
