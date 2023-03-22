using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Bau.Libraries.LibInterpreter.Interpreter.Context;
using Bau.Libraries.LibInterpreter.Interpreter.Context.Variables;
using Bau.Libraries.LibInterpreter.Models.Expressions;
using Bau.Libraries.LibInterpreter.Models.Symbols;

namespace Bau.Libraries.LibInterpreter.Interpreter.Evaluator;

/// <summary>
///		Clase para el cálculo de expresiones
/// </summary>
internal class ExpressionCompute
{
	/// <summary>
	///		Enumerado con el tipo de operación que se debe hacer sobre una fecha
	/// </summary>
	private enum DateOperation
	{
		/// <summary>Añadir / restar días</summary>
		Day,
		/// <summary>Añadir / restar meses</summary>
		Month,
		/// <summary>Añadir / restar semanas</summary>
		Week,
		/// <summary>Añadir / restar años</summary>
		Year,
		/// <summary>Añadir / restar horas</summary>
		Hour,
		/// <summary>Añadir / restar minutos</summary>
		Minutes,
		/// <summary>Añadir / restar segundos</summary>
		Seconds
	}

	internal ExpressionCompute(ProgramProcessor processor)
	{
		Processor = processor;
	}

	/// <summary>
	///		Evalúa una serie de expresiones
	/// </summary>
	internal async Task<(string error, VariableModel? variable)> EvaluateAsync(ContextModel context, ExpressionsCollection expressions, 
																			   CancellationToken cancellationToken)
	{
		return await ComputeAsync(context, new ExpressionConversorRpn().ConvertToRPN(expressions), cancellationToken);
	}

	/// <summary>
	///		Calcula una expresión
	/// </summary>
	private async Task<(string error, VariableModel? variable)> ComputeAsync(ContextModel context, ExpressionsCollection stackExpressions, 
																			 CancellationToken cancellationToken)
	{
		string error = string.Empty;
		Stack<VariableModel> stackOperators = new Stack<VariableModel>();

			// Calcula el resultado
			foreach (ExpressionBase expressionBase in stackExpressions)
				if (string.IsNullOrWhiteSpace(error))
					switch (expressionBase)
					{
						case ExpressionConstant expression:
								stackOperators.Push(new VariableModel("Constant", expression.Type, expression.Value));
							break;
						case ExpressionVariableIdentifier expression:
								(string errorSearch, VariableModel variable) = await SearchAsync(context, expression, cancellationToken);

									// Comprueba que se haya encontrado la variable y la añade a la pila
									if (!string.IsNullOrWhiteSpace(errorSearch))
										error = errorSearch;
									else if (variable is null)
										error = $"Cant find the variable '{expression.Name}' value";
									else
										stackOperators.Push(variable);
							break;
						case ExpressionFunction expression:
								VariableModel? resultFunction = await Processor.ExecuteFunctionAsync(expression, cancellationToken);

									// Si se ha podido ejecutar la función, la añade a la pila
									if (resultFunction is null)
										error = $"Cant execute function {expression.Function}";
									else
										stackOperators.Push(resultFunction);
							break;
						case ExpressionOperatorBase expression:
								if (stackOperators.Count < 2)
								{
									if (expression is ExpressionOperatorMath expressionMath)
									{
										VariableModel first = stackOperators.Pop();
										VariableModel result = ComputeUnary(expressionMath, first, out error);

											// Si no ha habido ningún error se añade a la pila
											if (string.IsNullOrWhiteSpace(error))
												stackOperators.Push(result);
									}
									else if (expression is ExpressionOperatorLogical expressionLogical)
									{
										VariableModel first = stackOperators.Pop();
										VariableModel result = ComputeUnary(expressionLogical, first, out error);

											// Si no ha habido ningún error se añade a la pila
											if (string.IsNullOrWhiteSpace(error))
												stackOperators.Push(result);
									}
									else
										error = "There is not enough operators in stack for execute this operation";
								}
								else
								{
									VariableModel second = stackOperators.Pop(); //? cuidado al sacar de la pila, están al revés
									VariableModel first = stackOperators.Pop();
									VariableModel result = ComputeBinary(expression, first, second, out error);

										// Si no ha habido ningún error, se añade a la pila
										if (string.IsNullOrEmpty(error))
											stackOperators.Push(result);
								}
							break;
					}
			// Obtiene el resultado
			if (!string.IsNullOrWhiteSpace(error))
				return (error, null);
			else if (string.IsNullOrWhiteSpace(error) && stackOperators.Count == 1)
				return (error, stackOperators.Pop());
			else if (stackOperators.Count == 0)
				return ("There is no operators in the operations stack", null); 
			else
				return ("There are too much operator in the operations stack", null);
	}

	/// <summary>
	///		Busca recursivamente el valor de una variable
	/// </summary>
	private async Task<(string error, VariableModel? variable)> SearchAsync(ContextModel context, ExpressionVariableIdentifier expressionVariable, 
																		    CancellationToken cancellationToken)
	{
		string error = string.Empty;
		VariableModel? variable = null;
		int index = 0;

			// Obtiene el índice asociado a la variable
			if (expressionVariable.IndexExpressions.Count > 0)
			{
				(string errorCompute, VariableModel? indexVariable) = await ComputeAsync(context, expressionVariable.IndexExpressions, cancellationToken);

					if (!string.IsNullOrWhiteSpace(errorCompute))
						error = errorCompute;
					else if (indexVariable.Type != SymbolModel.SymbolType.Numeric)
						error = "Index expression is not a numeric value";
					else
						index = (int) indexVariable.Value;
			}
			// Si no hay ningún error, obtiene la variable
			if (string.IsNullOrWhiteSpace(error))
				variable = context.VariablesTable.Get(expressionVariable.Name, index);
			// Devuelve la variable
			return (error, variable);
	}

	/// <summary>
	///		Calcula una operación con dos valores
	/// </summary>
	private VariableModel? ComputeBinary(ExpressionOperatorBase expression, VariableModel first, VariableModel second, out string error)
	{
		switch (first.Type)
		{
			case SymbolModel.SymbolType.Boolean:
				return ComputeBoolean(expression, first, second, out error);
			case SymbolModel.SymbolType.String:
				return ComputeString(expression, first, second, out error);
			case SymbolModel.SymbolType.Date:
				return ComputeDate(expression, first, second, out error);
			case SymbolModel.SymbolType.Numeric:
				return ComputeNumeric(expression, first, second, out error);
			default:
				error = "Unknow type";
				return null;
		}
	}

	/// <summary>
	///		Calcula una expresión lógica unaria: en prinicipio, sólo not
	/// </summary>
	private VariableModel ComputeUnary(ExpressionOperatorLogical expressionLogical, VariableModel first, out string error)
	{
		throw new NotImplementedException();
	}

	/// <summary>
	///		Calcula una expresión matemática unaria: en prinicipio, sólo + ó - tomados como positivo / negativo
	/// </summary>
	private VariableModel ComputeUnary(ExpressionOperatorMath expression, VariableModel first, out string error)
	{
		VariableModel result = new VariableModel($"Variable_{Guid.NewGuid().ToString()}", SymbolModel.SymbolType.Numeric, null);

			// Inicializa el argumento de salida
			error = string.Empty;
			// Sólo se evalúan las expresiones + ó menos
			if (expression.Type == ExpressionOperatorMath.MathType.Sum || expression.Type == ExpressionOperatorMath.MathType.Substract)
			{
				if (first.Type == SymbolModel.SymbolType.Numeric)
				{
					double value = (double?) first.Value ?? 0;

						// Cambia el signo del valor
						if (expression.Type == ExpressionOperatorMath.MathType.Substract)
							value = -1 * value;
						// Asigna el resultado
						result.Value = value;
				}
				else
					error = $"Expression value is not numeric: {first.GetStringValue()}. Operator {expression.Type.ToString()}";
			}
			else
				error = $"Unary operator unknown: {expression.Type.ToString()}";
			// Devuelve el reultado
			return result;
	}

	/// <summary>
	///		Calcula una operación numérica
	/// </summary>
	private VariableModel? ComputeNumeric(ExpressionOperatorBase expression, VariableModel first, VariableModel second, out string error)
	{
		// Inicializa los argumentos de salida
		error = string.Empty;
		// Si el segundo valor es una cadena, convierte y procesa con cadenas
		switch (second.Type)
		{
			case SymbolModel.SymbolType.String:
				return ComputeString(expression, new VariableModel("Converted", SymbolModel.SymbolType.String, first.Value.ToString()), second, out error);
			case SymbolModel.SymbolType.Date:
				return ComputeDate(expression, second, first, out error);
			case SymbolModel.SymbolType.Numeric:
					double firstValue = (double?) first.Value ?? 0;
					double secondValue = (double?) second.Value ?? 0;

						switch (expression)
						{
							case ExpressionOperatorMath operation:
									switch (operation.Type)
									{
										case ExpressionOperatorMath.MathType.Sum:
											return new VariableModel("Result", SymbolModel.SymbolType.Numeric, firstValue + secondValue);
										case ExpressionOperatorMath.MathType.Substract:
											return new VariableModel("Result", SymbolModel.SymbolType.Numeric, firstValue - secondValue);
										case ExpressionOperatorMath.MathType.Multiply:
											return new VariableModel("Result", SymbolModel.SymbolType.Numeric, firstValue * secondValue);
										case ExpressionOperatorMath.MathType.Divide:
												if (secondValue == 0)
													error = "Cant divide by zero";
												else
													return new VariableModel("Result", SymbolModel.SymbolType.Numeric, firstValue / secondValue);
											break;
										case ExpressionOperatorMath.MathType.Modulus:
											if (secondValue == 0)
												error = "Cant compute a module by zero";
											else
												return new VariableModel("Result", SymbolModel.SymbolType.Numeric, firstValue % secondValue);
											break;
									}
								break;
							case ExpressionOperatorLogical operation:
									switch (operation.Type)
									{
										case ExpressionOperatorLogical.LogicalType.Distinct:
											return new VariableModel("Result", SymbolModel.SymbolType.Boolean, firstValue != secondValue);
										case ExpressionOperatorLogical.LogicalType.Equal:
											return new VariableModel("Result", SymbolModel.SymbolType.Boolean, firstValue == secondValue);
										case ExpressionOperatorLogical.LogicalType.Greater:
											return new VariableModel("Result", SymbolModel.SymbolType.Boolean, firstValue > secondValue);
										case ExpressionOperatorLogical.LogicalType.GreaterOrEqual:
											return new VariableModel("Result", SymbolModel.SymbolType.Boolean, firstValue >= secondValue);
										case ExpressionOperatorLogical.LogicalType.Less:
											return new VariableModel("Result", SymbolModel.SymbolType.Boolean, firstValue < secondValue);
										case ExpressionOperatorLogical.LogicalType.LessOrEqual:
											return new VariableModel("Result", SymbolModel.SymbolType.Boolean, firstValue <= secondValue);
									}
								break;
						}
				break;
		}
		// Si ha llegado hasta aquí es porque no se ha podido evaluar la operación
		if (string.IsNullOrEmpty(error))
			error = "Cant execute this operation with a numeric value";
		return null;
	}

	/// <summary>
	///		Calcula una operación de fecha
	/// </summary>
	private VariableModel? ComputeDate(ExpressionOperatorBase expression, VariableModel first, VariableModel second, out string error)
	{
		DateTime firstValue = (DateTime) first.Value;

			// Inicializa los valores de salida
			error = string.Empty;
			// Dependiendo del tipo del segundo valor
			switch (second.Type)
			{
				case SymbolModel.SymbolType.Numeric:
						if (expression is ExpressionOperatorMath operation)
						{
							int interval = (int) second.Value;

								switch (operation.Type)
								{
									case ExpressionOperatorMath.MathType.Sum:
										return new VariableModel("Result", SymbolModel.SymbolType.Date, ComputeDate(firstValue, true, interval, DateOperation.Day));
									case ExpressionOperatorMath.MathType.Substract:
										return new VariableModel("Result", SymbolModel.SymbolType.Date, ComputeDate(firstValue, false, interval, DateOperation.Day));
								}
						}
					break;
				case SymbolModel.SymbolType.String:
						if (expression is ExpressionOperatorMath operationString)
						{
							(int interval, DateOperation dateOperation) = GetDateIncrement(second.Value?.ToString());

								switch (operationString.Type)
								{
									case ExpressionOperatorMath.MathType.Sum:
										return new VariableModel("Result", SymbolModel.SymbolType.Date, ComputeDate(firstValue, true, interval, dateOperation));
									case ExpressionOperatorMath.MathType.Substract:
										return new VariableModel("Result", SymbolModel.SymbolType.Date, ComputeDate(firstValue, false, interval, dateOperation));
								}
						}
					break;
				case SymbolModel.SymbolType.Date:
						if (expression is ExpressionOperatorLogical logical)
						{
							DateTime secondValue = (DateTime) second.Value;

								switch (logical.Type)
								{
									case ExpressionOperatorLogical.LogicalType.Distinct:
										return new VariableModel("Result", SymbolModel.SymbolType.Boolean, firstValue != secondValue);
									case ExpressionOperatorLogical.LogicalType.Equal:
										return new VariableModel("Result", SymbolModel.SymbolType.Boolean, firstValue == secondValue);
									case ExpressionOperatorLogical.LogicalType.Greater:
										return new VariableModel("Result", SymbolModel.SymbolType.Boolean, firstValue > secondValue);
									case ExpressionOperatorLogical.LogicalType.GreaterOrEqual:
										return new VariableModel("Result", SymbolModel.SymbolType.Boolean, firstValue >= secondValue);
									case ExpressionOperatorLogical.LogicalType.Less:
										return new VariableModel("Result", SymbolModel.SymbolType.Boolean, firstValue < secondValue);
									case ExpressionOperatorLogical.LogicalType.LessOrEqual:
										return new VariableModel("Result", SymbolModel.SymbolType.Boolean, firstValue <= secondValue);
								}
						}
					break;
			}
			// Si ha llegado hasta aquí es porque no se ha podido evaluar la operación
			error = "Can execute this operation with a date";
			return null;
	}

	/// <summary>
	///		Obtiene los valores de un incremento para una fecha
	/// </summary>
	private (int increment, DateOperation type) GetDateIncrement(string value)
	{
		DateOperation type = DateOperation.Day;

			// Obtiene el tipo de incremento
			value = value.ToUpper();
			if (value.EndsWith("W"))
				type = DateOperation.Week;
			else if (value.EndsWith("M"))
				type = DateOperation.Month;
			else if (value.EndsWith("Y"))
				type = DateOperation.Year;
			else if (value.EndsWith("H"))
				type = DateOperation.Hour;
			else if (value.EndsWith("N"))
				type = DateOperation.Minutes;
			else if (value.EndsWith("S"))
				type = DateOperation.Seconds;
			// Quita el tipo de incremento
			if (value.EndsWith("D") || value.EndsWith("W") || value.EndsWith("M") || value.EndsWith("Y") || value.EndsWith("H") ||
				value.EndsWith("N") || value.EndsWith("S"))
			{
				if (value.Length > 1)
					value = value.Substring(0, value.Length - 1);
				else
					value = "1";
			}
			// Obtiene el incremento
			if (!int.TryParse(value, out int result))
				throw new NotImplementedException("The increment has no value");
			else
				return (result, type);
	}

	/// <summary>
	///		Calcula una operación sobre una fecha
	/// </summary>
	private DateTime ComputeDate(DateTime date, bool sum, int interval, DateOperation operation)
	{
		DateTime result = date;

			// Cambia el signo de la operación
			if (!sum)
				interval = -1 * interval;
			// Añade el intervalo a la fecha
			switch (operation)
			{
				case DateOperation.Day:
						result = result.AddDays(interval);
					break;
				case DateOperation.Week:
						result = result.AddDays(7 * interval);
					break;
				case DateOperation.Month:
						result = result.AddMonths(interval);
					break;
				case DateOperation.Year:
						result = result.AddYears(interval);
					break;
				case DateOperation.Hour:
						result = result.AddHours(interval);
					break;
				case DateOperation.Minutes:
						result = result.AddMinutes(interval);
					break;
				case DateOperation.Seconds:
						result = result.AddSeconds(interval);
					break;
			}
			// Devuelve la fecha final
			return result;
	}

	/// <summary>
	///		Calcula una operación de cadena
	/// </summary>
	private VariableModel? ComputeString(ExpressionOperatorBase expression, VariableModel first, VariableModel second, out string error)
	{
		string firstValue = first.Value?.ToString() ?? string.Empty;
		string secondValue = second.Value?.ToString() ?? string.Empty;

			// Inicializa los argumentos de salida
			error = string.Empty;
			// Ejecuta la operación
			switch (expression)
			{
				case ExpressionOperatorMath operation:
						if (operation.Type == ExpressionOperatorMath.MathType.Sum)
							return new VariableModel("Result", SymbolModel.SymbolType.String, firstValue + secondValue);
					break;
				case ExpressionOperatorLogical operation:
						int compare = NormalizeString(firstValue).CompareTo(NormalizeString(secondValue));

							switch (operation.Type)
							{
								case ExpressionOperatorLogical.LogicalType.Distinct:
									return new VariableModel("Result", SymbolModel.SymbolType.Boolean, compare != 0);
								case ExpressionOperatorLogical.LogicalType.Equal:
									return new VariableModel("Result", SymbolModel.SymbolType.Boolean, compare == 0);
								case ExpressionOperatorLogical.LogicalType.Greater:
									return new VariableModel("Result", SymbolModel.SymbolType.Boolean, compare > 0);
								case ExpressionOperatorLogical.LogicalType.GreaterOrEqual:
									return new VariableModel("Result", SymbolModel.SymbolType.Boolean, compare >= 0);
								case ExpressionOperatorLogical.LogicalType.Less:
									return new VariableModel("Result", SymbolModel.SymbolType.Boolean, compare < 0);
								case ExpressionOperatorLogical.LogicalType.LessOrEqual:
									return new VariableModel("Result", SymbolModel.SymbolType.Boolean, compare <= 0);
							}
					break;
			}
			// Si ha llegado hasta aquí es porque no se puede ejecutar la operación
			error = "Cant execute this operation with a string";
			return null;
	}

	/// <summary>
	///		Normaliza una cadena: sin nulos, sin espacios y en mayúsculas
	/// </summary>
	private string NormalizeString(string value)
	{   
		// Normaliza la cadena
		if (string.IsNullOrWhiteSpace(value))
			value = "";
		// Devuelve la cadena sin espacios y en mayúscula
		return value.Trim().ToUpper();
	}

	/// <summary>
	///		Calcula una operación lógica
	/// </summary>
	private VariableModel? ComputeBoolean(ExpressionOperatorBase expression, VariableModel first, VariableModel second, out string error)
	{
		bool firstValue = (bool) first.Value;
		bool secondValue = false;

			// Inicializa los argumentos de salida
			error = string.Empty;
			// Normaliza el segundo valor
			if (second.Value != null)
				secondValue = (bool) second.Value;
			// Ejecuta la operación
			switch (expression)
			{
				case ExpressionOperatorLogical logical:
						switch (logical.Type)
						{
							case ExpressionOperatorLogical.LogicalType.Distinct:
								return new VariableModel("Result", SymbolModel.SymbolType.Boolean, firstValue != secondValue);
							case ExpressionOperatorLogical.LogicalType.Equal:
								return new VariableModel("Result", SymbolModel.SymbolType.Boolean, firstValue == secondValue);
						}
					break;
				case ExpressionOperatorRelational relational:
						switch (relational.Type)
						{
							case ExpressionOperatorRelational.RelationalType.And:
								return new VariableModel("Result", SymbolModel.SymbolType.Boolean, firstValue && secondValue);
							case ExpressionOperatorRelational.RelationalType.Or:
								return new VariableModel("Result", SymbolModel.SymbolType.Boolean, firstValue || secondValue);
						}
					break;
			}
			// Si ha llegado hasta aquí es porque no se ha podido ejecutar la operación
			error = "Cant execute this operation with logic value";
			return null;
	}

	/// <summary>
	///		Procesador
	/// </summary>
	private ProgramProcessor Processor { get; }
}
