using System.Collections.Generic;
using Xunit;
using FluentAssertions;

using Bau.Libraries.LibInterpreter.Lexer;
using Bau.Libraries.LibInterpreter.Lexer.Builder;
using Bau.Libraries.LibInterpreter.Lexer.Rules;
using Bau.Libraries.LibInterpreter.Models.Tokens;

namespace Test_Lexer;

/// <summary>
///		Pruebas para <see cref="LexerManager"/>
/// </summary>
public class Lexer_should
{
	// Constantes privadas
	private const string TokenVariable = "variable";
	private const string TokenNumber = "number";
	private const string TokenString = "string";
	private const string TokenAnd = "and";
	private const string TokenOr = "or";
	private const string TokenNot = "not";
	private const string TokenContains = "contains";
	private const string TokenOpenParenthesis = "openparenthesis";
	private const string TokenCloseParenthesis = "closeparenthesis";
	private const string TokenEquals = "equals";
	private const string TokenDistinct = "distinct";

	/// <summary>
	///		Prueba de generación de reglas
	/// </summary>
	[Fact]
	public void build_rules()
	{
		LexerManager lexer = new();
		TokenCollection tokens;

			// Genera las reglas
			lexer.Rules.AddRange(SeedRules());
			// Interpreta una cadena
			tokens = lexer.Parse("({{Company}} = 'Company1' and {{Application}} = 'FM') OR {{Company}} != 'Company2' AND 1 != 2.3");
			// Comprueba los datos
			tokens.Count.Should().BeGreaterThan(0);
			tokens[0].Type.Should().BeEquivalentTo(TokenOpenParenthesis);
			tokens[1].Type.Should().BeEquivalentTo(TokenVariable);
			tokens[1].Value.Should().Be("Company");
			tokens[2].Type.Should().BeEquivalentTo(TokenEquals);
			tokens[3].Type.Should().BeEquivalentTo(TokenString);
			tokens[3].Value.Should().Be("Company1");
			tokens[4].Type.Should().BeEquivalentTo(TokenAnd);
			tokens[5].Type.Should().BeEquivalentTo(TokenVariable);
			tokens[5].Value.Should().Be("Application");
			tokens[6].Type.Should().BeEquivalentTo(TokenEquals);
			tokens[7].Type.Should().BeEquivalentTo(TokenString);
			tokens[7].Value.Should().Be("FM");
			tokens[8].Type.Should().BeEquivalentTo(TokenCloseParenthesis);
			tokens[9].Type.Should().BeEquivalentTo(TokenOr);
			tokens[10].Type.Should().BeEquivalentTo(TokenVariable);
			tokens[10].Value.Should().Be("Company");
			tokens[11].Type.Should().BeEquivalentTo(TokenDistinct);
			tokens[12].Type.Should().BeEquivalentTo(TokenString);
			tokens[12].Value.Should().Be("Company2");
			tokens[13].Type.Should().BeEquivalentTo(TokenAnd);
			tokens[14].Type.Should().BeEquivalentTo(TokenNumber);
			tokens[14].Value.Should().Be("1");
			tokens[15].Type.Should().BeEquivalentTo(TokenDistinct);
			tokens[16].Type.Should().BeEquivalentTo(TokenNumber);
			tokens[16].Value.Should().Be("2.3");
	}

	/// <summary>
	///		Genera las reglas del analizador
	/// </summary>
	private List<RuleBase> SeedRules()
	{
		RuleBuilder builder = new();

			// Añade las reglas
			builder.WithDelimited(TokenVariable, "{{", "}}")
				   .WithDelimited(TokenString, "'", "'")
				   .WithWords(TokenAnd, "and")
				   .WithWords(TokenOr, "or")
				   .WithWords(TokenNot, "not")
				   .WithWords(TokenContains, "contains")
				   .WithWords(TokenOpenParenthesis, "(")
				   .WithWords(TokenCloseParenthesis, ")")
				   .WithWords(TokenEquals, "=")
				   .WithWords(TokenDistinct, "!=")
				   .WithDefaultNumbers(TokenNumber);
			// Genera las reglas
			return builder.Build();
	}
}