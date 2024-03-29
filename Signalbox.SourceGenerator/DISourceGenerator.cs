﻿using System.Diagnostics.CodeAnalysis;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace Signalbox.SourceGenerator;

[Generator]
public class DISourceGenerator : ISourceGenerator
{
    public void Initialize(GeneratorInitializationContext context)
    {
    }

    public void Execute(GeneratorExecutionContext context)
    {
        var compilation = context.Compilation;

        var sourceBuilder = Generate(context, compilation);
        context.AddSource("ServiceLocator.cs", SourceText.From(sourceBuilder, Encoding.UTF8));
    }

    public static string Generate(GeneratorExecutionContext context, Compilation compilation)
    {
        var stub = @"
// <auto-generated />
namespace DI
{ 
    internal static class ServiceLocator
    {
        /// <summary>Gets a service via ⭐ magic ⭐</summary>
        public static T GetService<T>()
        {
            return default;
        }
    }
}
";

        var options = (compilation as CSharpCompilation)?.SyntaxTrees[0].Options as CSharpParseOptions;
        compilation = compilation.AddSyntaxTrees(CSharpSyntaxTree.ParseText(SourceText.From(stub, Encoding.UTF8), options));

        var diags = compilation.GetDiagnostics();

        var sourceBuilder = new StringBuilder();

        var services = new List<Service>();

        var serviceLocatorClass = compilation.GetTypeByMetadataName("DI.ServiceLocator")!;
        var transientAttribute = compilation.GetTypeByMetadataName("Signalbox.Engine.Utilities.TransientAttribute")!;
        var orderAttribute = compilation.GetTypeByMetadataName("Signalbox.Engine.Utilities.OrderAttribute")!;
        var layoutOfT = compilation.GetTypeByMetadataName("Signalbox.Engine.Entity.ILayout`1")!.ConstructUnboundGenericType();
        var filteredLayout = compilation.GetTypeByMetadataName("Signalbox.Engine.Entity.FilteredLayout`1")!;
        var iEnumerableOfT = compilation.GetTypeByMetadataName("System.Collections.Generic.IEnumerable`1")!.ConstructUnboundGenericType();
        var listOfT = compilation.GetTypeByMetadataName("System.Collections.Generic.List`1")!;

        var knownTypes = new KnownTypes(transientAttribute, orderAttribute, layoutOfT, filteredLayout, iEnumerableOfT, listOfT);

        foreach (var tree in compilation.SyntaxTrees)
        {
            var semanticModel = compilation.GetSemanticModel(tree);
            IEnumerable<INamedTypeSymbol>? typesToCreate = from i in tree.GetRoot().DescendantNodesAndSelf().OfType<InvocationExpressionSyntax>()
                                                           let symbol = semanticModel.GetSymbolInfo(i).Symbol as IMethodSymbol
                                                           where symbol != null
                                                           where SymbolEqualityComparer.Default.Equals(symbol.ContainingType, serviceLocatorClass)
                                                           select symbol.ReturnType as INamedTypeSymbol;

            foreach (var typeToCreate in typesToCreate)
            {
                Generate(context, typeToCreate, compilation, services, null, knownTypes);
            }
        }

        sourceBuilder.AppendLine(@"
// <auto-generated />
namespace DI
{ 
    internal static class ServiceLocator
    {");
        var fields = new List<Service>();
        GenerateFields(sourceBuilder, services, fields);

        sourceBuilder.AppendLine(@"
        /// <summary>Gets a service via ⭐ magic ⭐</summary>
        public static T GetService<T>()
        {");

        foreach (var service in services)
        {
            sourceBuilder.AppendLine("        if (typeof(T) == typeof(" + service.Type + "))");
            sourceBuilder.AppendLine("        {");
            sourceBuilder.AppendLine($"            return (T)(object){GetTypeConstruction(service, service.IsTransient ? new() : fields)};");
            sourceBuilder.AppendLine("        }");
        }

        sourceBuilder.AppendLine("        throw new System.InvalidOperationException(\"Don't know how to initialize type: \" + typeof(T).Name);");
        sourceBuilder.AppendLine(@"
        }
    }
}");

        return sourceBuilder.ToString();
    }

    private static void GenerateFields(StringBuilder sourceBuilder, List<Service> services, List<Service> fields)
    {
        foreach (var service in services)
        {
            GenerateFields(sourceBuilder, service.ConstructorArguments, fields);
            if (service.IsTransient) continue;
            if (fields.Any(f => SymbolEqualityComparer.Default.Equals(f.ImplementationType, service.ImplementationType)))
            {
                continue;
            }
            service.VariableName = GetVariableName(service, fields);
            sourceBuilder.AppendLine($"        private static {service.Type} {service.VariableName} = {GetTypeConstruction(service, fields)};");
            fields.Add(service);
        }
    }

    private static string GetTypeConstruction(Service service, List<Service> fields)
    {
        var sb = new StringBuilder();

        var field = fields.FirstOrDefault(f => SymbolEqualityComparer.Default.Equals(f.ImplementationType, service.ImplementationType));
        if (field != null)
        {
            if (service.Parent?.ElementType is not null)
            {
                sb.Append('(');
                sb.Append(service.Parent.ElementType);
                sb.Append(')');
            }
            sb.Append(field.VariableName);
        }
        else
        {
            sb.Append("new ");
            sb.Append(service.ImplementationType);
            sb.Append('(');
            if (service.UseCollectionInitializer)
            {
                sb.Append(')');
                sb.Append('{');
            }
            var first = true;
            foreach (var arg in service.ConstructorArguments)
            {
                if (!first)
                {
                    sb.Append(", ");

                }
                sb.Append(GetTypeConstruction(arg, fields));
                first = false;
            }
            if (service.UseCollectionInitializer)
            {
                sb.Append('}');
            }
            else
            {
                sb.Append(')');
            }
        }
        return sb.ToString();
    }

    private static string GetVariableName(Service service, List<Service> fields)
    {
        var typeName = service.ImplementationType.ToString().Replace("<", "").Replace(">", "").Replace("?", "");

        string[] parts = typeName.Split('.');
        for (var i = parts.Length - 1; i >= 0; i--)
        {
            var candidate = string.Join("", parts.Skip(i));
            candidate = "_" + char.ToLowerInvariant(candidate[0]) + candidate.Substring(1);
            if (!fields.Any(f => string.Equals(f.VariableName, candidate, StringComparison.Ordinal)))
            {
                typeName = candidate;
                break;
            }
        }
        return typeName;
    }

    private static void Generate(GeneratorExecutionContext context, INamedTypeSymbol typeToCreate, Compilation compilation, List<Service> services, Service? parent, KnownTypes knownTypes)
    {
        // System.Diagnostics.Debugger.Launch();
        typeToCreate = (INamedTypeSymbol)typeToCreate.WithNullableAnnotation(default);

        if (typeToCreate.IsGenericType && SymbolEqualityComparer.Default.Equals(typeToCreate.ConstructUnboundGenericType(), knownTypes.IEnumerableOfT))
        {
            var typeToFind = typeToCreate.TypeArguments[0];
            var types = FindImplementations(context, compilation, knownTypes, typeToFind);

            if (!types.Any())
            {
                context.ReportDiagnostic(Diagnostic.Create("TRAINS2", "DI", $"Can't find an implemnentation for {typeToFind} to construct an IEnumerable", DiagnosticSeverity.Error, DiagnosticSeverity.Error, true, 0, false));
            }

            var list = knownTypes.ListOfT.Construct(typeToFind);
            var listService = new Service(typeToCreate, parent)
            {
                ImplementationType = list,
                UseCollectionInitializer = true,
                ElementType = typeToFind
            };

            if (CheckForCycle(context, services, list))
            {
                return;
            }
            services.Add(listService);

            foreach (var thingy in types)
            {
                Generate(context, thingy, compilation, listService.ConstructorArguments, listService, knownTypes);
            }
        }
        else if (typeToCreate.IsGenericType && SymbolEqualityComparer.Default.Equals(typeToCreate.ConstructUnboundGenericType(), knownTypes.ILayoutOfT))
        {
            var entityType = typeToCreate.TypeArguments[0];

            var layout = knownTypes.FilteredLayout.Construct(entityType);

            var layoutService = new Service(typeToCreate, parent);
            services.Add(layoutService);
            layoutService.ImplementationType = layout;
            Generate(context, layout, compilation, layoutService.ConstructorArguments, layoutService, knownTypes);
        }
        else
        {
            var realType = typeToCreate.IsAbstract ? FindImplementation(context, compilation, knownTypes, typeToCreate) : typeToCreate;

            if (realType == null)
            {
                context.ReportDiagnostic(Diagnostic.Create("TRAINS1", "DI", $"Can't find an implemnentation for {typeToCreate}", DiagnosticSeverity.Error, DiagnosticSeverity.Error, true, 0, false));
            }
            else
            {
                var service = new Service(typeToCreate, parent)
                {
                    ImplementationType = realType,
                    IsTransient = typeToCreate.GetAttributes().Any(c => SymbolEqualityComparer.Default.Equals(c.AttributeClass, knownTypes.TransientAttribute))
                };

                if (CheckForCycle(context, services, realType))
                {
                    return;
                }
                services.Add(service);

                var constructor = realType?.Constructors.FirstOrDefault();
                if (constructor != null)
                {
                    foreach (var parametr in constructor.Parameters)
                    {
                        if (parametr.Type is INamedTypeSymbol paramType)
                        {
                            Generate(context, paramType, compilation, service.ConstructorArguments, service, knownTypes);
                        }
                    }
                }
            }
        }
    }

    private static bool CheckForCycle(GeneratorExecutionContext context, List<Service> services, INamedTypeSymbol typeToCreate)
    {
        foreach (var service in services)
        {
            var current = service;
            while (current != null)
            {
                if (SymbolEqualityComparer.Default.Equals(current.ImplementationType, typeToCreate))
                {
                    context.ReportDiagnostic(Diagnostic.Create("TRAINS3", "DI", $"Circular reference detected: {typeToCreate}", DiagnosticSeverity.Error, DiagnosticSeverity.Error, true, 0, false));
                    return true;
                }
                current = current.Parent;
            }
        }
        return false;
    }

    private static INamedTypeSymbol? FindImplementation(GeneratorExecutionContext context, Compilation compilation, KnownTypes knownTypes, ITypeSymbol typeToFind)
    {
        return FindImplementations(context, compilation, knownTypes, typeToFind).FirstOrDefault();
    }

    private static IOrderedEnumerable<INamedTypeSymbol> FindImplementations(GeneratorExecutionContext context, Compilation compilation, KnownTypes knownTypes, ITypeSymbol typeToFind)
    {
        return FindImplementations(context, typeToFind, compilation).OrderBy(t => (int)(t.GetAttributes().FirstOrDefault(a => SymbolEqualityComparer.Default.Equals(a.AttributeClass, knownTypes.OrderAttribute))?.ConstructorArguments[0].Value ?? 0));
    }

    [SuppressMessage("Usage", "CA1801:Review unused parameters", Justification = "<Pending>")]
    [SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "<Pending>")]
    private static IEnumerable<INamedTypeSymbol> FindImplementations(GeneratorExecutionContext context, ITypeSymbol typeToFind, Compilation compilation)
    {
        foreach (var ns in compilation.GlobalNamespace.GetNamespaceMembers())
        {
            if (ns.Name == "System" || ns.Name == "Microsoft" || ns.Name == "SkiaSharp" || ns.Name == "OpenTK") continue;

            var count = 0;
            foreach (var x in GetAllTypes(new[] { ns }))
            {
                count++;
                if (!x.IsAbstract && x.AllInterfaces.Any(i => SymbolEqualityComparer.Default.Equals(i, typeToFind)))
                {
                    yield return x;
                }
            }
        }
    }

    private static IEnumerable<INamedTypeSymbol> GetAllTypes(IEnumerable<INamespaceSymbol> namespaces)
    {
        foreach (var ns in namespaces)
        {
            foreach (var t in ns.GetTypeMembers())
            {
                yield return t;
            }

            foreach (var subType in GetAllTypes(ns.GetNamespaceMembers()))
            {
                yield return subType;
            }
        }
    }
}
