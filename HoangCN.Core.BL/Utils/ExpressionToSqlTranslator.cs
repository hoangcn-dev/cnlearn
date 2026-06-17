using Dapper;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace HoangCN.Core.BL.Utils
{
    /// <summary>
    /// Bộ biên dịch từ Linq Expression sang câu lệnh raw SQL và DynamicParameters cho Dapper
    /// </summary>
    public class ExpressionToSqlTranslator : ExpressionVisitor
    {
        private readonly List<string> _sqlParts = new();
        private readonly DynamicParameters _parameters = new();
        private int _paramCounter = 0;
        private readonly ParameterExpression _parameter;
        private bool _inComparison = false;

        public ExpressionToSqlTranslator(ParameterExpression parameter)
        {
            _parameter = parameter;
        }

        public static (string Sql, DynamicParameters Parameters) Translate(LambdaExpression lambda)
        {
            var translator = new ExpressionToSqlTranslator(lambda.Parameters[0]);
            var (sql, parameters) = translator.TranslateInternal(lambda.Body);
            return (sql, parameters);
        }

        private (string Sql, DynamicParameters Parameters) TranslateInternal(Expression expression)
        {
            Visit(expression);
            var sql = string.Join(" ", _sqlParts);
            return (sql, _parameters);
        }

        protected override Expression VisitBinary(BinaryExpression node)
        {
            _sqlParts.Add("(");

            bool isComparison = node.NodeType == ExpressionType.Equal ||
                                node.NodeType == ExpressionType.NotEqual ||
                                node.NodeType == ExpressionType.LessThan ||
                                node.NodeType == ExpressionType.LessThanOrEqual ||
                                node.NodeType == ExpressionType.GreaterThan ||
                                node.NodeType == ExpressionType.GreaterThanOrEqual;

            bool wasInComparison = _inComparison;
            if (isComparison)
            {
                _inComparison = true;
            }

            bool isLeftNull = IsNullExpression(node.Left);
            bool isRightNull = IsNullExpression(node.Right);

            if (isLeftNull || isRightNull)
            {
                var nonNullNode = isLeftNull ? node.Right : node.Left;
                Visit(nonNullNode);

                string nullOp = node.NodeType switch
                {
                    ExpressionType.Equal => "IS NULL",
                    ExpressionType.NotEqual => "IS NOT NULL",
                    _ => throw new NotSupportedException($"Toán tử so sánh NULL {node.NodeType} chưa được hỗ trợ.")
                };

                _sqlParts.Add(nullOp);
            }
            else
            {
                Visit(node.Left);

                string op = node.NodeType switch
                {
                    ExpressionType.Equal => "=",
                    ExpressionType.NotEqual => "!=",
                    ExpressionType.LessThan => "<",
                    ExpressionType.LessThanOrEqual => "<=",
                    ExpressionType.GreaterThan => ">",
                    ExpressionType.GreaterThanOrEqual => ">=",
                    ExpressionType.AndAlso => "AND",
                    ExpressionType.OrElse => "OR",
                    _ => throw new NotSupportedException($"Toán tử {node.NodeType} chưa được hỗ trợ.")
                };

                _sqlParts.Add(op);
                Visit(node.Right);
            }

            _inComparison = wasInComparison;
            _sqlParts.Add(")");
            return node;
        }

        protected override Expression VisitMember(MemberExpression node)
        {
            if (node.Expression == _parameter)
            {
                var tableName = _parameter.Type.Name;
                _sqlParts.Add($"`{tableName}`.`{node.Member.Name}`");

                if (node.Type == typeof(bool) && !_inComparison)
                {
                    _sqlParts.Add("= 1");
                }
                return node;
            }

            if (!HasParameter(node))
            {
                var value = Evaluate(node);
                var paramName = $"p_{_paramCounter++}";
                _parameters.Add(paramName, value);
                _sqlParts.Add($"@{paramName}");
                return node;
            }

            throw new NotSupportedException($"Truy cập Member {node.Member.Name} không được hỗ trợ.");
        }

        protected override Expression VisitConstant(ConstantExpression node)
        {
            var value = node.Value;
            if (value == null)
            {
                _sqlParts.Add("NULL");
            }
            else
            {
                var paramName = $"p_{_paramCounter++}";
                _parameters.Add(paramName, value);
                _sqlParts.Add($"@{paramName}");
            }
            return node;
        }

        protected override Expression VisitUnary(UnaryExpression node)
        {
            if (node.NodeType == ExpressionType.Not)
            {
                if (node.Operand is MemberExpression mem && mem.Expression == _parameter && mem.Type == typeof(bool))
                {
                    var tableName = _parameter.Type.Name;
                    _sqlParts.Add($"`{tableName}`.`{mem.Member.Name}` = 0");
                    return node;
                }

                _sqlParts.Add("NOT (");
                Visit(node.Operand);
                _sqlParts.Add(")");
                return node;
            }
            else if (node.NodeType == ExpressionType.Convert)
            {
                Visit(node.Operand);
                return node;
            }
            throw new NotSupportedException($"Toán tử Unary {node.NodeType} chưa được hỗ trợ.");
        }

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            if (node.Method.Name == "Contains")
            {
                Expression? listNode = null;
                Expression? itemNode = null;

                if (node.Object != null && node.Arguments.Count == 1)
                {
                    // Instance method: list.Contains(item)
                    listNode = node.Object;
                    itemNode = node.Arguments[0];
                }
                else if (node.Object == null && node.Arguments.Count == 2)
                {
                    // Extension method: Enumerable.Contains(list, item)
                    listNode = node.Arguments[0];
                    itemNode = node.Arguments[1];
                }

                if (listNode != null && itemNode != null && HasParameter(itemNode) && !HasParameter(listNode))
                {
                    Visit(itemNode);
                    _sqlParts.Add("IN");
                    var listValue = Evaluate(listNode);
                    var paramName = $"p_{_paramCounter++}";
                    _parameters.Add(paramName, listValue);
                    _sqlParts.Add($"@{paramName}");
                    return node;
                }
            }

            if (node.Method.DeclaringType == typeof(string))
            {
                if (node.Method.Name == "Contains" && node.Object != null)
                {
                    Visit(node.Object);
                    _sqlParts.Add("LIKE");
                    var value = Evaluate(node.Arguments[0]);
                    var paramName = $"p_{_paramCounter++}";
                    _parameters.Add(paramName, $"%{value}%");
                    _sqlParts.Add($"@{paramName}");
                    return node;
                }
                if (node.Method.Name == "StartsWith" && node.Object != null)
                {
                    Visit(node.Object);
                    _sqlParts.Add("LIKE");
                    var value = Evaluate(node.Arguments[0]);
                    var paramName = $"p_{_paramCounter++}";
                    _parameters.Add(paramName, $"{value}%");
                    _sqlParts.Add($"@{paramName}");
                    return node;
                }
                if (node.Method.Name == "EndsWith" && node.Object != null)
                {
                    Visit(node.Object);
                    _sqlParts.Add("LIKE");
                    var value = Evaluate(node.Arguments[0]);
                    var paramName = $"p_{_paramCounter++}";
                    _parameters.Add(paramName, $"%{value}");
                    _sqlParts.Add($"@{paramName}");
                    return node;
                }
            }

            if (!HasParameter(node))
            {
                var value = Evaluate(node);
                var paramName = $"p_{_paramCounter++}";
                _parameters.Add(paramName, value);
                _sqlParts.Add($"@{paramName}");
                return node;
            }

            throw new NotSupportedException($"Phương thức {node.Method.Name} chưa được hỗ trợ.");
        }

        private bool HasParameter(Expression node)
        {
            var detector = new ParameterDetector(_parameter);
            detector.Visit(node);
            return detector.HasParameter;
        }

        private bool IsNullExpression(Expression node)
        {
            if (node is ConstantExpression constExpr && constExpr.Value == null)
            {
                return true;
            }
            if (!HasParameter(node))
            {
                return Evaluate(node) == null;
            }
            return false;
        }

        private object? Evaluate(Expression node)
        {
            if (node == null) return null;

            if (node is ConstantExpression constExpr)
            {
                return constExpr.Value;
            }

            if (node is MemberExpression memberExpr)
            {
                var container = memberExpr.Expression != null ? Evaluate(memberExpr.Expression) : null;
                if (memberExpr.Member is FieldInfo fieldInfo)
                {
                    return fieldInfo.GetValue(container);
                }
                if (memberExpr.Member is PropertyInfo propInfo)
                {
                    return propInfo.GetValue(container);
                }
            }

            var objectMember = Expression.Convert(node, typeof(object));
            var getterLambda = Expression.Lambda<Func<object>>(objectMember);
            var getter = getterLambda.Compile();
            return getter();
        }

        private class ParameterDetector : ExpressionVisitor
        {
            private readonly ParameterExpression _parameterToFind;
            public bool HasParameter { get; private set; }

            public ParameterDetector(ParameterExpression parameterToFind)
            {
                _parameterToFind = parameterToFind;
            }

            protected override Expression VisitParameter(ParameterExpression node)
            {
                if (node == _parameterToFind)
                {
                    HasParameter = true;
                }
                return base.VisitParameter(node);
            }
        }
    }
}
