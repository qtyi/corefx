// Licensed to the Qtyi under one or more agreements.
// The Qtyi licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Reflection;

namespace Qtyi.CodeAnalysis.Editor.UnitTests;

public partial class LangFieldAttributeTests : TestBase
{
    protected static AttributeTargets? GetAttributeValidOn<T>() where T : Attribute => GetAttributeValidOn(typeof(T));

    protected static AttributeTargets? GetAttributeValidOn(Type enumType) => enumType.GetCustomAttribute<AttributeUsageAttribute>()!.ValidOn;

    protected static IEnumerable<string> GenerateCSharpConstructors(string attributeName, Type baseAttributeType)
    {
        foreach (var ci in baseAttributeType.GetTypeInfo().DeclaredConstructors)
        {
            if (!ci.IsFamily) continue;

            var pis = ci.GetParameters();
            var parameters = string.Join(", ", pis.Select(GenerateParameterDeclaration));
            var arguments = string.Join(", ", pis.Select(GenerateArgument));
            var ctor = $"public {attributeName}({parameters}) : base({arguments}) {{}}";

            yield return ctor;

            static string GenerateParameterDeclaration(ParameterInfo pi)
            {
                return $"{GenerateDirection(pi)}{pi.ParameterType.FullName} {pi.Name}";
            }

            static string GenerateArgument(ParameterInfo pi)
            {
                return $"{GenerateDirection(pi)}{pi.Name}";
            }

            static string GenerateDirection(ParameterInfo pi)
            {
                if (pi.ParameterType.IsByRef)
                {
                    if (pi.IsOut)
                        return "out ";
                    else
                        return "ref ";
                }
                else return string.Empty;
            }
        }
    }
}
