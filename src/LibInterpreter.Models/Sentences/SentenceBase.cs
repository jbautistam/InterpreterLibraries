namespace Bau.Libraries.LibInterpreter.Models.Sentences;

/// <summary>
///		Base para las sentencias
/// </summary>
public abstract class SentenceBase
{
	/// <summary>
	///		Genera la cadena de depuración
	/// </summary>
	public void Debug(System.Text.StringBuilder builder, int indent)
	{
		string debug = GetDebugInfo(indent);

			// Añade la línea de depuración
			if (!string.IsNullOrWhiteSpace(debug))
				builder.AppendLine(debug);
	}

	/// <summary>
	///		Obtiene la información de depuración de la sentencia
	/// </summary>
	public abstract string GetDebugInfo(int indent);

	/// <summary>
	///		Añade datos de depuración a una cadena
	/// </summary>
	protected string AddDebug(int indent, string debug) => $"{new string('\t', indent)} {debug}" + Environment.NewLine;
}