namespace Bau.Libraries.LibInterpreter.Models.Sentences;

/// <summary>
///		Sentencia para imprimir un mensaje
/// </summary>
public class SentencePrint : SentenceBase
{
	/// <summary>
	///		Genera la cadena de depuración
	/// </summary>
	public override string GetDebugInfo(int indent) => AddDebug(indent, $"Print {Message}");

	/// <summary>
	///		Mensaje
	/// </summary>
	public string? Message { get; set; }
}
