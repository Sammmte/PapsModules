using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System.Text;
using System.CodeDom.Compiler;
using System.IO;
using System.Collections.Immutable;
using System.Linq;
using System.Diagnostics;

namespace Paps.Persistence.SourceGeneration;

[Generator]
public class DataStorageTypeRegistrationGenerator : IIncrementalGenerator
{
    private const string ATTRIBUTE_METADATA_NAME = "Paps.Persistence.DataStorageValueTypeAttribute";
    private const string GENERATED_FILE = "DataStorageTypeRegistration.g.cs";

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        #if DEBUG
        if(!Debugger.IsAttached)
        {
            Debugger.Launch();
        }
        #endif
        var candidateTypes = context.SyntaxProvider
            .ForAttributeWithMetadataName(ATTRIBUTE_METADATA_NAME,
                predicate: static (s, _) => true,
                transform: static (ctx, _) => GetTypeData(ctx.Attributes, ctx.SemanticModel, ctx.TargetNode))
            .Where(static m => m is not null)
            .Collect();

        context.RegisterSourceOutput(candidateTypes, static (spc, source) => Execute(spc, source));
    }

    private static void Execute(SourceProductionContext context, ImmutableArray<DataStorageTypeRegistrationData?> typesData)
    {
        if(typesData.Length == 0)
            return;

        var source = GenerateSource(typesData);

        context.AddSource(GENERATED_FILE, source);
    }

    static DataStorageTypeRegistrationData? GetTypeData(ImmutableArray<AttributeData> attributes, SemanticModel semanticModel, SyntaxNode typeDeclarationSyntax)
    {
        // Get the semantic representation of the type syntax
        if(semanticModel.GetDeclaredSymbol(typeDeclarationSyntax) is not INamedTypeSymbol semanticTypeDeclaration)
        {
            // something went wrong
            return null;
        }

        var typeDiscriminator = attributes[0].ConstructorArguments[0].Value as string;

        if(typeDiscriminator == null)
        {
            // we didn't find the attribute or the type discriminator
            return null;
        }

        var typeName = semanticTypeDeclaration.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);

        var initialPoolCapacity = (int)attributes[0].ConstructorArguments[1].Value;

        return new DataStorageTypeRegistrationData(typeName, typeDiscriminator, initialPoolCapacity);
    }

    private static SourceText GenerateSource(ImmutableArray<DataStorageTypeRegistrationData?> typesData)
    {
        using var sourceStream = new StringWriter();
        using var codeWriter = new IndentedTextWriter(sourceStream);
        
        codeWriter.WriteLine("using System;");
        codeWriter.WriteLine("using UnityEngine;");
        codeWriter.WriteLine("namespace Paps.Persistence.SourceGeneration.Generated {");
        codeWriter.Indent++;

        codeWriter.WriteLine("internal static class DataStorageTypeRegistration {");
        codeWriter.Indent++;

        codeWriter.WriteLine("[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]");
        codeWriter.WriteLine("public static void RegisterValidTypes()");
        codeWriter.WriteLine("{");
        codeWriter.Indent++;

        foreach(var typeInfo in typesData)
        {
            codeWriter.WriteLine($"DataStorageSerializationHelper.RegisterDataStorageEntryValueType<{typeInfo.Value.FullTypeMetadataName}>(\"{typeInfo.Value.TypeDiscriminator}\");");
            codeWriter.WriteLine($"DataStorageEntry<{typeInfo.Value.FullTypeMetadataName}>.PreparePoolAmount({typeInfo.Value.InitialPoolCapacity});");
        }

        codeWriter.Indent--;
        codeWriter.WriteLine("}");

        codeWriter.Indent--;
        codeWriter.WriteLine("}");

        codeWriter.Indent--;
        codeWriter.WriteLine("}");
        
        return SourceText.From(sourceStream.ToString(), Encoding.UTF8);
    }
}
