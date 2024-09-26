using System.CodeDom.Compiler;
using System.Reflection;
using System.Runtime;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace RmMiddleware.Pages;

public class IndexModel : PageModel
{
    private readonly IHostApplicationLifetime _hostApplicationLifetime;
    private readonly ApplicationPartManager _partManager;

    public IndexModel(ILogger<IndexModel> logger, ApplicationPartManager partManager,
        IHostApplicationLifetime hostApplicationLifetime)
    {
        _partManager = partManager;
        _hostApplicationLifetime = hostApplicationLifetime;
    }

    public void OnGet()
    {
    }

    public IActionResult OnPostAsync()
    {
        if (!ModelState.IsValid) return Page();

        var loadContext = new RmAssemblyLoadContext(_partManager);
        var assemblyName = new AssemblyName("RmMiddleware.Controllers");
        loadContext.UnloadAssembly(assemblyName);
        Compile2();
        System.IO.File.Move(
            @"C:\Development\RiskMatrix\RmMiddleware\RmMiddleware\bin\Debug\net7.0\RmMiddleware.Models.dll",
            @"C:\Development\RiskMatrix\RmMiddleware\RmMiddleware\bin\Debug\net7.0\RmMiddleware.Models.dll.bak");
        System.IO.File.Move(
            @"C:\Development\RiskMatrix\RmMiddleware\RmMiddleware\bin\Debug\net7.0\RmMiddleware.Models1.dll",
            @"C:\Development\RiskMatrix\RmMiddleware\RmMiddleware\bin\Debug\net7.0\RmMiddleware.Models.dll");
        _hostApplicationLifetime.StopApplication();
        loadContext.LoadFromAssemblyName(assemblyName);

        return RedirectToPage("./Index");
    }

    private static void Compile()
    {
        var parameters = new CompilerParameters
        {
            GenerateExecutable = false,
            OutputAssembly =
                @"C:\Development\RiskMatrix\RmMiddleware\RmMiddleware\bin\Debug\net7.0\RmMiddleware.Models.dll"
        };
        parameters.ReferencedAssemblies.Add("System.dll");
        parameters.ReferencedAssemblies.Add("System.ComponentModel.DataAnnotations.dll");
        var netstandard =
            Assembly.Load("netstandard, Version=2.0.0.0, Culture=neutral, PublicKeyToken=cc7b13ffcd2ddd51");
        parameters.ReferencedAssemblies.Add(netstandard.Location);
        var sb = new StringBuilder();
        sb.AppendLine("using System;");
        sb.AppendLine("using System.ComponentModel.DataAnnotations;");
        sb.AppendLine("namespace RmMiddleware.Models {");
        sb.AppendLine("public class TestModel {");

        sb.AppendLine("public string Id { get; set; }");
        sb.AppendLine("public string Name { get; set; }");
        sb.AppendLine("public string FullName { get; set; }");

        sb.AppendLine("}\n}");

        CodeDomProvider.CreateProvider("CSharp").CompileAssemblyFromSource(parameters, sb.ToString());
    }

    private static void Compile2()
    {
        var sb = new StringBuilder();
        sb.AppendLine("using System;");
        sb.AppendLine("using System.Reflection;");
        sb.AppendLine("using System.ComponentModel.DataAnnotations;");
        sb.AppendLine("[assembly: AssemblyVersion(\"1.0.0\")]");
        sb.AppendLine("[assembly: AssemblyFileVersion(\"1.0.0\")]");
        sb.AppendLine("namespace RmMiddleware.Models {");
        sb.AppendLine("public class TestModel {");

        sb.AppendLine("public string Id { get; set; }");
        sb.AppendLine("public string Name { get; set; }");
        sb.AppendLine("public string FullName { get; set; }");

        sb.AppendLine("}\n}");

        var options = CSharpParseOptions.Default.WithLanguageVersion(LanguageVersion.CSharp11);
        var syntaxTree = CSharpSyntaxTree.ParseText(sb.ToString(), options);

        const string assemblyName = "RmMiddleware.Models.dll";
        var empty = string.Empty;
        var refPaths = new[]
        {
            typeof(object).GetTypeInfo().Assembly.Location,
            typeof(Console).GetTypeInfo().Assembly.Location,
            Path.Combine(
                Path.GetDirectoryName(typeof(GCSettings).GetTypeInfo().Assembly.Location) ??
                string.Empty, "System.Runtime.dll"),
            Path.Combine(
                Path.GetDirectoryName(typeof(GCSettings).GetTypeInfo().Assembly.Location) ?? empty,
                "System.ComponentModel.DataAnnotations.dll")
        };
        var references = refPaths.Select(r => MetadataReference.CreateFromFile(r)).ToArray();


        var compilation = CSharpCompilation.Create(
            assemblyName,
            new[] { syntaxTree },
            references,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary,
                optimizationLevel: OptimizationLevel.Release,
                assemblyIdentityComparer: DesktopAssemblyIdentityComparer.Default));


        using var ms = new MemoryStream();
        var result =
            compilation.Emit(
                @"C:\Development\RiskMatrix\RmMiddleware\RmMiddleware\bin\Debug\net7.0\RmMiddleware.Models1.dll");

        if (!result.Success)
        {
            var failures = result.Diagnostics.Where(diagnostic =>
                diagnostic.IsWarningAsError ||
                diagnostic.Severity == DiagnosticSeverity.Error);

            foreach (var diagnostic in failures)
                Console.Error.WriteLine("\t{0}: {1}", diagnostic.Id, diagnostic.GetMessage());
        }
        else
        {
            ms.Seek(0, SeekOrigin.Begin);
        }
    }

    private static void ReloadAssembly()
    {
    }
}