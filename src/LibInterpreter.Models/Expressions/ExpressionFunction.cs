using System;
using System.Collections.Generic;

namespace Bau.Libraries.LibInterpreter.Models.Expressions
{
	/// <summary>
	///		Expresión de llamada a función
	/// </summary>
	public class ExpressionFunction : ExpressionBase
	{
		public ExpressionFunction(string function)
		{
			Function = function;
		}

		/// <summary>
		///		Clona la expresión de llamada
		/// </summary>
		public override ExpressionBase Clone()
		{
			ExpressionFunction target = new ExpressionFunction(Function);

				// Clona las expresiones
				target.Arguments.AddRange(Arguments);
				// Devuelve el objeto clonado
				return target;
		}

		/// <summary>
		///		Obtiene la información de depuración
		/// </summary>
		public override string GetDebugInfo()
		{
			string debug = Function;

				// Añade el índice
				foreach (ExpressionsCollection argument in Arguments)
					foreach (ExpressionBase expression in argument)
						debug += "[" + expression.GetDebugInfo() + "]";
				// Devuelve la información de depuración
				return debug;
		}

		/// <summary>
		///		Nombre de la función
		/// </summary>
		public string Function { get; }

		/// <summary>
		///		Expresiones de los argumentos de llamada
		/// </summary>
		public List<ExpressionsCollection> Arguments { get; } = new List<ExpressionsCollection>();
	}
}
