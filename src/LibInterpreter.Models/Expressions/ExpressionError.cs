namespace Bau.Libraries.LibInterpreter.Models.Expressions;

/// <summary>
///		Expresión indicando un error
/// </summary>
public class ExpressionError(string message) : ExpressionBase
{
	/// <summary>
	///		Clona la expresión
	/// </summary>
	public override ExpressionBase Clone() => new ExpressionError(Message);

	/// <summary>
	///		Obtiene la información de depuración
	/// </summary>
	public override string GetDebugInfo() => $"Error: {Message}]";

	/// <summary>
	///		Mensaje de error
	/// </summary>
	public string Message { get; } = message;
}
