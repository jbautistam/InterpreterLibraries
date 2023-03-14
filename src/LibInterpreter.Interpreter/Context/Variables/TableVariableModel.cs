using System;
using System.Collections.Generic;

using Bau.Libraries.LibInterpreter.Models.Symbols;

namespace Bau.Libraries.LibInterpreter.Interpreter.Context.Variables
{
	/// <summary>
	///		Tabla de variables
	/// </summary>
	public class TableVariableModel
	{
		public TableVariableModel(ContextModel context)
		{
			Context = context;
		}

		/// <summary>
		///		Añade una colección de variables
		/// </summary>
		public void AddRange(List<VariableModel> variables)
		{
			if (variables != null)
				foreach (VariableModel variable in variables)
					Add(variable);
		}

		/// <summary>
		///		Añade una variable
		/// </summary>
		public void Add(string name, object? value)
		{
			Add(new VariableModel(name, value));
		}

		/// <summary>
		///		Añade una variable
		/// </summary>
		public void Add(VariableModel? variable)
		{
			if (variable is not null)
				Add(variable.Name, variable.Type, variable.Value);
		}

		/// <summary>
		///		Añade una variable de un tipo con el valor por defecto
		/// </summary>
		public void Add(string name, SymbolModel.SymbolType type)
		{
			switch (type)
			{
				case SymbolModel.SymbolType.Boolean:
						Add(name, type, false);
					break;
				case SymbolModel.SymbolType.Date:
						Add(name, type, null);
					break;
				case SymbolModel.SymbolType.Numeric:
						Add(name, type, 0);
					break;
				case SymbolModel.SymbolType.String:
						Add(name, type, string.Empty);
					break;
				default:
					throw new ArgumentException("Type unknown");
			}
		}

		/// <summary>
		///		Añade una variable
		/// </summary>
		public void Add(string name, SymbolModel.SymbolType type, object? value)
		{
			string key = Normalize(name);

				// Añade / modifica el valor
				if (Variables.ContainsKey(key))
					Variables[key].Value = value;
				else
				{
					VariableModel variable = new VariableModel(name, type);

						// Asigna el valor
						variable.Value = value;
						// Añade la variable a la tabla
						Variables.Add(key, variable);
				}
		}

		/// <summary>
		///		Clona todas las variables de este contexto
		/// </summary>
		public Dictionary<string, VariableModel> GetAll()
		{
			Dictionary<string, VariableModel> variables = new Dictionary<string, VariableModel>();

				// Convierte las variables
				foreach (KeyValuePair<string, VariableModel> item in Variables)
					variables.Add(item.Key, item.Value);
				// Devuelve la colección de variables
				return variables;
		}

		/// <summary>
		///		Enumera las variables de este contexto
		/// </summary>
		public IEnumerable<(string key, VariableModel variable)> Enumerate()
		{
			foreach (KeyValuePair<string, VariableModel> item in Variables)
				yield return (item.Key, item.Value);
		}

		/// <summary>
		///		Obtiene una variable (si existe, no la crea si no existe en la tabla de variables)
		/// </summary>
		//TODO --> FALTA buscar por el índice
		public VariableModel? Get(string name, int? index = null)
		{
			// Normaliza el nombre
			name = Normalize(name);
			// Obtiene el valor
			if (Variables.ContainsKey(name))
				return Variables[name];
			else if (Context.Parent != null)
				return Context.Parent.VariablesTable.Get(name);
			else
				return null;
		}

		/// <summary>
		///		Comprueba si existe una variable en la tabla
		/// </summary>
		public bool Exists(string name)
		{
			return Variables.ContainsKey(Normalize(name));
		}

		/// <summary>
		///		Normaliza el nombre de la variable
		/// </summary>
		private string Normalize(string name)
		{
			return name.ToUpper();
		}

		/// <summary>
		///		Indizador
		/// </summary>
		public VariableModel? this[string name]
		{
			get { return Get(name); }
			set { Add(value); }
		}

		/// <summary>
		///		Contexto
		/// </summary>
		private ContextModel Context { get; }

		/// <summary>
		///		Diccionario de variables
		/// </summary>
		private Dictionary<string, VariableModel> Variables { get; } = new();
	}
}