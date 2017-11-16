all: simple_expression.exe int64lib.dll
	
simple_expression.exe : simple_expression.cs
	mcs simple_expression.cs

int64lib.dll : int64lib.cs
	mcs /t:library int64lib.cs
