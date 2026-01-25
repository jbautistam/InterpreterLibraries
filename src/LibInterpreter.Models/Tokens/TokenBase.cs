namespace Bau.Libraries.LibInterpreter.Models.Tokens;

/// <summary>
///		Clase base con los datos de un token
/// </summary>
public class TokenBase(string type, int row, int column, string value)
{
	/// <summary>
	///		Obtiene la información de depuración del token
	/// </summary>
	public virtual string GetDebugInfo() => $"Type {Type} (R {Row} - C {Column} - I {Indent}) : #{Value}#";

	/// <summary>
	///		Tipo del token
	/// </summary>
	public string Type { get; } = type;

	/// <summary>
	///		Fila
	/// </summary>
	public int Row { get; } = row;

	/// <summary>
	///		Columna
	/// </summary>
	public int Column { get; } = column;

	/// <summary>
	///		Indentación
	/// </summary>
	public int Indent { get; set; }

	/// <summary>
	///		Contenido del token
	/// </summary>
	public string Value { get; set; } = value;
}