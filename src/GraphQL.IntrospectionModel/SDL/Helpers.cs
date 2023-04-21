using System.Globalization;
using System.Text;
using GraphQLParser.Visitors;

namespace GraphQL.IntrospectionModel.SDL;

/// <summary>
/// Extension methods for SDL printing.
/// </summary>
public static class Helpers
{
    // frequently reused object
    private static StringBuilder? _reusableStringBuilder;

    // https://graphql.github.io/graphql-spec/October2021/#sec-String-Value
    // TODO: can be changed to stackalloc / string.Create()
    internal static string BuildEscapedString(string source)
    {
        var sb = Interlocked.Exchange(ref _reusableStringBuilder, null) ?? new StringBuilder(source.Length + 2);

        try
        {
            sb.Append('"');
            foreach (char ch in source)
            {
                if (ch < ' ')
                {
                    if (ch == '\b')
                        sb.Append("\\b");
                    else if (ch == '\f')
                        sb.Append("\\f");
                    else if (ch == '\n')
                        sb.Append("\\n");
                    else if (ch == '\r')
                        sb.Append("\\r");
                    else if (ch == '\t')
                        sb.Append("\\t");
                    else
                        sb.Append("\\u" + ((int)ch).ToString("X4", CultureInfo.InvariantCulture));
                }
                else if (ch == '\\')
                {
                    sb.Append("\\\\");
                }
                else if (ch == '"')
                {
                    sb.Append("\\\"");
                }
                else
                {
                    sb.Append(ch);
                }
            }

            sb.Append('"');
            return sb.ToString();
        }
        finally
        {
            sb.Clear();
            _ = Interlocked.CompareExchange(ref _reusableStringBuilder, sb, null);
        }
    }

    internal static List<T> AddIfNotNull<T>(this List<T> list, T? value)
        where T : class
    {
        if (value != null)
            list.Add(value);
        return list;
    }

    /// <summary>
    /// Prints the SDL schema using the GraphQL schema from introspection request using the specified options.
    /// </summary>
    public static string Print(this GraphQLSchema schema, ASTConverterOptions? options = null)
    {
        var converter = new ASTConverter(options ??= new());
        var document = converter.ToDocument(schema);
        var printer = new SDLPrinter(options);
        return printer.Print(document);
    }
}
