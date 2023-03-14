using System;
using System.Collections.Generic;

namespace Bau.Libraries.LibInterpreter.Interpreter.Context
{
	/// <summary>
	///		Pila de <see cref="ContextModel"/>
	/// </summary>
	public class ContextStackModel
	{
		/// <summary>
		///		Añade un contexto a la pila
		/// </summary>
		public void Add()
		{
			if (Contexts.Count > 0)
				Contexts.Add(new ContextModel(Actual));
			else
				Contexts.Add(new ContextModel(null));
		}

		/// <summary>
		///		Limpia el contexto
		/// </summary>
		public void Clear()
		{
			Contexts.Clear();
		}

		/// <summary>
		///		Quita el último contexto de la pila
		/// </summary>
		public void Pop()
		{
			if (Contexts.Count > 0)
				Contexts.RemoveAt(Contexts.Count - 1);
		}

		/// <summary>
		///		Contexto actual
		/// </summary>
		public ContextModel Actual
		{
			get 
			{
				if (Contexts.Count == 0)
					throw new Exceptions.InterpreterException("Context undefined");
				else
					return Contexts[Contexts.Count - 1];
			}
		}

		/// <summary>
		///		Contextos
		/// </summary>
		private List<ContextModel> Contexts { get; } = new();
	}
}
