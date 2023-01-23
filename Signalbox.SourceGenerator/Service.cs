using Microsoft.CodeAnalysis;

namespace Signalbox.SourceGenerator;

public class Service
{
    public Service(INamedTypeSymbol typeToCreate, Service? parent)
    {
        Type = typeToCreate;
        Parent = parent;
    }

    public INamedTypeSymbol Type { get; set; }
    public INamedTypeSymbol ImplementationType { get; internal set; } = null!;
    public List<Service> ConstructorArguments { get; internal set; } = new();
    public bool IsTransient { get; internal set; }
    public bool UseCollectionInitializer { get; internal set; }
    public string? VariableName { get; internal set; }
    public Service? Parent { get; internal set; }
    public ITypeSymbol? ElementType { get; internal set; }
}
