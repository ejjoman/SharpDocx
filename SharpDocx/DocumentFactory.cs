using System;
using System.Collections;
using System.IO;

namespace SharpDocx
{
    // DocumentFactory purpose:
    // -Generate and cache DocumentAssemblies;
    // -Create instances of a DocumentBase derived type.

    public static class DocumentFactory
    {
        private static readonly Hashtable Assemblies = new Hashtable();
        private static readonly object AssembliesLock = new object();

        public static DocumentBase<TModel> Create<TModel>(Stream documentStream)
        {
            return Create<TModel>(documentStream, false);
        }

        public static DocumentBase<TModel> Create<TModel>(Stream documentStream, bool forceCompile)
        {
            return Create<DocumentBase<TModel>, TModel>(documentStream, forceCompile);
        }

        public static TBaseClass Create<TBaseClass, TModel>(Stream documentStream) where TBaseClass : DocumentBase<TModel>
        {
            return Create<TBaseClass, TModel>(documentStream, false);
        }

        public static TBaseClass Create<TBaseClass, TModel>(Stream documentStream, bool forceCompile) where TBaseClass : DocumentBase<TModel>
        {
            var baseClassName = typeof(TBaseClass).Name;
            var modelTypeName = typeof(TModel).Name;

            DocumentAssembly<TBaseClass, TModel> da;
            lock (AssembliesLock)
            {
                var assemblyKey = baseClassName + modelTypeName;

                da = (DocumentAssembly<TBaseClass, TModel>)Assemblies[assemblyKey];

                if (da == null || forceCompile)
                {
                    da = new DocumentAssembly<TBaseClass, TModel>(documentStream);
                    Assemblies[assemblyKey] = da;
                }
            }

            var document = (TBaseClass)da.Instance();
            document.Init(documentStream);
            return document;
        }
    }
}