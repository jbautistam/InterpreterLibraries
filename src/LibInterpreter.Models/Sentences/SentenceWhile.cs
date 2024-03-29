﻿namespace Bau.Libraries.LibInterpreter.Models.Sentences;

/// <summary>
///		Sentencia para un bucle While
/// </summary>
public class SentenceWhile : SentenceBase
{
	/// <summary>
	///		Condición del bucle cuando aún no se han interpretado las expresiones
	/// </summary>
	public string ConditionString { get; set; }

	/// <summary>
	///		Condición
	/// </summary>
	public Expressions.ExpressionsCollection Condition { get; } = new Expressions.ExpressionsCollection();

	/// <summary>
	///		Sentencias
	/// </summary>
	public SentenceCollection Sentences { get; } = new SentenceCollection();
}
