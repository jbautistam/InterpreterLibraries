using Bau.Libraries.LibInterpreter.Models.Symbols;

namespace Bau.Libraries.LibInterpreter.Interpreter.Context.Functions;

/// <summary>
///		Función implícita (definida por el intérprete) 
/// </summary>
public class ImplicitFunctionModel(SymbolModel definition, List<SymbolModel> arguments) : BaseFunctionModel(definition, arguments)
{
}
