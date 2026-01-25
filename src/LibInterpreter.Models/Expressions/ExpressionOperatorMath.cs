namespace Bau.Libraries.LibInterpreter.Models.Expressions;

/// <summary>
///		Expresión para un operador matemático
/// </summary>
public class ExpressionOperatorMath(ExpressionOperatorMath.MathType type) : ExpressionOperatorBase
{
	/// <summary>
	///		Tipo de operación
	/// </summary>
	public enum MathType
	{
		/// <summary>Suma</summary>
		Sum,
		/// <summary>Resta</summary>
		Substract,
		/// <summary>Multiplicación</summary>
		Multiply,
		/// <summary>División</summary>
		Divide,
		/// <summary>Módulo</summary>
		Modulus
	}

	/// <summary>
	///		Clona la expresión
	/// </summary>
	public override ExpressionBase Clone() => new ExpressionOperatorMath(Type);

	/// <summary>
	///		Obtiene la información de depuración
	/// </summary>
	public override string GetDebugInfo() => $"[{Type.ToString()}]";

	/// <summary>
	///		Tipo de operación
	/// </summary>
	public MathType Type { get; } = type;

	/// <summary>
	///		Obtiene la prioridad de la operación
	/// </summary>
	public override int Priority
	{
		get
		{
			return Type switch
					{
						MathType.Multiply or MathType.Divide or MathType.Modulus => 20,
						MathType.Sum or MathType.Substract => 19,
						_ => 0,
					};
		}
	}
}
