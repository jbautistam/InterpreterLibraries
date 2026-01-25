namespace Bau.Libraries.LibInterpreter.Models.Expressions;

/// <summary>
///		Expresión de tipo variable
/// </summary>
public class ExpressionVariableIdentifier(string name) : ExpressionBase
{
	/// <summary>
	///		Clona el identificador de variable
	/// </summary>
	public override ExpressionBase Clone()
	{
		ExpressionVariableIdentifier variable = new(Name);

			// Clona las expresiones
			variable.IndexExpressions = IndexExpressions.Clone();
			if (Member is not null)
				variable.Member = Member.Clone() as ExpressionVariableIdentifier;
			// Devuelve el objeto clonado
			return variable;
	}

	/// <summary>
	///		Obtiene la información de depuración
	/// </summary>
	public override string GetDebugInfo()
	{
		string debug = Name;

			// Añade el índice
			if (IndexExpressions?.Count > 0)
				debug += "[" + IndexExpressions.GetDebugInfo(0) + "]";
			// Añade el miembro
			if (Member != null)
				debug += "->" + Member.GetDebugInfo();
			// Devuelve la información de depuración
			return debug;
	}

	/// <summary>
	///		Nombre de la variable
	/// </summary>
	public string Name { get; } = name;

	/// <summary>
	///		Expresiones de índice
	/// </summary>
	public ExpressionsCollection IndexExpressions { get; set; } = [];

	/// <summary>
	///		Identificador de variable
	/// </summary>
	public ExpressionVariableIdentifier? Member { get; set; }
}
