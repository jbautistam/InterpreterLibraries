namespace Bau.Libraries.LibInterpreter.Models.Expressions;

/// <summary>
///		Expresión para un operador lógico
/// </summary>
public class ExpressionOperatorLogical(ExpressionOperatorLogical.LogicalType type) : ExpressionOperatorBase
{
	/// <summary>
	///		Tipo de operación
	/// </summary>
	public enum LogicalType
	{
		/// <summary>Igual</summary>
		Equal,
		/// <summary>Distinto</summary>
		Distinct,
		/// <summary>Mayor</summary>
		Greater,
		/// <summary>Menor</summary>
		Less,
		/// <summary>Mayor o igual</summary>
		GreaterOrEqual,
		/// <summary>Menor o igual</summary>
		LessOrEqual
	}

	/// <summary>
	///		Clona la expresión
	/// </summary>
	public override ExpressionBase Clone() => new ExpressionOperatorLogical(Type);

	/// <summary>
	///		Obtiene la información de depuración
	/// </summary>
	public override string GetDebugInfo() => $"[{Type.ToString()}]";

	/// <summary>
	///		Tipo de operación
	/// </summary>
	public LogicalType Type { get; } = type;

	/// <summary>
	///		Obtiene la prioridad de la operación
	/// </summary>
	public override int Priority
	{
		get
		{
			return Type switch
					{
						LogicalType.Greater or LogicalType.GreaterOrEqual or LogicalType.Less or LogicalType.LessOrEqual => 18,
						LogicalType.Equal or LogicalType.Distinct => 17,
						_ => 0,
					};
		}
	}
}