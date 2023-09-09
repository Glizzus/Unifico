using System.Reflection;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Unifico.Core.Plugins;

public static class PluginFactory
{
    private static readonly Dictionary<string, IStrategyPlugin> _pluginCache = new();

    private static IStrategyPlugin? ExtractPlugin(Assembly assembly)
    {
        foreach (var type in assembly.GetTypes())
        {
            if (!typeof(IStrategyPlugin).IsAssignableFrom(type)) continue;
            if (Activator.CreateInstance(type) is IStrategyPlugin plugin)
                return plugin;
        }

        return null;
    }

    private static async Task<Assembly> CompileToDll(string relativePath)
    {
        var syntax = await File.ReadAllTextAsync(relativePath);
        var syntaxTree = CSharpSyntaxTree.ParseText(syntax);

        var assemblyName = Path.GetRandomFileName();
        var references = new List<MetadataReference>
        {
            MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(IStrategyPlugin).Assembly.Location)
        };

        Assembly.GetEntryAssembly()?.GetReferencedAssemblies()
            .ToList()
            .ForEach(assembly => references.Add(MetadataReference.CreateFromFile(Assembly.Load(assembly).Location)));

        var compilation = CSharpCompilation.Create(
            assemblyName,
            new[] { syntaxTree },
            references,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        using var dllStream = new MemoryStream();
        using var pdbStream = new MemoryStream();
        var compilationResult = compilation.Emit(dllStream, pdbStream);

        if (!compilationResult.Success)
        {
            var failures = compilationResult.Diagnostics.Where(diagnostic =>
                diagnostic.IsWarningAsError ||
                diagnostic.Severity == DiagnosticSeverity.Error);

            var message = new StringBuilder();
            foreach (var diagnostic in failures) message.AppendLine($"{diagnostic.Id}: {diagnostic.GetMessage()}");
            throw new Exception(message.ToString());
        }

        dllStream.Seek(0, SeekOrigin.Begin);
        pdbStream.Seek(0, SeekOrigin.Begin);
        return Assembly.Load(dllStream.ToArray(), pdbStream.ToArray());
    }

    public static async Task<IStrategyPlugin?> Create(string relativePath)
    {
        if (_pluginCache.TryGetValue(relativePath, out var plugin))
            return plugin;
        var assembly = await CompileToDll(relativePath);
        return ExtractPlugin(assembly);
    }
}