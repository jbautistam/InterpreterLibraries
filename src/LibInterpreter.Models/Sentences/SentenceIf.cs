using System;

namespace Bau.Libraries.LibInterpreter.Models.Sentences
{
	/// <summary>
	///		Sentencia de ejecución de una condición
	/// </summary>
	public class SentenceIf : SentenceBase
	{
		/// <summary>
		///		Condición (cuando aún no se han interpretado las expresiones)
		/// </summary>
		public string Condition { get; set; }

		/// <summary>
		///		Condición
		/// </summary>
		public Expressions.ExpressionsCollection ConditionExpression { get; } = new Expressions.ExpressionsCollection();

		/// <summary>
		///		Sentencias a ejecutar si el resultado de la condición es verdadero
		/// </summary>
		public SentenceCollection SentencesThen { get; } = new SentenceCollection();

		/// <summary>
		///		Sentencias a ejecutar si el resultado de la condición es false
		/// </summary>
		public SentenceCollection SentencesElse { get; } = new SentenceCollection();
	}
}
