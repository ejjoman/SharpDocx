using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using SharpDocx.Extensions;

namespace SharpDocx
{
    internal class DocumentAssembly<TBaseClass, TModel> where TBaseClass : DocumentBase<TModel>
    {
        private readonly Assembly _assembly;
        private readonly string _className;

        internal DocumentAssembly(Stream documentStream)
        {
            if (documentStream == null)
            {
                throw new ArgumentNullException(nameof(documentStream));
            }

            // Load base class assembly.
            var assembly = Assembly.LoadFrom(typeof(TBaseClass).Assembly.Location);
            if (assembly == null)
            {
                throw new TypeLoadException($"Can't load assembly '{typeof(TBaseClass).Assembly}'");
            }

            // Get the base class type.
            var t = assembly.GetType(typeof(TBaseClass).FullName);
            if (t == null)
            {
                throw new TypeLoadException(
                    $"Can't find base class '{typeof(TBaseClass).FullName}' in assembly '{typeof(TBaseClass).Assembly}'");
            }

            // Check base class type.
            if (t != typeof(DocumentBase<TModel>) && !t.IsSubclassOf(typeof(DocumentBase<TModel>)))
            {
                throw new InvalidOperationException("baseClass should be a DocumentBase derived type");
            }

            // Get user defined using directives by calling the static DocumentBase.GetUsingDirectives method.
            var usingDirectives =
                (List<string>) assembly.Invoke(
                    typeof(TBaseClass).FullName,
                    null,
                    nameof(DocumentBase<TModel>.GetUsingDirectives),
                    null)
                ?? new List<string>();
            

            // Get user defined assemblies to reference.
            var referencedAssemblies =
                (List<string>) assembly.Invoke(
                    typeof(TBaseClass).FullName,
                    null,
                    nameof(DocumentBase<TModel>.GetReferencedAssemblies),
                    null)
                ?? new List<string>();

            // Add namespace(s) of Model and reference Model assembly/assemblies.
            foreach (var type in GetTypes(typeof(TModel)))
            {
                usingDirectives.Add($"using {type.Namespace};");
                referencedAssemblies.Add(type.Assembly.Location);
            }

            // Create a unique class name.
            _className = $"SharpDocument_{Guid.NewGuid():N}";

            // Create an assembly for this class.
            _assembly = DocumentCompiler<TBaseClass, TModel>.Compile(
                documentStream,
                _className,
                usingDirectives,
                referencedAssemblies);
        }

        public object Instance()
        {
            return _assembly.CreateInstance($"{DocumentCompiler<TBaseClass, TModel>.Namespace}.{_className}", null);
        }

        private static IEnumerable<Type> GetTypes(Type type)
        {
#if !NET35
            if (type.IsConstructedGenericType)
            {
                foreach (var t in type.GenericTypeArguments)
                {
                    yield return t;
                }
            }
#endif
            yield return type;
        }
    }
}