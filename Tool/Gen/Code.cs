namespace Gen;

using Microsoft.CSharp;
using System.CodeDom;
using System.CodeDom.Compiler;

internal static class Code
{
    internal static CodeTypeDeclaration ConvertForEnum(EnumDecl def)
    {
        var cls = new CodeTypeDeclaration(Utility.ToClassName(def.Name))
        {
            IsEnum = true,
        };

        if (Code.IsPower2ValueOnly(def.Values.Select(v => v.Value)))
        {
            cls.CustomAttributes.Add(new CodeAttributeDeclaration(typeof(FlagsAttribute).FullName!));
        }

        foreach (var value in def.Values)
        {
            var field = new CodeMemberField(typeof(int), Utility.ToPropertyName(value.Key))
            {
                InitExpression = new CodePrimitiveExpression(value.Value),
            };
            cls.Members.Add(field);
        }

        return cls;
    }

    internal static void WriteFile(string path, CodeNamespace ns)
    {
        var compileUnit = new CodeCompileUnit();
        compileUnit.Namespaces.Add(ns);

        var provider = new CSharpCodeProvider();

        using var stream = File.OpenWrite(path);
        using var writer = new StreamWriter(stream, leaveOpen: true);
        provider.GenerateCodeFromCompileUnit(compileUnit, writer, new CodeGeneratorOptions());
        writer.Flush();
    }

    private static bool IsPower2ValueOnly(IEnumerable<int> values)
    {
        foreach (var value in values)
        {
            if (value == 0)
            {
                continue;
            }

            if ((value & (value - 1)) != 0)
            {
                return false;
            }
        }

        return true;
    }
}
