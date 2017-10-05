/*

    Scanner + Parser for the following simple expression language:

    Expr -> Expr "+" Term
    Expr -> Term
    Term -> Term "*" Fact
    Term -> Fact
    Fact -> Int
    Fact -> "(" Expr ")"

    Converted to LL(1):

    Prog -> Expr Eof
    Expr -> Term ("+" Term)*
    Term -> Fact ("*" Fact)*
    Fact -> Int | "(" Expr ")"

*/

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public enum TokenCategory {
    PLUS, TIMES, PAR_OPEN, PAR_CLOSE, INT, EOF, ILLEGAL
}

public class Token {
    public TokenCategory Category { get; }
    public String Lexeme { get; }
    public Token(TokenCategory category, String lexeme) {
        Category = category;
        Lexeme = lexeme;
    }
    public override String ToString() {
        return String.Format("[{0}, \"{1}\"]", Category, Lexeme);
    }
}

public class Scanner {
    readonly String input;
    static readonly Regex regex = new Regex(@"([+])|([*])|([(])|([)])|(\d+)|(\s)|(.)");
    public Scanner(String input) {
        this.input = input;
    }
    public IEnumerable<Token> Start() {
        foreach (Match m in regex.Matches(input)) {
            if (m.Groups[1].Success) {
                yield return new Token(TokenCategory.PLUS, m.Value);
            } else if (m.Groups[2].Success) {
                yield return new Token(TokenCategory.TIMES, m.Value);
            } else if (m.Groups[3].Success) {
                yield return new Token(TokenCategory.PAR_OPEN, m.Value);
            } else if (m.Groups[4].Success) {
                yield return new Token(TokenCategory.PAR_CLOSE, m.Value);
            } else if (m.Groups[5].Success) {
                yield return new Token(TokenCategory.INT, m.Value);
            } else if (m.Groups[6].Success) {
                continue;
            } else if (m.Groups[7].Success) {
                yield return new Token(TokenCategory.ILLEGAL, m.Value);
            }
        }
        yield return new Token(TokenCategory.EOF, "");
    }
}

class SyntaxError: Exception {
}

public class Parser {
    IEnumerator<Token> tokenStream;
    public Parser(IEnumerator<Token> tokenStream) {
        this.tokenStream = tokenStream;
        this.tokenStream.MoveNext();
    }
    public TokenCategory Current {
        get { return tokenStream.Current.Category; }
    }
    public Token Expect(TokenCategory category) {
        if (Current == category) {
            Token current = tokenStream.Current;
            tokenStream.MoveNext();
            return current;
        } else {
            throw new SyntaxError();
        }
    }
    public int Prog() {
        var result = Expr();
        Expect(TokenCategory.EOF);
        return result;
    }

    public int Expr() {
        var result = Term();
        while (Current == TokenCategory.PLUS) {
            Expect(TokenCategory.PLUS);
            result += Term();
        }
        return result;
    }

    public int Term() {
        var result = Fact();
        while (Current == TokenCategory.TIMES) {
            Expect(TokenCategory.TIMES);
            result *= Fact();
        }
        return result;
    }

    public int Fact() {
        switch (Current) {

        case TokenCategory.INT:
            var token = Expect(TokenCategory.INT);
            return Convert.ToInt32(token.Lexeme);

        case TokenCategory.PAR_OPEN:
            Expect(TokenCategory.PAR_OPEN);
            var result = Expr();
            Expect(TokenCategory.PAR_CLOSE);
            return result;    
            
        default:
            throw new SyntaxError();
        }
    }
}

public class SimpleExpression {
    public static void Main() {
        var line = Console.ReadLine();
        var parser = new Parser(new Scanner(line).Start().GetEnumerator());
        try {
            var result = parser.Prog();
            Console.WriteLine(result);
        } catch (SyntaxError) {
            Console.Error.WriteLine("Found syntax error!");
        }
    }
}