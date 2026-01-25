namespace Bau.Libraries.LibInterpreter.Models.Expressions;

/// <summary>
///		Expresión que identifica un paréntesis de apertura o cierre
/// </summary>
public class ExpressionParenthesis(bool open) : ExpressionBase
{
	/// <summary>
	///		Clona la expresión
	/// </summary>
	public override ExpressionBase Clone() => new ExpressionParenthesis(Open);

	/// <summary>
	///		Obtiene la información de depuración
	/// </summary>
	public override string GetDebugInfo() => $"[{(Open ? '(' : ')')}]";

	/// <summary>
	///		Indica si es un paréntesis de apertura
	/// </summary>
	public bool Open { get; } = open;
}
