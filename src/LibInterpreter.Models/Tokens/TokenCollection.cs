namespace Bau.Libraries.LibInterpreter.Models.Tokens;

/// <summary>
///		Colección de <see cref="TokenBase"/>
/// </summary>
public class TokenCollection : List<TokenBase>
{
	/// <summary>
	///		Añade una palabra a la colección
	/// </summary>
	public void Add(string type, int row, int column, string value)
	{
		Add(new TokenBase(type, row, column, value));
	}

	/// <summary>
	///		Crea un texto de depuración
	/// </summary>
	public string GetDebugInfo()
	{
		System.Text.StringBuilder builder = new();

			// Añade la información a la cadena
			foreach (TokenBase token in this)
				builder.AppendLine(token.GetDebugInfo());
			// Devuelve la cadena
			return builder.ToString();
	}
}
