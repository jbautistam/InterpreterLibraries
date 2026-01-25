namespace Bau.Libraries.LibInterpreter.Interpreter;

/// <summary>
///		Opciones del procesador
/// </summary>
public class ProcessorOptions
{
	/// <summary>
	///		Indica si es necesario declarar las variables antes de utilizarlas
	/// </summary>
	public required bool NeedDeclareVariables { get; init; }
}
