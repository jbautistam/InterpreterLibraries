using Bau.Libraries.LibInterpreter.Models.Expressions;

namespace Bau.Libraries.LibInterpreter.Evaluator;

/// <summary>
///		Clase para el cálculo de expresiones
/// </summary>
public abstract class ExpressionEvaluatorBase
{
	/// <summary>
	///		Calcula el resultado de una expresión
	/// </summary>
	public ExpressionBase? Compute(ExpressionsCollection stackExpressionsRpn)
	{
		Stack<ExpressionBase> stackOperators = new();

			// Calcula el resultado
			foreach (ExpressionBase expressionBase in stackExpressionsRpn)
				switch (expressionBase)
				{
					case ExpressionConstant expression:
							stackOperators.Push(expression);
						break;
					case ExpressionVariableIdentifier expression:
							stackOperators.Push(ComputeOperation(expression, null, null));
						break;
					case ExpressionFunction expression:
							stackOperators.Push(ComputeOperation(expression, null, null));
						break;
					case ExpressionOperatorBase expression:
							if (stackOperators.Count < 2)
							{
								// Evalúa una expresión de relación (con un único operador [not, por ejemplo o +/- en caso de
								// una operación matemática])
								if (expression is ExpressionOperatorRelational || expression is ExpressionOperatorMath ||
									expression is ExpressionOperatorLogical)
								{
									ExpressionBase first = stackOperators.Pop();

										// Evalúa una expresión con un único operador
										stackOperators.Push(ComputeOperation(first, expression, null));
								}
								else
									throw new Models.Exceptions.InterpreterException("There is not enough operators in stack for execute this operation");
							}
							else
							{
								ExpressionBase second = stackOperators.Pop(); //? cuidado al sacar de la pila, están al revés (y es importante para las operaciones no conmutativas)
								ExpressionBase first = stackOperators.Pop();

									// Evalúa una expresión con dos operadores
									stackOperators.Push(ComputeOperation(first, expression, second));
							}
						break;
				}
			// Obtiene el resultado
			if (stackOperators.Count == 1)
				return stackOperators.Pop();
			else if (stackOperators.Count == 0)
				throw new Models.Exceptions.InterpreterException("There is no operators in the operations stack"); 
			else
				throw new Models.Exceptions.InterpreterException("There are too much operator in the operations stack");
	}

	/// <summary>
	///		Rutina que realiza el cálculo
	/// </summary>
	protected abstract ExpressionBase ComputeOperation(ExpressionBase operandFirst, ExpressionBase? operation, ExpressionBase? operandSecond);
}