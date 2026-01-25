namespace Bau.Libraries.LibInterpreter.Models.Expressions;

/// <summary>
///		Expresión con los datos de una constante
/// </summary>
public class ExpressionConstant(Symbols.SymbolModel.SymbolType type, object value) : ExpressionBase
{
	/// <summary>
	///		Clona la expresión
	/// </summary>
	public override ExpressionBase Clone() => new ExpressionConstant(Type, Value);

	/// <summary>
	///		Obtiene la información de depuración
	/// </summary>
	public override string GetDebugInfo() => $"[{Type.ToString()} - {Value}]";

	/// <summary>
	///		Tipo de la constante
	/// </summary>
	public Symbols.SymbolModel.SymbolType Type { get; } = type;

	/// <summary>
	///		Valor de la constante
	/// </summary>
	public object Value { get; } = value;
}
