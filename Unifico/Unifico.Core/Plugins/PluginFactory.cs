using System.Reflection;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Unifico.Core.Plugins;

/// <summary>
///     A factory that creates <see cref="IStrategyPlugin" />s from C# code.
/// </summary>
public static class PluginFactory
{
    private static IStrategyPlugin? ExtractPlugin(Assembly assembly)
    {
        // For each type in the assembly, attempt to find our IStrategyPlugin implementation.
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

        // This adds all of the assemblies that include things like IEnumerable<T>
        Assembly.GetEntryAssembly()?.GetReferencedAssemblies()
            .ToList()
            .ForEach(assembly => references.Add(MetadataReference.CreateFromFile(Assembly.Load(assembly).Location)));

        // Compile the code to a DLL in memory
        var compilation = CSharpCompilation.Create(
            assemblyName,
            new[] { syntaxTree },
            references,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        using var dllStream = new MemoryStream();
        using var pdbStream = new MemoryStream();

        // Split the DLL and PDB to memory streams
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

        // Returns an Assembly (in memory) from the DLL with the PDB debug info
        return Assembly.Load(dllStream.ToArray(), pdbStream.ToArray());
    }

    /// <summary>
    ///     Creates a new <see cref="IStrategyPlugin" /> from the C# code at <paramref name="relativePath" />.
    ///     The code must include an implementation of <see cref="IStrategyPlugin" />.
    /// </summary>
    /// <param name="relativePath">The path at which the C# code exists.</param>
    /// <returns>An IStrategyPlugin, or null if compilation wasn't successful.</returns>
    public static async Task<IStrategyPlugin?> Create(string relativePath)
    {
        var assembly = await CompileToDll(relativePath);
        return ExtractPlugin(assembly);
    }
}