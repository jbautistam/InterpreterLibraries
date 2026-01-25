namespace Bau.Libraries.LibInterpreter.Models.Expressions;

/// <summary>
///		Expresión para un operador relacional
/// </summary>
public class ExpressionOperatorRelational(ExpressionOperatorRelational.RelationalType type) : ExpressionOperatorBase
{
	/// <summary>
	///		Tipo de operación
	/// </summary>
	public enum RelationalType
	{
		/// <summary>And</summary>
		And,
		/// <summary>Or</summary>
		Or,
		/// <summary>Not</summary>
		Not
	}

	/// <summary>
	///		Clona la expresión
	/// </summary>
	public override ExpressionBase Clone() => new ExpressionOperatorRelational(Type);

	/// <summary>
	///		Obtiene la información de depuración
	/// </summary>
	public override string GetDebugInfo() => $"[{Type.ToString()}]";

	/// <summary>
	///		Tipo de operación
	/// </summary>
	public RelationalType Type { get; } = type;

	/// <summary>
	///		Obtiene la prioridad de la operación
	/// </summary>
	public override int Priority
	{
		get
		{
			return Type switch
					{
						RelationalType.And => 16,
						RelationalType.Or => 15,
						_ => 0,
					};
		}
	}
}