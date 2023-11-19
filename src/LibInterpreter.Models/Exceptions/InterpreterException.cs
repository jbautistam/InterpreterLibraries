using System.Runtime.Serialization;

namespace Bau.Libraries.LibInterpreter.Models.Exceptions;

/// <summary>
///		Excepción asociada al intérprete
/// </summary>
public class InterpreterException : Exception
{
	/// <inheritdoc/>
	public InterpreterException() { }

	/// <inheritdoc/>
	public InterpreterException(string message) : base(message) { }

	/// <inheritdoc/>
	public InterpreterException(string message, Exception innerException) : base(message, innerException) { }

	/// <inheritdoc/>
	protected InterpreterException(SerializationInfo info, StreamingContext context) : base(info, context) { }
}
