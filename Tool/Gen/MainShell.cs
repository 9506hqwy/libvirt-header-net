namespace Gen;

using ClangSharp;
using ClangSharp.Interop;
using System.CodeDom;
using static ClangSharp.Interop.CXCursorKind;
using static ClangSharp.Interop.CXTranslationUnit_Flags;

internal class MainShell
{
    internal static void Main(string[] args)
    {
        try
        {
            var shell = new MainShell();
            shell.Work(args);
        }
        catch (Exception e)
        {
            Console.Error.WriteLine("{0}", e);
        }
    }

    private void Work(string[] args)
    {
        using var index = CXIndex.Create();
        var handle = CXTranslationUnit.Parse(
            index,
            args[0],
            args.Skip(1).ToArray(),
            Array.Empty<CXUnsavedFile>(),
            CXTranslationUnit_None);

        if (handle.NumDiagnostics > 0)
        {
            var diag = handle.GetDiagnostic(0);
            throw new Exception($"{diag}");
        }

        var tu = TranslationUnit.GetOrCreate(handle);

        var results = new List<EnumDecl>();
        foreach (var cursor in tu.TranslationUnitDecl.CursorChildren
            .Where(c => c.CursorKind == CXCursor_TypedefDecl))
        {
            this.Collect(cursor, results);
        }

        var ns = new CodeNamespace("Libvirt.Header");
        results.Select(r => Code.ConvertForEnum(r)).ToList().ForEach(t => ns.Types.Add(t));

        Code.WriteFile("Generated.cs", ns);
    }

    private void Collect(Cursor cursor, List<EnumDecl> results)
    {
        foreach (var child in cursor.CursorChildren
            .Where(c => c.CursorKind == CXCursor_EnumDecl))
        {
            var result = new EnumDecl(cursor.Spelling);

            foreach (var cnt in child.CursorChildren
                .Where(c => c.CursorKind == CXCursor_EnumConstantDecl))
            {
                result.Add(cnt.Spelling, (int)cnt.Handle.EnumConstantDeclValue);
            }

            results.Add(result);
        }
    }
}
