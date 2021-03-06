/*

    Scanner + Parser for the following simple expression language:

    Expr -> Expr "+" Term    // "+" has left associativity
    Expr -> Term
    Term -> Term "*" Pow     // "*" has left associativity
    Term -> Pow
    Pow  -> Fact "^" Pow     // "^" has right associativity
    Pow  -> Fact
    Fact -> Int
    Fact -> "(" Expr ")"

    Converted to LL(1):

    Prog -> Expr Eof
    Expr -> Term ("+" Term)*
    Term -> Pow ("*" Pow)*
    Pow  -> Fact ("^" Pow)?
    Fact -> Int | "(" Expr ")"

*/

using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

public enum TokenCategory {
    PLUS, TIMES, POW, PAR_OPEN, PAR_CLOSE, INT, EOF, ILLEGAL
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
    static readonly Regex regex = new Regex(@"([+])|([*])|([(])|([)])|(\d+)|(\s)|(\^)|(.)");
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
                yield return new Token(TokenCategory.POW, m.Value);
            } else if (m.Groups[8].Success) {
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

    public Node Prog() {
        var r = Expr();
        Expect(TokenCategory.EOF);
        return new Prog() { r };
    }

    public Node Expr() {
        var r = Term();
        while (Current == TokenCategory.PLUS) {
            var p = new Plus() {
                AnchorToken = Expect(TokenCategory.PLUS)
            };
            p.Add(r);
            p.Add(Term());
            r = p;
        }
        return r;
    }

    public Node Term() {
        var r = Pow();
        while (Current == TokenCategory.TIMES) {
            var p = new Times() {
                AnchorToken = Expect(TokenCategory.TIMES)
            };
            p.Add(r);
            p.Add(Pow());
            r = p;
        }
        return r;
    }

    public Node Pow() {
        var r = Fact();
        if (Current == TokenCategory.POW) {
            var p = new Pow() {
                AnchorToken = Expect(TokenCategory.POW)
            };
            p.Add(r);
            p.Add(Pow());
            r = p;
        }
        return r;
    }

    public Node Fact() {
        switch (Current) {

        case TokenCategory.INT:
            return new Int() {
                AnchorToken = Expect(TokenCategory.INT)
            };

        case TokenCategory.PAR_OPEN:
            Expect(TokenCategory.PAR_OPEN);
            var r = Expr();
            Expect(TokenCategory.PAR_CLOSE);
            return r;

        default:
            throw new SyntaxError();
        }
    }
}

public class Node: IEnumerable<Node> {

    IList<Node> children = new List<Node>();

    public Node this[int index] {
        get {
            return children[index];
        }
    }

    public Token AnchorToken { get; set; }

    public void Add(Node node) {
        children.Add(node);
    }

    public IEnumerator<Node> GetEnumerator() {
        return children.GetEnumerator();
    }

    System.Collections.IEnumerator
    System.Collections.IEnumerable.GetEnumerator() {
        throw new NotImplementedException();
    }

    public override string ToString() {
        return String.Format("{0} {1}", GetType().Name, AnchorToken);
    }

    public string ToStringTree() {
        var sb = new StringBuilder();
        TreeTraversal(this, "", sb);
        return sb.ToString();
    }

    static void TreeTraversal(Node node, string indent, StringBuilder sb) {
        sb.Append(indent);
        sb.Append(node);
        sb.Append('\n');
        foreach (var child in node.children) {
            TreeTraversal(child, indent + "  ", sb);
        }
    }
}

public class Prog : Node {}
public class Plus : Node {}
public class Times : Node {}
public class Pow : Node {}
public class Int : Node {}

public class EvalVisitor {

    public int Visit(Prog node) {
        return Visit((dynamic) node[0]);
    }

    public int Visit(Plus node) {
        return 
            Visit((dynamic) node[0]) +
            Visit((dynamic) node[1]);
    }

    public int Visit(Times node) {
        return 
            Visit((dynamic) node[0]) *
            Visit((dynamic) node[1]);
    }

    public int Visit(Pow node) {
        return 
            (int) Math.Pow(
                    Visit((dynamic) node[0]),
                    Visit((dynamic) node[1]));
    }

    public int Visit(Int node) {
        return Convert.ToInt32(node.AnchorToken.Lexeme);
    }
    
}

public class SExpressionVisitor {
    
    public String Visit(Prog node) {
        return Visit((dynamic) node[0]);
    }

    public String Visit(Plus node) {
        return "(+ " + Visit((dynamic) node[0]) +
               " " + Visit((dynamic) node[1]) + ")";
    }

    public String Visit(Times node) {
        return "(* " + Visit((dynamic) node[0]) +
            " " + Visit((dynamic) node[1]) + ")";
    }

    public String Visit(Pow node) {
        return "(expt " + Visit((dynamic) node[0]) +
            " " + Visit((dynamic) node[1]) + ")";
    }

    public String Visit(Int node) {
        return node.AnchorToken.Lexeme;
    }
}

public class CVisitor {
    
    public String Visit(Prog node) {
        return @"
#include <stdio.h>
#include <math.h>

int main(void) {
    printf(""%d\n"", " + Visit((dynamic) node[0]) + @");
    return 0;
}";
    }

    public String Visit(Plus node) {
        return 
            "(" + Visit((dynamic) node[0]) +
            "+" + Visit((dynamic) node[1]) + ")";
    }

    public String Visit(Times node) {
        return 
            "(" + Visit((dynamic) node[0]) +
            "*" + Visit((dynamic) node[1]) + ")";
    }

    public String Visit(Pow node) {
        return 
            "(int) pow(" + Visit((dynamic) node[0]) +
            "," + Visit((dynamic) node[1]) + ")";
    }

    public String Visit(Int node) {
        return node.AnchorToken.Lexeme;
    }
}

public class SimpleExpression {
    public static void Main() {
        Console.Write("> ");
        var line = Console.ReadLine();
        var parser = new Parser(new Scanner(line).Start().GetEnumerator());
        try {
            var root = parser.Prog();
            // Console.WriteLine(root.ToStringTree());
            Console.WriteLine(new EvalVisitor().Visit((dynamic) root));
            Console.WriteLine(new SExpressionVisitor().Visit((dynamic) root));
            Console.WriteLine();
            Console.WriteLine(new CVisitor().Visit((dynamic) root));

        } catch (SyntaxError) {
            Console.Error.WriteLine("Found syntax error!");
        }
    }
}