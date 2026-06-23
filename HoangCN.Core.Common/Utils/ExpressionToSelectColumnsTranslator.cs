using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace HoangCN.Core.Common.Utils
{
    /// <summary>
    /// Bộ biên dịch chuyển đổi biểu thức Expression selector thành danh sách tên cột
    /// </summary>
    public static class ExpressionToSelectColumnsTranslator
    {
        /// <summary>
        /// Phân tích biểu thức Expression để lấy danh sách thuộc tính được chọn
        /// </summary>
        public static List<string> Translate<TEntity>(Expression<Func<TEntity, object>> selector)
        {
            if (selector == null)
            {
                throw new ArgumentNullException(nameof(selector));
            }

            var columns = new List<string>();
            var body = selector.Body;

            // Xử lý Convert node (ví dụ: x => (object)x.PropertyName)
            if (body is UnaryExpression unary && unary.NodeType == ExpressionType.Convert)
            {
                body = unary.Operand;
            }

            if (body is NewExpression newExp)
            {
                foreach (var arg in newExp.Arguments)
                {
                    columns.Add(GetPropertyName(arg));
                }
            }
            else if (body is MemberExpression)
            {
                columns.Add(GetPropertyName(body));
            }
            else
            {
                throw new ArgumentException($"Biểu thức selector '{body.GetType().Name}' chưa được hỗ trợ. Chỉ hỗ trợ kiểu nặc danh (new {{ ... }}) hoặc thuộc tính đơn lẻ.");
            }

            return columns;
        }

        private static string GetPropertyName(Expression exp)
        {
            var current = exp;
            if (current is UnaryExpression unary && unary.NodeType == ExpressionType.Convert)
            {
                current = unary.Operand;
            }

            if (current is MemberExpression memberExp && memberExp.Expression is ParameterExpression)
            {
                return memberExp.Member.Name;
            }

            throw new ArgumentException($"Biểu thức chọn không hợp lệ: {exp}. Chỉ chấp nhận truy cập thuộc tính trực tiếp của thực thể (e.g. x.PropName).");
        }
    }
}
