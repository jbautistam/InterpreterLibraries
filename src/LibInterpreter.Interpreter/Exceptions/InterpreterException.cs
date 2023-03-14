using System;
using System.Runtime.Serialization;

namespace Bau.Libraries.LibInterpreter.Interpreter.Exceptions;

/// <summary>
///		Excepción asociada al intérprete
/// </summary>
public class InterpreterException : Exception
{
	public InterpreterException() {}

	public InterpreterException(string? message) : base(message) {}

	public InterpreterException(string? message, Exception? innerException) : base(message, innerException) {}

	protected InterpreterException(SerializationInfo info, StreamingContext context) : base(info, context) {}
}
