using Bau.Libraries.LibInterpreter.Models.Symbols;

namespace Bau.Libraries.LibInterpreter.Interpreter.Context.Variables;

/// <summary>
///		Tabla de variables
/// </summary>
public class TableVariableModel(ContextModel context)
{
	/// <summary>
	///		Añade una colección de variables
	/// </summary>
	public void AddRange(List<VariableModel> variables)
	{
		if (variables is not null)
			foreach (VariableModel variable in variables)
				Add(variable);
	}

	/// <summary>
	///		Añade una variable
	/// </summary>
	public VariableModel Add(VariableModel variable) => Add(variable.Name, variable.Type, variable.Value);

	/// <summary>
	///		Añade una variable
	/// </summary>
	public VariableModel Add(string name, SymbolModel.SymbolType type, object? value)
	{
		// Añade / modifica el valor
		if (Variables.ContainsKey(name))
			Variables[name].Value = value;
		else
		{
			VariableModel variable = new(name, type, value);

				// Asigna el valor
				variable.Value = value;
				// Añade la variable a la tabla
				Variables.Add(name, variable);
		}
		// Devuelve la variable
		return Variables[name];
	}

	/// <summary>
	///		Clona todas las variables de este contexto
	/// </summary>
	public Dictionary<string, VariableModel> GetAll()
	{
		Dictionary<string, VariableModel> variables = new(StringComparer.CurrentCultureIgnoreCase);

			// Convierte las variables
			foreach (KeyValuePair<string, VariableModel> item in Variables)
				variables.Add(item.Key, item.Value);
			// Devuelve la colección de variables
			return variables;
	}

	/// <summary>
	///		Obtiene el diccionario de la tabla de variables
	/// </summary>
	public Dictionary<string, object?> GetDictionary()
	{
		Dictionary<string, object?> result = new(StringComparer.CurrentCultureIgnoreCase);

			// Obtiene los valores de las variables
			foreach (KeyValuePair<string, VariableModel> keyValue in Variables)
				result.Add(keyValue.Key, keyValue.Value.Value);
			// Devuelve el diccionario
			return result;
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
	///		Obtiene una variable si existe. Dependiendo de las opciones de la interpretación del proyecto, crea la variable si no existe
	/// </summary>
	//TODO --> FALTA buscar por el índice
	public VariableModel? Get(string name, bool needDeclareVariables, int? index = null)
	{
		VariableModel? variable = SearchRecursive(name, index);

			// Si no se ha encontrado la variable pero las opciones del proyecto indican que no es necesario definirla previamente,
			// la añade a este contexto (por eso la búsqueda recursiva está fuera de este método
			if (variable is null && !needDeclareVariables)
				variable = Add(name, SymbolModel.SymbolType.Unknown, null);
			// Devuelve la variable localizada
			return variable;
	}

	/// <summary>
	///		Busca una variable recursivamente. Es decir, si no está en este contexto, lo busca en el padre
	/// </summary>
	private VariableModel? SearchRecursive(string name, int? index = null)
	{
		// Obtiene el valor
		if (Variables.ContainsKey(name))
			return Variables[name];
		else if (Context.Parent is not null)
			return Context.Parent.VariablesTable.Get(name, false);
		else
			return null;
	}

	/// <summary>
	///		Comprueba si existe una variable en la tabla
	/// </summary>
	public bool Exists(string name) => Variables.ContainsKey(name);

	/// <summary>
	///		Elimina una variable de la tabla
	/// </summary>
	public void Remove(string name)
	{
		if (Variables.ContainsKey(name))
			Variables.Remove(name);
	}

	/// <summary>
	///		Indizador
	/// </summary>
	public VariableModel? this[string name]
	{
		get { return Get(name, false); }
		set 
		{ 
			if (value is not null)
				Add(value); 
		}
	}

	/// <summary>
	///		Contexto
	/// </summary>
	private ContextModel Context { get; } = context;

	/// <summary>
	///		Diccionario de variables
	/// </summary>
	private Dictionary<string, VariableModel> Variables { get; } = new(StringComparer.CurrentCultureIgnoreCase);
}