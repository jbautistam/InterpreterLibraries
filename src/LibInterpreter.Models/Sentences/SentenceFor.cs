using System;

namespace Bau.Libraries.LibInterpreter.Models.Sentences
{
	/// <summary>
	///		Sentencia de ejecución de un bucle 
	/// </summary>
	public class SentenceFor : SentenceBase
	{
		/// <summary>
		///		Nombre de variable
		/// </summary>
		public Symbols.SymbolModel Variable { get; } = new();

		/// <summary>
		///		Cadena con la expresión de inicio cuando aún no se ha interpretado
		/// </summary>
		public string StartExpressionString { get; set; }

		/// <summary>
		///		Expresión para el valor de inicio
		/// </summary>
		public Expressions.ExpressionsCollection StartExpression { get; set; } = new();

		/// <summary>
		///		Cadena con la expresión de fin cuando aún no se ha interpretado
		/// </summary>
		public string EndExpressionString { get; set; }

		/// <summary>
		///		Expresión para el valor de fin
		/// </summary>
		public Expressions.ExpressionsCollection EndExpression { get; set; } = new();

		/// <summary>
		///		Cadena con la expresión de incremento cuando aún no se ha interpretado
		/// </summary>
		public string StepExpressionString { get; set; }

		/// <summary>
		///		Expresión para el valor del paso
		/// </summary>
		public Expressions.ExpressionsCollection StepExpression { get; set; } = new();

		/// <summary>
		///		Sentencias a ejecutar
		/// </summary>
		public SentenceCollection Sentences { get; } = new();
	}
}
