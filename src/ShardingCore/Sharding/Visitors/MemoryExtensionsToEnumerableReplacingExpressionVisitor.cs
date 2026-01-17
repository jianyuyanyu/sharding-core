using System;
using System.Linq;
using System.Linq.Expressions;

namespace ShardingCore.Sharding.Visitors
{
    /// <summary>
    /// Expression visitor that replaces MemoryExtensions.Contains calls with Enumerable.Contains.
    /// This is needed because .NET 6+ C# compiler may optimize array.Contains() to use
    /// MemoryExtensions.Contains with ReadOnlySpan{T}, which EF Core cannot evaluate
    /// because ReadOnlySpan{T} is a ref struct that cannot be boxed.
    /// </summary>
    internal class MemoryExtensionsToEnumerableReplacingExpressionVisitor : ExpressionVisitor
    {
        private static readonly Type MemoryExtensionsType = typeof(MemoryExtensions);

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            // Check if this is a MemoryExtensions.Contains call
            if (node.Method.DeclaringType == MemoryExtensionsType &&
                node.Method.Name == nameof(MemoryExtensions.Contains))
            {
                // Try to convert to Enumerable.Contains
                var converted = TryConvertToEnumerableContains(node);
                if (converted != null)
                {
                    return converted;
                }
            }

            return base.VisitMethodCall(node);
        }

        private Expression TryConvertToEnumerableContains(MethodCallExpression node)
        {
            // MemoryExtensions.Contains has the signature: Contains<T>(ReadOnlySpan<T>, T)
            // We need to find the underlying array and convert to Enumerable.Contains<T>(IEnumerable<T>, T)

            if (node.Arguments.Count < 2)
                return null;

            // First argument is the span (which might be an implicit conversion from array)
            var spanArg = node.Arguments[0];
            var valueArg = node.Arguments[1];

            // Try to get the underlying array from the span argument
            var arrayExpr = TryGetUnderlyingArray(spanArg);
            if (arrayExpr == null)
                return null;

            // Get the element type
            var elementType = arrayExpr.Type.GetElementType();
            if (elementType == null)
            {
                // Try to get from generic type
                if (arrayExpr.Type.IsGenericType)
                {
                    elementType = arrayExpr.Type.GetGenericArguments().FirstOrDefault();
                }
            }

            if (elementType == null)
                return null;

            // Visit the value argument to handle any nested MemoryExtensions calls
            var visitedValueArg = Visit(valueArg);

            // Get the Enumerable.Contains<T> method
            var containsMethod = typeof(Enumerable)
                .GetMethods()
                .FirstOrDefault(m => m.Name == nameof(Enumerable.Contains) && m.GetParameters().Length == 2)?
                .MakeGenericMethod(elementType);

            if (containsMethod == null)
                return null;

            // Create the Enumerable.Contains call
            return Expression.Call(containsMethod, arrayExpr, visitedValueArg);
        }

        private Expression TryGetUnderlyingArray(Expression spanExpr)
        {
            // Handle implicit conversion: op_Implicit(array) -> ReadOnlySpan<T>
            if (spanExpr is MethodCallExpression methodCall &&
                methodCall.Method.Name == "op_Implicit" &&
                methodCall.Arguments.Count == 1)
            {
                return Visit(methodCall.Arguments[0]);
            }

            // Handle direct NewArrayExpression wrapped in conversion
            if (spanExpr is UnaryExpression unary &&
                unary.NodeType == ExpressionType.Convert)
            {
                var operand = unary.Operand;
                if (operand.Type.IsArray)
                {
                    return Visit(operand);
                }
            }

            return null;
        }
    }
}
