namespace Bau.Libraries.LibInterpreter.Lexer.Rules;

/// <summary>
///		Regla con los datos de una palabra con longitud fija (utilizando o no los separadores)
/// </summary>
public class RuleWordFixed(string tokenType, string word, bool toSeparator) : RuleBase(tokenType, true, true)
{
	/// <summary>
	///		Palabra clave
	/// </summary>
	public string Word { get; set; } = word;

	/// <summary>
	///		Indica si se debe buscar hasta un separador
	/// </summary>
	public bool ToSeparator { get; } = toSeparator;
}
