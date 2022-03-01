﻿using MiniLanguageCompiler.Core.Models;
using MiniLanguageCompiler.Core.Types;

namespace MiniLanguageCompiler.Core.Expressions
{
    public class ConstantExpression : TypedExpression
    {
        public ConstantExpression(Type type, Token token)
            : base(type, token)
        {
        }

        public override Type GetExpressionType()
        {
            return type;
        }
    }
}
