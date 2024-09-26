using System.Reflection;
using System.Runtime.Loader;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.Extensions.DependencyModel;

namespace RmMiddleware;

public class RmAssemblyLoadContext : AssemblyLoadContext
{
    private readonly ApplicationPartManager _partManager;

    public RmAssemblyLoadContext(ApplicationPartManager partManager)
    {
        _partManager = partManager;
    }

    protected override Assembly? Load(AssemblyName assemblyName)
    {
        var currentPart = _partManager.ApplicationParts.FirstOrDefault(x => x.Name == assemblyName.Name);
        if (currentPart != null) _partManager.ApplicationParts.Remove(currentPart);

        var deps = DependencyContext.Default;
        var res = deps?.CompileLibraries.Where(d => assemblyName.Name != null && d.Name.Contains(assemblyName.Name))
            .ToList();
        var name = res?.FirstOrDefault()?.Name;
        if (name == null) return null;
        var assembly = Assembly.Load(new AssemblyName(name));
        ApplicationPart part = new AssemblyPart(assembly);
        _partManager.ApplicationParts.Add(part);
        return assembly;
    }

    public void UnloadAssembly(AssemblyName assemblyName)
    {
        var currentPart = _partManager.ApplicationParts.FirstOrDefault(x => x.Name == assemblyName.Name);
        if (currentPart != null) _partManager.ApplicationParts.Remove(currentPart);
    }

    public void ReLoad(Assembly assembly)
    {
        var currentPart = _partManager.ApplicationParts.FirstOrDefault(x => x.Name == assembly.FullName);
        if (currentPart != null) _partManager.ApplicationParts.Remove(currentPart);

        ApplicationPart part = new AssemblyPart(assembly);
        _partManager.ApplicationParts.Add(part);
    }
}