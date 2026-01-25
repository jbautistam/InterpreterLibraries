namespace Bau.Libraries.LibInterpreter.Models.Sentences;

/// <summary>
///		Clase con los datos de una excepción
/// </summary>
public class SentenceException : SentenceBase
{
	/// <summary>
	///		Genera la cadena de depuración
	/// </summary>
	public override string GetDebugInfo(int indent) => AddDebug(indent, $"Exception: {Message}");

	/// <summary>
	///		Mensaje de error
	/// </summary>
	public string? Message { get; set; }
}
