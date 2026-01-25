using Bau.Libraries.LibInterpreter.Models.Symbols;

namespace Bau.Libraries.LibInterpreter.Interpreter.Context.Variables;

/// <summary>
///		Clase con los datos de una variable
/// </summary>
public class VariableModel
{
	/// <summary>
	///		Resultado de una comparación
	/// </summary>
	public enum CompareResult
	{
		/// <summary>Ambos datos son iguales</summary>
		Equals,
		/// <summary>El primer dato es mayor que el segundo</summary>
		GreaterThan,
		/// <summary>El primer dato es menor que el segundo</summary>
		LessThan
	}

	/// <summary>
	///		Tipos de incrementos de fecha
	/// </summary>
	private enum DateIncrement
	{
		/// <summary>Por días</summary>
		Day,
		/// <summary>Por semanas</summary>
		Week,
		/// <summary>Por mes</summary>
		Month,
		/// <summary>Por año</summary>
		Year
	}
	// Variables privadas
	private SymbolModel.SymbolType _type = SymbolModel.SymbolType.Unknown;

	public VariableModel(string name, SymbolModel.SymbolType type, object? value)
	{
		// Asigna los datos
		Name = name;
		Type = type;
		Value = value;
		// Asigna el valor predeterminado
		if (value is null)
			AssignDefault();
	}

	/// <summary>
	///		Infiere el tipo de variable a partir del tipo de valor
	/// </summary>
	private SymbolModel.SymbolType InferType(object value)
	{
		return value switch
					{
						decimal or float or double or int or long or short or byte => SymbolModel.SymbolType.Numeric,
						string => SymbolModel.SymbolType.String,
						DateTime or DateOnly or TimeOnly => SymbolModel.SymbolType.Date,
						bool => SymbolModel.SymbolType.Boolean,
						_ => SymbolModel.SymbolType.Object
					};
	}

	/// <summary>
	///		Asigna el valor predeterminado a la variable
	/// </summary>
	internal void AssignDefault()
	{
		switch (Type)
		{
			case SymbolModel.SymbolType.Boolean:
					Value = false;
				break;
			case SymbolModel.SymbolType.Date:
					Value = new DateTime(1900, 1, 1);
				break;
			case SymbolModel.SymbolType.Numeric:
					Value = 0;
				break;
			case SymbolModel.SymbolType.String:
					Value = string.Empty;
				break;
		}
	}

	/// <summary>
	///		Convierte el valor de la variable a una cadena para depuración
	/// </summary>
	public string GetStringValue()
	{
		if (Value is null)
			return "null";
		else
			return ConvertToString(this);
	}

	/// <summary>
	///		Comprueba si el valor de la variable es mayor que un valor
	/// </summary>
	public bool IsGreaterThan(VariableModel value) => Compare(value) == CompareResult.GreaterThan;

	/// <summary>
	///		Comprueba si el valor de la variable es menor que un valor
	/// </summary>
	public bool IsLessThan(VariableModel value) => Compare(value) == CompareResult.LessThan;

	/// <summary>
	///		Comprueba si el valor de la variable es mayor o igual que otro
	/// </summary>
	public bool IsGreaterOrEqualThan(VariableModel value)
	{
		CompareResult result = Compare(value);

			return result == CompareResult.Equals || result == CompareResult.GreaterThan;
	}

	/// <summary>
	///		Comprueba si el valor de la variable es menor o igual que otro
	/// </summary>
	public bool IsLessOrEqualThan(VariableModel value)
	{
		CompareResult result = Compare(value);

			return result == CompareResult.Equals || result == CompareResult.LessThan;
	}

	/// <summary>
	///		Suma el valor de una variable
	/// </summary>
	public void Sum(VariableModel value)
	{
		switch (Type)
		{
			case SymbolModel.SymbolType.String:
					Value = Value?.ToString() + value.Value?.ToString();
				break;
			case SymbolModel.SymbolType.Numeric:
					Value = ConvertToNumeric(this) + ConvertToNumeric(value);
				break;
			case SymbolModel.SymbolType.Boolean:
					Value = ConvertToBoolean(this) || ConvertToBoolean(value);
				break;
			case SymbolModel.SymbolType.Date:
					DateTime? date = ConvertToDate(this);

						if (date == null)
							throw new NotImplementedException($"Source date has a null value. Variable {Name}");
						else
							Value = SumSubstractDate(date ?? DateTime.Now, ConvertToString(value), true);
				break;
		}
	}

	/// <summary>
	///		Resta el valor de una variable
	/// </summary>
	public void Substract(VariableModel value)
	{
		switch (Type)
		{
			case SymbolModel.SymbolType.String:
				throw new NotImplementedException($"Cant substract a string. Variable: {Name}");
			case SymbolModel.SymbolType.Numeric:
					Value = ConvertToNumeric(this) - ConvertToNumeric(value);
				break;
			case SymbolModel.SymbolType.Boolean:
				throw new NotImplementedException($"Cant substract a boolean. Variable: {Name}");
			case SymbolModel.SymbolType.Date:
					DateTime? date = ConvertToDate(this);

						if (date is null)
							throw new NotImplementedException($"Source date has a null value. Variable {Name}");
						else
							Value = SumSubstractDate(date ?? DateTime.Now, ConvertToString(value), false);
				break;
		}
	}

	/// <summary>
	///		Suma un valor a una fecha
	/// </summary>
	private DateTime SumSubstractDate(DateTime value, string increment, bool mustSum)
	{
		if (string.IsNullOrWhiteSpace(increment))
			throw new NotImplementedException($"Increment string is empty. Variable {Name}");
		else 
		{
			(int incrementValue, DateIncrement type) = GetDateIncrement(increment);

				// Si se debe restar, el incremento es negativo
				if (!mustSum)
					incrementValue = -1 * incrementValue;
				// Añade / resta los días, meses...
				return type switch
						{
							DateIncrement.Day => value.AddDays(incrementValue),
							DateIncrement.Week => value.AddDays(7 * incrementValue),
							DateIncrement.Month => value.AddMonths(incrementValue),
							DateIncrement.Year => value.AddYears(incrementValue),
							_ => throw new NotImplementedException($"Increment type unknown. Variable {Name}. Increment {increment}"),
						};
		}
	}

	/// <summary>
	///		Obtiene los valores de un incremento
	/// </summary>
	private (int increment, DateIncrement type) GetDateIncrement(string value)
	{
		DateIncrement type = DateIncrement.Day;

			// Obtiene el tipo de incremento
			value = value.ToUpper();
			if (value.EndsWith("W"))
				type = DateIncrement.Week;
			else if (value.EndsWith("M"))
				type = DateIncrement.Month;
			else if (value.EndsWith("Y"))
				type = DateIncrement.Year;
			// Quita el tipo de incremento
			if (value.EndsWith("D") || value.EndsWith("W") || value.EndsWith("M") || value.EndsWith("Y"))
			{
				if (value.Length > 1)
					value = value[..^1];
				else
					value = "1";
			}
			// Obtiene el incremento
			if (!int.TryParse(value, out int result))
				throw new NotImplementedException("The increment has no value");
			else
				return (result, type);
	}

	/// <summary>
	///		Comprueba si el valor de la variable es nulo
	/// </summary>
	public bool IsNull() => Value is null;

	/// <summary>
	///		Compara el valor de la variable con un valor
	/// </summary>
	public CompareResult Compare(VariableModel target)
	{
		if (target.IsNull() && IsNull())
			return CompareResult.Equals;
		else if (!target.IsNull() && IsNull())
			return CompareResult.LessThan;
		else if (target.IsNull() && !IsNull())
			return CompareResult.GreaterThan;
		else
			return Type switch
			{
				SymbolModel.SymbolType.Numeric => CompareWithNumeric(ConvertToNumeric(target)),
				SymbolModel.SymbolType.Date => CompareWithDate(ConvertToDate(target)),
				SymbolModel.SymbolType.Boolean => CompareWithBoolean(ConvertToBoolean(target)),
				SymbolModel.SymbolType.String => CompareWithString(ConvertToString(target)),
				_ => throw new NotImplementedException($"Cant compare variable {Name} with type {Type} and the value {target.Value ?? string.Empty}"),
			};
	}

	/// <summary>
	///		Compara el valor con una cadena
	/// </summary>
	private CompareResult CompareWithString(string target)
	{
		if (Type == SymbolModel.SymbolType.String)
			return ConvertToString(this).ToUpperInvariant().CompareTo(target.ToUpperInvariant()) switch
						{
							0 => CompareResult.Equals,
							1 => CompareResult.GreaterThan,
							_ => CompareResult.LessThan,
						};
		else
			throw new NotImplementedException($"Cant compare string with {Type}");
	}

	/// <summary>
	///		Compara el valor con un numérico
	/// </summary>
	private CompareResult CompareWithNumeric(double target)
	{
		if (Type == SymbolModel.SymbolType.Numeric)
		{
			double value = ConvertToNumeric(this);

				if (value == target)
					return CompareResult.Equals;
				else if (value > target)
					return CompareResult.GreaterThan;
				else
					return CompareResult.LessThan;
		}
		else
			throw new NotImplementedException($"Cant compare numeric with {Type}");
	}

	/// <summary>
	///		Compara el valor con una fecha
	/// </summary>
	private CompareResult CompareWithDate(DateTime? target)
	{
		if (Type == SymbolModel.SymbolType.Date)
		{
			DateTime? value = ConvertToDate(this);

				if (value == target)
					return CompareResult.Equals;
				else if (value > target)
					return CompareResult.GreaterThan;
				else
					return CompareResult.LessThan;
		}
		else
			throw new NotImplementedException($"Cant compare numeric with {Type}");
	}

	/// <summary>
	///		Compara un valor con un boolean
	/// </summary>
	private CompareResult CompareWithBoolean(bool target)
	{
		if (Type == SymbolModel.SymbolType.Boolean)
		{
			bool value = ConvertToBoolean(this);

				if (value == target)
					return CompareResult.Equals;
				else if (value)
					return CompareResult.GreaterThan;
				else
					return CompareResult.LessThan;
		}
		else
			throw new NotImplementedException($"Cant compare numeric with {Type}");
	}

	/// <summary>
	///		Convierte un objeto a numérico
	/// </summary>
	private double ConvertToNumeric(VariableModel variable) => Convert.ToDouble(variable.Value);

	/// <summary>
	///		Convierte un objeto a boolean
	/// </summary>
	private bool ConvertToBoolean(VariableModel variable) => Convert.ToBoolean(variable.Value);

	/// <summary>
	///		Convierte un objeto a fecha
	/// </summary>
	private DateTime? ConvertToDate(VariableModel variable)
	{
		if (variable.Value is DateTime date)
			return date;
		else
			return null;
	}

	/// <summary>
	///		Convierte un objeto a un tipo determinado
	/// </summary>
	private string ConvertToString(VariableModel variable)
	{
		switch (variable.Value)
		{
			case DateTime date:
				return $"{date:yyyy-MM-dd HH:mm:ss}";
			case double number:
				return number.ToString(System.Globalization.CultureInfo.InvariantCulture);
			case decimal number:
				return number.ToString(System.Globalization.CultureInfo.InvariantCulture);
			case int number:
				return number.ToString(System.Globalization.CultureInfo.InvariantCulture);
			case float number:
				return number.ToString(System.Globalization.CultureInfo.InvariantCulture);
			case bool boolean:
				if (boolean)
					return "true";
				else
					return "false";
			case string value:
				return value;
			default:
				return variable.Value?.ToString() ?? string.Empty;
		}
	}

	/// <summary>
	///		Nombre de la variable
	/// </summary>
	public string Name { get; }

	/// <summary>
	///		Obtiene el tipo de la variable
	/// </summary>
	public SymbolModel.SymbolType Type 
	{ 
		set { _type = value; }
		get
		{
			// Infiere el tipo si no se ha definido
			if (_type == SymbolModel.SymbolType.Unknown && Value is not null)
				_type = InferType(Value);
			// Devuelve el tipo
			return _type;
		}
	}

	/// <summary>
	///		Valor de la variable
	/// </summary>
	public object? Value { get; set; }
}
