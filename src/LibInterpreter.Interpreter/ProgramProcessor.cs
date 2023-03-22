using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

using Bau.Libraries.LibInterpreter.Interpreter.Context;
using Bau.Libraries.LibInterpreter.Interpreter.Context.Functions;
using Bau.Libraries.LibInterpreter.Interpreter.Context.Variables;
using Bau.Libraries.LibInterpreter.Models.Expressions;
using Bau.Libraries.LibInterpreter.Models.Sentences;
using Bau.Libraries.LibInterpreter.Models.Symbols;

namespace Bau.Libraries.LibInterpreter.Interpreter
{
	/// <summary>
	///		Clase abstracta para un intérprete
	/// </summary>
	public abstract class ProgramProcessor
	{   
		protected ProgramProcessor(ProcessorOptions options)
		{
			ExpressionEvaluator = new Evaluator.ExpressionCompute(this);
			Options = options;
		}

		/// <summary>
		///		Inicializa los datos de ejecución
		/// </summary>
		protected void Initialize(Dictionary<string, object?>? arguments)
		{
			// Crea el contexto inicial
			Context.Clear();
			Context.Add();
			// Añade los argumentos al contexto
			if (arguments is not null)
				foreach (KeyValuePair<string, object?> argument in arguments)
					Context.Actual.VariablesTable.Add(new VariableModel(argument.Key, SymbolModel.SymbolType.Unknown, argument.Value));
		}

		/// <summary>
		///		Ejecuta una serie de sentencias
		/// </summary>
		protected async Task ExecuteAsync(List<SentenceBase> sentences, CancellationToken cancellationToken)
		{
			foreach (SentenceBase abstractSentence in sentences)
				if (!Stopped && !cancellationToken.IsCancellationRequested)
					switch (abstractSentence)
					{
						case SentenceException sentence:
								ExecuteException(sentence);
							break;
						case SentenceDeclare sentence:
								await ExecuteDeclareAsync(sentence, cancellationToken);
							break;
						case SentenceLet sentence:
								await ExecuteLetAsync(sentence, cancellationToken);
							break;
						case SentenceFor sentence:
								await ExecuteForAsync(sentence, cancellationToken);
							break;
						case SentenceIf sentence:
								await ExecuteIfAsync(sentence, cancellationToken);
							break;
						case SentenceWhile sentence:
								await ExecuteWhileAsync(sentence, cancellationToken);
							break;
						case SentenceDo sentence:
								await ExecuteDoAsync(sentence, cancellationToken);
							break;
						case SentenceFunction sentence:
								ExecuteFunctionDeclare(sentence);
							break;
						case SentenceCallFunction sentence:
								await ExecuteFunctionCallAsync(sentence, cancellationToken);
							break;
						case SentenceReturn sentence:
								await ExecuteFunctionReturnAsync(sentence, cancellationToken);
							break;
						case SentenceComment sentence:
								ExecuteComment(sentence);
							break;
						case SentencePrint sentence:
								ExecutePrint(sentence);
							break;
						default:
								await ExecuteAsync(abstractSentence, cancellationToken);
							break;
					}
		}

		/// <summary>
		///		Llama al procesador principal para ejecutar una sentencia desconocida
		/// </summary>
		protected abstract Task ExecuteAsync(SentenceBase abstractSentence, CancellationToken cancellationToken);

		/// <summary>
		///		Ejecuta una función implícita
		/// </summary>
		protected abstract Task<VariableModel?> ExecuteAsync(ImplicitFunctionModel function, CancellationToken cancellationToken);

		/// <summary>
		///		Ejecuta una serie de sentencias creando un contexto nuevo
		/// </summary>
		protected async Task ExecuteWithContextAsync(List<SentenceBase> sentences, CancellationToken cancellationToken)
		{
			// Crea el contexto
			Context.Add();
			// Ejecuta las sentencias
			await ExecuteAsync(sentences, cancellationToken);
			// Elimina el contexto
			Context.Pop();
		}

		/// <summary>
		///		Ejecuta una sentencia de declaración
		/// </summary>
		private async Task ExecuteDeclareAsync(SentenceDeclare sentence, CancellationToken cancellationToken)
		{
			VariableModel variable = new(sentence.Variable.Name, sentence.Variable.Type, null);

				// Si es un tipo conocido, añade la variable al contexto
				if (variable.Type == SymbolModel.SymbolType.Unknown)
					AddError($"Unknown variable type: {sentence.Variable.Name} - {sentence.Variable.Type}");
				else
				{
					// Ejecuta la expresión
					if (sentence.Expressions.Count != 0)
						variable.Value = (await ExecuteExpressionAsync(sentence.Expressions, cancellationToken))?.Value;
					else
						variable.AssignDefault();
					// Si no hay errores, añade la variable a la colección
					if (!Stopped)
					{
						// Ejecuta la sentencia
						Context.Actual.VariablesTable.Add(variable);
						// Debug
						AddDebug($"Declare {sentence?.Variable?.Name ?? "Unknown variable"} = " + variable.GetStringValue());
					}
				}
		}

		/// <summary>
		///		Ejecuta una sentencia de asignación
		/// </summary>
		private async Task ExecuteLetAsync(SentenceLet sentence, CancellationToken cancellationToken)
		{
			if (string.IsNullOrWhiteSpace(sentence.Variable))
				AddError("Cant find the variable name");
			else
			{
				VariableModel? variable = Context.Actual.VariablesTable.Get(sentence.Variable);

					// Si no se ha definido la variable, añade un errorDeclara la variable si no existía
					if (variable is null)
						AddError($"Undefined variable {sentence.Variable}");
					else
						variable.Value = (await ExecuteExpressionAsync(sentence.Expressions, cancellationToken))?.Value;
			}
		}

		/// <summary>
		///		Ejecuta una sentencia for
		/// </summary>
		private async Task ExecuteForAsync(SentenceFor sentence, CancellationToken cancellationToken)
		{
			if (string.IsNullOrWhiteSpace(sentence.Variable.Name))
				AddError("Cant find the variable name for loop index");
			else 
			{
				// Interpreta las expresiones de inicio ...
				if (!string.IsNullOrWhiteSpace(sentence.StartExpressionString) && sentence.StartExpression.Count == 0)
					sentence.StartExpression = ParseExpression(sentence.StartExpressionString);
				// Interpreta las expresiones de fin ...
				if (!string.IsNullOrWhiteSpace(sentence.EndExpressionString) && sentence.EndExpression.Count == 0)
					sentence.EndExpression = ParseExpression(sentence.EndExpressionString);
				// Interpreta las expresiones de paso ...
				if (!string.IsNullOrWhiteSpace(sentence.StepExpressionString) && sentence.StepExpression.Count == 0)
					sentence.StepExpression = ParseExpression(sentence.EndExpressionString);
				// Ejecuta el bucle
				if (sentence.StartExpression.Count == 0)
					AddError("Cant find the start expression for loop index");
				else if (sentence.EndExpression.Count == 0)
					AddError("Cant find the end expression for loop index");
				else
				{
					VariableModel index = await GetVariableValueAsync(sentence.Variable.Name, sentence.StartExpression, cancellationToken);

						if (index.Type != sentence.Variable.Type)
							AddError($"The start expression result at loop for is not type {sentence.Variable.Type}. Variable: {sentence.Variable.Name}");
						else if (index.Type != SymbolModel.SymbolType.Numeric && index.Type != SymbolModel.SymbolType.Date)
							AddError($"The value of start at for loop must be numeric or date. Variable: {sentence.Variable.Name}");
						else
						{
							VariableModel end = await GetVariableValueAsync($"EndIndex_Context_{Context.Actual.ScopeIndex}", sentence.EndExpression, cancellationToken);

								// Si se han podido evaluar las expresiones de inicio y fin
								if (!Stopped)
								{
									if (index.Type != end.Type)
										AddError($"The types of start and end variable at for loop are distinct. Variable: {sentence.Variable.Name}");
									else
									{
										VariableModel step = new($"StepIndex_Context_{Context.Actual.ScopeIndex}", SymbolModel.SymbolType.Numeric, 1);

											// Asigna el valor a la expresión si no tenía
											if (sentence.StepExpression.Count > 0)
												step = await GetVariableValueAsync(step.Name, sentence.StepExpression, cancellationToken);
											// Comprueba que el paso sea numérico
											if (!Stopped)
											{
												if (!CanExecuteStep(index, step))
													AddError($"The step type is not compatible with for variable. Variable: {sentence.Variable.Name}");
												else // Ejecuta el bucle for
													await ExecuteForLoopAsync(sentence, index, end, step, cancellationToken);
											}
									}
								}
						}
				}
			}

			// Comprueba que se pueda incrementar el paso: si el índice es fecha y el intervalo es fecha o númerico (predeterminado un día)
			// o el índice y el paso tienen el mismo tipo
			bool CanExecuteStep(VariableModel index, VariableModel step)
			{
				return (index.Type == SymbolModel.SymbolType.Date && 
							(step.Type == SymbolModel.SymbolType.Numeric || step.Type == SymbolModel.SymbolType.Date)) ||
					   index.Type == step.Type;
			}
		}


		/// <summary>
		///		Ejecuta el contenido de un bucle for
		/// </summary>
		private async Task ExecuteForLoopAsync(SentenceFor sentence, VariableModel index, VariableModel end, VariableModel step, CancellationToken cancellationToken)
		{
			bool isPositiveStep = end.IsGreaterThan(index);

				// Antes de ejecutar el bucle comprueba si se trata de un bucle infinito
				if ((isPositiveStep && !IsPositive(step)) || (!isPositiveStep && IsPositive(step)))
					AddError($"The for loop is infinite. Variable {index.Name}");
				else
				{
					// Abre un nuevo contexto
					Context.Add();
					// Añade la variable al contexto
					Context.Actual.VariablesTable.Add(index);
					// Ejecuta las sentencias
					while (!IsEndForLoop(index, end, isPositiveStep) && !Stopped)
					{
						// Ejecuta las sentencias
						await ExecuteAsync(sentence.Sentences, cancellationToken);
						// Incrementa / decrementa el valor al índice (el step debería ser -x si es negativo, por tanto, siempre se suma)
						index.Sum(step);
						// y lo ajusta en el contexto
						Context.Actual.VariablesTable.Add(index);
					}
					// Elimina el contexto
					Context.Pop();
				}
		}

		/// <summary>
		///		Comprueba si una variable tiene un valor positivo
		/// </summary>
		private bool IsPositive(VariableModel variable)
		{
			VariableModel compareWith = new($"Variable_{Guid.NewGuid().ToString()}", SymbolModel.SymbolType.Numeric, 0);

				// Comprueba si la variable es positiva
				return variable.IsGreaterThan(compareWith);
		}

		/// <summary>
		///		Comprueba si se ha terminado un bucle for
		/// </summary>
		private bool IsEndForLoop(VariableModel index, VariableModel end, bool isPositiveStep)
		{
			if (isPositiveStep)
				return index.IsGreaterThan(end);
			else
				return index.IsLessThan(end);
		}

		/// <summary>
		///		Obtiene el valor de una variable
		/// </summary>
		private async Task<VariableModel> GetVariableValueAsync(string name, ExpressionsCollection expressions, CancellationToken cancellationToken)
		{
			if (expressions.Count > 0)
			{
				VariableModel? result = await ExecuteExpressionAsync(expressions, cancellationToken);

					// Asigna el nuevo tipo
					if (result is null)
						throw new Exceptions.InterpreterException($"Error when compute expression for {name}");
					else
						return new VariableModel(name, result.Type, result.Value);
			}
			else
				return new VariableModel(name, SymbolModel.SymbolType.Unknown, null);
		}

		/// <summary>
		///		Ejecuta una sentencia de excepción
		/// </summary>
		private void ExecuteException(SentenceException sentence)
		{
			if (string.IsNullOrWhiteSpace(sentence.Message))
				AddError("Exception");
			else
				AddError(FormatString(sentence.Message));
		}

		/// <summary>
		///		Ejecuta un comentario: no debería hacer nada, simplemente añade información de depuración
		/// </summary>
		private void ExecuteComment(SentenceComment sentence)
		{
			AddDebug($"Comment: {sentence.Content}");
		}

		/// <summary>
		///		Ejecuta una sentencia de impresión
		/// </summary>
		private void ExecutePrint(SentencePrint sentence)
		{
			if (string.IsNullOrWhiteSpace(sentence.Message))
				AddInfo("Print");
			else
				AddInfo(FormatString(sentence.Message));
		}

		/// <summary>
		///		Ejecuta una sentencia condicional
		/// </summary>
		private async Task ExecuteIfAsync(SentenceIf sentence, CancellationToken cancellationToken)
		{
			if (sentence.ConditionExpression.Empty)
				AddError("Cant find condition for if sentence");
			else
			{
				VariableModel? result = await ExecuteExpressionAsync(sentence.ConditionExpression, cancellationToken);

					if (result != null)
					{
						if (result.Type != SymbolModel.SymbolType.Boolean || result.Value is not bool resultLogical)
							AddError("If condition result is not a logical value");
						else
						{
							if (resultLogical && !sentence.SentencesThen.Empty)
								await ExecuteWithContextAsync(sentence.SentencesThen, cancellationToken);
							else if (!resultLogical && !sentence.SentencesElse.Empty)
								await ExecuteWithContextAsync(sentence.SentencesElse, cancellationToken);
						}
					}
					else
						AddError("Cant execute if condition");
			}
		}

		/// <summary>
		///		Ejecuta un bucle while
		/// </summary>
		private async Task ExecuteWhileAsync(SentenceWhile sentence, CancellationToken cancellationToken)
		{
			if (sentence.Condition.Empty)
				AddError("Cant find condition for while loop");
			else 
			{
				bool end = false;

					// Ejecuta el bucle
					while (!end && !Stopped)
					{
						VariableModel? result = await ExecuteExpressionAsync(sentence.Condition, cancellationToken);

							if (result != null)
							{
								if (result.Type != SymbolModel.SymbolType.Boolean || result.Value is not bool resultLogical)
									AddError("While condition result is not a logical value");
								else if (resultLogical)
									await ExecuteWithContextAsync(sentence.Sentences, cancellationToken);
								else
									end = true;
							}
					}
			}
		}

		/// <summary>
		///		Ejecuta un bucle do ... while
		/// </summary>
		private async Task ExecuteDoAsync(SentenceDo sentence, CancellationToken cancellationToken)
		{
			if (sentence.Condition.Empty)
				AddError("Cant find condition for do ... while loop");
			else
			{
				bool end = false;

					// Ejecuta el bucle
					do
					{
						VariableModel? result;

							// Ejecuta las sentencias del bucle
							await ExecuteWithContextAsync(sentence.Sentences, cancellationToken);
							// Ejecuta la condición
							result = await ExecuteExpressionAsync(sentence.Condition, cancellationToken);
							// Comprueba el resultado
							if (result != null)
							{
								if (result.Type != SymbolModel.SymbolType.Boolean || result.Value is not bool resultLogical)
									AddError("Do while condition result is not a logical value");
								else if (!resultLogical)
									end = true;
							}
					}
					while (!end && !Stopped);
			}
		}

		/// <summary>
		///		Ejecuta la declaración de una función: añade la función a la tabla de funciones del contexto
		/// </summary>
		private void ExecuteFunctionDeclare(SentenceFunction sentence)
		{
			if (string.IsNullOrWhiteSpace(sentence.Definition.Name))
				AddError("Cant find name for function declare");
			else
				Context.Actual.FunctionsTable.Add(sentence);
		}

		/// <summary>
		///		Ejecuta una llamada a una función
		/// </summary>
		private async Task ExecuteFunctionCallAsync(SentenceCallFunction sentence, CancellationToken cancellationToken)
		{
			if (string.IsNullOrWhiteSpace(sentence.Function))
				AddError("Cant find the name function for call");
			else
			{
				BaseFunctionModel function = Context.Actual.FunctionsTable.GetIfExists(sentence.Function);

					if (function == null)
						AddError($"Cant find the function to call: {sentence.Function}");
					else
						await ExecuteFunctionAsync(function, sentence.Arguments, false, cancellationToken);
			}
		}

		/// <summary>
		///		Ejecuta una función a partir de una expresión
		/// </summary>
		internal async Task<VariableModel?> ExecuteFunctionAsync(ExpressionFunction expression, CancellationToken cancellationToken)
		{
			VariableModel? result = null;

				// Busca la función y la ejecuta
				if (string.IsNullOrWhiteSpace(expression.Function))
					AddError("Cant find the name funcion for call");
				else
				{
					BaseFunctionModel function = Context.Actual.FunctionsTable.GetIfExists(expression.Function);

						if (function == null)
							AddError($"Cant find the function to call: {expression.Function}");
						else
							result = await ExecuteFunctionAsync(function, expression.Arguments, true, cancellationToken);
				}
				// Devuelve el resultado de la función
				return result;
		}

		/// <summary>
		///		Ejecuta una función
		/// </summary>
		private async Task<VariableModel?> ExecuteFunctionAsync(BaseFunctionModel function, List<ExpressionsCollection> arguments, 
															    bool waitReturn, CancellationToken cancellationToken)
		{
			VariableModel? result = null;

				// Crea un nuevo contexto
				Context.Add();
				// Añade los argumentos al contexto
				foreach (SymbolModel argument in function.Arguments)
					if (!Stopped)
					{
						int index = function.Arguments.IndexOf(argument);

							// Si el argumento corresponde a un parámetro, se añade al contexto esa variable con el valor
							if (arguments.Count > index)
							{
								VariableModel? argumentResult = await ExecuteExpressionAsync(arguments[index], cancellationToken);

									if (argumentResult != null)
										Context.Actual.VariablesTable.Add(argument.Name, argument.Type, argumentResult.Value);
							}
							else
								AddError($"Cant find any call value for argument {argument.Name}");
					}
				// Si no ha habido errores al calcular los argumentos, ejecuta realmente las sentencias de la función (o llama a la función implícita)
				if (!Stopped)
				{
					// Ejecuta las sentencias de la función
					switch (function)
					{
						case ImplicitFunctionModel implicitFunction:
								result = await ExecuteAsync(implicitFunction, cancellationToken);
							break;
						case UserDefinedFunctionModel userDefinedFunction:
								// Añade el nombre que debe tener el valor de retorno
								Context.Actual.ScopeFuntionResultVariable = "Return_" + Guid.NewGuid().ToString();
								// Ejecuta la sentencia
								await ExecuteAsync(userDefinedFunction.Sentences, cancellationToken);
								// Si es una función, no una subrutina, obtiene el resultado
								if (waitReturn)
								{
									// y obtiene el resultado
									result = Context.Actual.VariablesTable.Get(Context.Actual.ScopeFuntionResultVariable);
									// Si no se ha obtenido ningún resultado (faltaba la sentencia return), añade un error
									if (result == null)
										AddError($"Cant find result for funcion {function.Definition.Name}. Check if define return sentence");
								}
							break;
					}
				}
				// Elimina el contexto
				Context.Pop();
				// Devuelve el resultado de la función
				return result;
		}

		/// <summary>
		///		Ejecuta la sentencia para devolver el resultado de una función
		/// </summary>
		private async Task ExecuteFunctionReturnAsync(SentenceReturn sentence, CancellationToken cancellationToken)
		{
			if (string.IsNullOrWhiteSpace(Context.Actual.ScopeFuntionResultVariable))
				AddError("Cant execute a return because there is not function block");
			else
			{
				VariableModel? result = await ExecuteExpressionAsync(sentence.Expression, cancellationToken);

					// Si no hay error, añade el resultado al contexto
					if (result != null)
						Context.Actual.VariablesTable.Add(Context.Actual.ScopeFuntionResultVariable, result.Type, result.Value);
			}
		}

		/// <summary>
		///		Ejecuta una expresión
		/// </summary>
		protected async Task<VariableModel?> ExecuteExpressionAsync(ExpressionsCollection expressions, CancellationToken cancellationToken)
		{
			(string error, VariableModel? result) = await ExpressionEvaluator.EvaluateAsync(Context.Actual, expressions, cancellationToken);

				// Añade el error si es necesario
				if (!string.IsNullOrWhiteSpace(error))
				{
					// Añade el error
					AddError(error);
					// El resultado no es válido
					result = null;
				}
				// Devuelve el resultado
				return result;
		}

		/// <summary>
		///		Interpreta una cadena de expresión
		/// </summary>
		protected abstract ExpressionsCollection ParseExpression(string expression);

		/// <summary>
		///		Formatea una cadena (en su caso, puede que cambie las variables)
		/// </summary>
		protected abstract string FormatString(string value);

		/// <summary>
		///		Añade un mensaje de depuración
		/// </summary>
		protected abstract void AddDebug(string message, [CallerFilePath] string? fileName = null, 
										 [CallerMemberName] string? methodName = null, [CallerLineNumber] int lineNumber = 0);

		/// <summary>
		///		Añade un mensaje informativo
		/// </summary>
		protected abstract void AddInfo(string message, [CallerFilePath] string? fileName = null, 
										[CallerMemberName] string? methodName = null, [CallerLineNumber] int lineNumber = 0);

		/// <summary>
		///		Añade una cadena a la consola
		/// </summary>
		protected abstract void AddConsoleOutput(string message, [CallerFilePath] string? fileName = null, 
												 [CallerMemberName] string? methodName = null, [CallerLineNumber] int lineNumber = 0);

		/// <summary>
		///		Añade un error
		/// </summary>
		protected abstract void AddError(string error, Exception? exception = null, [CallerFilePath] string? fileName = null, 
										 [CallerMemberName] string? methodName = null, [CallerLineNumber] int lineNumber = 0);

		/// <summary>
		///		Opciones de ejecución del procesador
		/// </summary>
		protected ProcessorOptions Options { get; }

		/// <summary>
		///		Contexto de ejecución
		/// </summary>
		protected ContextStackModel Context { get; } = new();

		/// <summary>
		///		Indica si se ha detenido el programa por una excepción
		/// </summary>
		protected bool Stopped { get; set; }

		/// <summary>
		///		Evaluador de expresiones
		/// </summary>
		private Evaluator.ExpressionCompute ExpressionEvaluator { get; }
	}
}