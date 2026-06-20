using HoangCN.Core.Common.Enums;
using HoangCN.Core.Common.Model.Requests;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace HoangCN.Core.BL.Utils
{
    /// <summary>
    /// Bộ dịch từ C# LINQ Expression sang AdvancedFilterGroup
    /// </summary>
    public static class ExpressionToFilterTranslator
    {
        public static AdvancedFilterGroup Translate<TEntity>(Expression<Func<TEntity, bool>> expression)
        {
            if (expression == null) return new AdvancedFilterGroup();
            return TranslateExpression(expression.Body, expression.Parameters[0]);
        }

        private static AdvancedFilterGroup TranslateExpression(Expression node, ParameterExpression parameter)
        {
            // 1. Xử lý các phép logic AND / OR
            if (node is BinaryExpression binary && (node.NodeType == ExpressionType.AndAlso || node.NodeType == ExpressionType.OrElse))
            {
                var groupType = node.NodeType == ExpressionType.AndAlso ? FilterGroupType.And : FilterGroupType.Or;
                var leftGroup = TranslateExpression(binary.Left, parameter);
                var rightGroup = TranslateExpression(binary.Right, parameter);

                return new AdvancedFilterGroup
                {
                    GroupType = groupType,
                    Groups = new List<AdvancedFilterGroup> { leftGroup, rightGroup }
                };
            }

            // 2. Xử lý so sánh bằng / khác / lớn / bé (Binary)
            if (node is BinaryExpression binaryCompare)
            {
                var op = binaryCompare.NodeType switch
                {
                    ExpressionType.Equal => FilterOperator.Equal,
                    ExpressionType.NotEqual => FilterOperator.NotEqual,
                    ExpressionType.LessThan => FilterOperator.LessThan,
                    ExpressionType.LessThanOrEqual => FilterOperator.LessThanOrEqual,
                    ExpressionType.GreaterThan => FilterOperator.GreaterThan,
                    ExpressionType.GreaterThanOrEqual => FilterOperator.GreaterThanOrEqual,
                    _ => throw new NotSupportedException($"Toán tử so sánh '{binaryCompare.NodeType}' chưa được hỗ trợ.")
                };

                // Trường hợp 2.1: Cả hai vế đều là thuộc tính của Entity (So sánh cột với cột)
                if (TryGetMemberExpression(binaryCompare.Left, parameter, out var leftProp, out var leftType) &&
                    TryGetMemberExpression(binaryCompare.Right, parameter, out var rightProp, out var rightType))
                {
                    var filterType = GetFilterType(leftType!);
                    return new AdvancedFilterGroup
                    {
                        Filters = new List<Filter>
                        {
                            new Filter(leftProp!, op, null, filterType, rightProp!)
                        }
                    };
                }

                // Trường hợp 2.2: Vế trái là cột, vế phải là giá trị
                if (TryGetMemberExpression(binaryCompare.Left, parameter, out var propName, out var propType))
                {
                    var val = Evaluate(binaryCompare.Right);
                    return CreateFilterGroup(propName!, op, val, propType!);
                }

                // Trường hợp 2.3: Vế phải là cột, vế trái là giá trị (đảo chiều toán tử)
                if (TryGetMemberExpression(binaryCompare.Right, parameter, out propName, out propType))
                {
                    var flippedOp = op switch
                    {
                        FilterOperator.Equal => FilterOperator.Equal,
                        FilterOperator.NotEqual => FilterOperator.NotEqual,
                        FilterOperator.LessThan => FilterOperator.GreaterThan,
                        FilterOperator.LessThanOrEqual => FilterOperator.GreaterThanOrEqual,
                        FilterOperator.GreaterThan => FilterOperator.LessThan,
                        FilterOperator.GreaterThanOrEqual => FilterOperator.LessThanOrEqual,
                        _ => op
                    };
                    var val = Evaluate(binaryCompare.Left);
                    return CreateFilterGroup(propName!, flippedOp, val, propType!);
                }
            }

            // 3. Xử lý phép phủ định (Unary NOT) - Thường là !u.IsActive
            if (node is UnaryExpression unary && unary.NodeType == ExpressionType.Not)
            {
                if (TryGetMemberExpression(unary.Operand, parameter, out var propName, out var propType) && propType == typeof(bool))
                {
                    return CreateFilterGroup(propName!, FilterOperator.Equal, false, propType!);
                }
                
                // Hỗ trợ phủ định biểu thức con phức tạp bằng cách đổi nhóm
                var innerGroup = TranslateExpression(unary.Operand, parameter);
                throw new NotSupportedException("Chưa hỗ trợ phủ định biểu thức logic phức tạp.");
            }

            // Hỗ trợ Convert (ví dụ: ép kiểu)
            if (node is UnaryExpression unaryConvert && unaryConvert.NodeType == ExpressionType.Convert)
            {
                return TranslateExpression(unaryConvert.Operand, parameter);
            }

            // 4. Xử lý thuộc tính Bool trực tiếp (ví dụ: u => u.IsActive)
            if (TryGetMemberExpression(node, parameter, out var boolProp, out var boolPropType) && boolPropType == typeof(bool))
            {
                return CreateFilterGroup(boolProp!, FilterOperator.Equal, true, boolPropType!);
            }

            // 5. Xử lý gọi phương thức (Contains, StartsWith, EndsWith)
            if (node is MethodCallExpression methodCall)
            {
                if (methodCall.Method.DeclaringType == typeof(string))
                {
                    var op = methodCall.Method.Name switch
                    {
                        "Contains" => FilterOperator.Contain,
                        "StartsWith" => FilterOperator.StartWith,
                        "EndsWith" => FilterOperator.EndWith,
                        _ => throw new NotSupportedException($"Phương thức string.{methodCall.Method.Name} chưa được hỗ trợ.")
                    };

                    if (methodCall.Object != null && TryGetMemberExpression(methodCall.Object, parameter, out var propName, out var propType))
                    {
                        var val = Evaluate(methodCall.Arguments[0]);
                        return CreateFilterGroup(propName!, op, val, propType!);
                    }
                }
                else if (methodCall.Method.Name == "Contains")
                {
                    Expression? listNode = null;
                    Expression? itemNode = null;

                    if (methodCall.Object != null && methodCall.Arguments.Count == 1)
                    {
                        listNode = methodCall.Object;
                        itemNode = methodCall.Arguments[0];
                    }
                    else if (methodCall.Object == null && methodCall.Arguments.Count == 2)
                    {
                        listNode = methodCall.Arguments[0];
                        itemNode = methodCall.Arguments[1];
                    }

                    if (listNode != null && itemNode != null && TryGetMemberExpression(itemNode, parameter, out var propName, out var propType))
                    {
                        var listValue = Evaluate(listNode);
                        return CreateFilterGroup(propName!, FilterOperator.In, listValue, propType!);
                    }
                }
            }

            throw new NotSupportedException($"Biểu thức kiểu '{node.GetType().Name}' chưa được hỗ trợ dịch tự động.");
        }

        private static bool TryGetMemberExpression(Expression node, ParameterExpression parameter, out string? propertyName, out Type? propertyType)
        {
            propertyName = null;
            propertyType = null;

            if (node is UnaryExpression unary && unary.NodeType == ExpressionType.Convert)
            {
                node = unary.Operand;
            }

            if (node is MemberExpression member && member.Expression == parameter)
            {
                propertyName = member.Member.Name;
                propertyType = member.Type;
                return true;
            }

            return false;
        }

        private static FilterType GetFilterType(Type type)
        {
            var underlyingType = Nullable.GetUnderlyingType(type) ?? type;

            if (underlyingType == typeof(bool))
            {
                return FilterType.Bool;
            }
            if (underlyingType == typeof(int) || underlyingType == typeof(long) ||
                underlyingType == typeof(double) || underlyingType == typeof(float) ||
                underlyingType == typeof(decimal) || underlyingType == typeof(short) ||
                underlyingType == typeof(byte))
            {
                return FilterType.Number;
            }
            if (underlyingType == typeof(DateTime))
            {
                return FilterType.Date;
            }
            return FilterType.String;
        }

        private static AdvancedFilterGroup CreateFilterGroup(string propertyName, FilterOperator op, object? value, Type propertyType)
        {
            var filterType = GetFilterType(propertyType);
            return new AdvancedFilterGroup
            {
                Filters = new List<Filter>
                {
                    new Filter(propertyName, op, value, filterType)
                }
            };
        }

        private static object? Evaluate(Expression? node)
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

            if (node is UnaryExpression unaryExpr && unaryExpr.NodeType == ExpressionType.Convert)
            {
                return Evaluate(unaryExpr.Operand);
            }

            if (node is MethodCallExpression methodCallExpr)
            {
                var obj = methodCallExpr.Object != null ? Evaluate(methodCallExpr.Object) : null;
                var args = methodCallExpr.Arguments.Select(Evaluate).ToArray();
                return methodCallExpr.Method.Invoke(obj, args);
            }

            throw new NotSupportedException($"Biểu thức loại '{node.GetType().Name}' không được hỗ trợ tính toán trực tiếp.");
        }
    }
}
