﻿using System;
namespace MiniLanguageCompiler.Core
{
    public enum TokenType
    {
       Plus,
       Minus,
       Equal,
       Colon,
       Semicolon,
       LogicalAnd,
       LogicalOr,
       LeftParens,
       RightParens,
       Multiplication,
       Division,
       GreaterThan,
       LessThan,
       GreaterOrEqualThan,
       LessOrEqualThan,
       LeftBracket,
       RightBracket,
       LeftBrace,
       RightBrace,
       Comma,
       IfKeyword,
       WhileKeyword,
       ElseKeyword,
       NumberKeyword,
       Identifier,
       StringKeyword,
    }
}
