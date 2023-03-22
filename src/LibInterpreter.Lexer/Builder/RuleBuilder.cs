using System;
using System.Collections.Generic;

using Bau.Libraries.LibInterpreter.Lexer.Rules;

namespace Bau.Libraries.LibInterpreter.Lexer.Builder
{
	/// <summary>
	///		Generador de <see cref="RuleBase"/>
	/// </summary>
	public class RuleBuilder
	{
		/// <summary>
		///		Añade una regla <see cref="RuleDelimited"/> para el mismo separador de inicio y fin
		/// </summary>
		public RuleBuilder WithDelimited(string token, string separator)
		{
			// Añade una regla delimitada
			Rules.Add(new RuleDelimited(token, new[] { separator }, new[] { separator }, false, false, false, false, false));
			// Devuelve el generador
			return this;
		}

		/// <summary>
		///		Añade una regla <see cref="RuleDelimited"/> para un separador de inicio y otro de fin
		/// </summary>
		public RuleBuilder WithDelimited(string token, string start, string end)
		{
			// Añade una regla delimitada
			Rules.Add(new RuleDelimited(token, new[] { start }, new[] { end }, false, false, false, false, false));
			// Devuelve el generador
			return this;
		}

		/// <summary>
		///		Añade una regla <see cref="RuleWordFixed"/> con varias palabras claves
		/// </summary>
		public RuleBuilder WithWords(string token, params string[] words)
		{
			// Añade una regla asociada a una serie de palabras clave
			Rules.Add(new RuleWordFixed(token, words));
			// Devuelve el generador
			return this;
		}

		/// <summary>
		///		Añade una regla <see cref="RulePattern"/> con patrones
		/// </summary>
		public RuleBuilder WithPattern(string token, string patternStart, string patternContent)
		{
			// Añade el patrón
			Rules.Add(new RulePattern(token, patternStart, patternContent));
			// Devuelve el generador
			return this;
		}

		/// <summary>
		///		Añade una regla predefinida para números
		/// </summary>
		public RuleBuilder WithDefaultNumbers(string token)
		{
			return WithPattern(token, "9", "9.");
		}

		/// <summary>
		///		Añade una regla para los operadores matemáticos (paréntesis de apertura y cierra, +, -, *, / y %)
		/// </summary>
		public RuleBuilder WithDefaultMathOperators(string token)
		{
			return WithWords(token, "(", ")", "+", "-", "*", "/", "%");
		}

		/// <summary>
		///		Añade una regla para los operadores relacionales (||, &&, !)
		/// </summary>
		public RuleBuilder WithDefaultRelationalOperators(string token)
		{
			return WithWords(token, "||", "&&", "!");
		}

		/// <summary>
		///		Añade una regla para los operadores lógicos (<, >, >=, <=, ==, !=)
		/// </summary
		public RuleBuilder WithDefaultLogicalOperators(string token)
		{
			return WithWords(token, "<", ">", ">=", "<=", "==", "!=");
		}

		/// <summary>
		///		Genera las reglas
		/// </summary>
		public List<RuleBase> Build() => Rules;

		/// <summary>
		///		Reglas generadas
		/// </summary>
		private List<RuleBase> Rules { get; } = new();
	}
}
