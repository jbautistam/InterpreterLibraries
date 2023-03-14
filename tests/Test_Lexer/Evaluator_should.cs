using Xunit;
using FluentAssertions;

using Bau.Libraries.LibInterpreter.Evaluator;
using Bau.Libraries.LibInterpreter.Models.Expressions;
using Bau.Libraries.LibInterpreter.Models.Symbols;

namespace Test_Lexer;

/// <summary>
///		Pruebas de <see cref="ExpressionConversorRpn"/>
/// </summary>
public class Evaluator_should
{
	/// <summary>
	///		Comprueba si se genera la pila de expresiones
	/// </summary>
	[Fact]
	public void generate_expressions_stack()
	{
		ExpressionsCollection result = new ExpressionConversorRpn().ConvertToRPN(GetExpressions());

			// Comprueba los datos
			result.Count.Should().BeGreaterThan(0);
			(result[0] as ExpressionConstant)?.Value.Should().Be(20);
			(result[1] as ExpressionConstant)?.Value.Should().Be(2);
			(result[2] as ExpressionOperatorMath)?.Type.Should().Be(ExpressionOperatorMath.MathType.Substract);
			(result[3] as ExpressionConstant)?.Value.Should().Be(5);
			(result[4] as ExpressionOperatorMath)?.Type.Should().Be(ExpressionOperatorMath.MathType.Multiply);
	}

	/// <summary>
	///		Obtiene una lista de expresiones
	/// </summary>
	private ExpressionsCollection GetExpressions()
	{
		ExpressionsCollection expressions = new();

			// Añade las expresiones
			expressions.Add(new ExpressionParenthesis(true));
			expressions.Add(new ExpressionConstant(SymbolModel.SymbolType.Numeric, 20));
			expressions.Add(new ExpressionOperatorMath(ExpressionOperatorMath.MathType.Substract));
			expressions.Add(new ExpressionConstant(SymbolModel.SymbolType.Numeric, 2));
			expressions.Add(new ExpressionParenthesis(false));
			expressions.Add(new ExpressionOperatorMath(ExpressionOperatorMath.MathType.Multiply));
			expressions.Add(new ExpressionConstant(SymbolModel.SymbolType.Numeric, 5));
			// Devuelve las expresiones generadas
			return expressions;
	}
}
