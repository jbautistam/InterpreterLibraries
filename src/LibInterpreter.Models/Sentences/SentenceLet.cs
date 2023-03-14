using System;

namespace Bau.Libraries.LibInterpreter.Models.Sentences
{
	/// <summary>
	///		Sentencia para ejecución de una expresión
	/// </summary>
	public class SentenceLet : SentenceBase
	{
		/// <summary>
		///		Tipo de la variable de salida: sólo es necesario cuando no está definida
		/// </summary>
		public Symbols.SymbolModel.SymbolType Type { get; set; }

		/// <summary>
		///		Nombre de variable
		/// </summary>
		public string Variable { get; set; }

		/// <summary>
		///		Cadena con la expresión de asignación (no interpretada)
		/// </summary>
		public string ExpressionString { get; set; }

		/// <summary>
		///		Expresiones a ejecutar
		/// </summary>
		public Expressions.ExpressionsCollection Expressions { get; set; } = new Expressions.ExpressionsCollection();
	}
}