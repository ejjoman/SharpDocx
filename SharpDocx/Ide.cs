using System;
using System.Diagnostics;
using System.IO;

#if NET35
using SharpDocx.Extensions;
#endif

namespace SharpDocx
{
    public static class Ide
    {
        public static void Start<TModel>(string viewPath, string documentPath, TModel model = default, Action<DocumentBase<TModel>> initializeDocument = null)
        {
            Start<DocumentBase<TModel>, TModel>(viewPath, documentPath, model, initializeDocument);
        }

        public static void Start<TBaseClass, TModel>(string viewPath, string documentPath, TModel model = default, Action<TBaseClass> initializeDocument = null) where TBaseClass : DocumentBase<TModel>
        {
            Console.WriteLine("Initializing SharpDocx IDE...");

            viewPath = Path.GetFullPath(viewPath);
            documentPath = Path.GetFullPath(documentPath);
            ConsoleKeyInfo keyInfo;

            do
            {
                try
                {
                    Console.WriteLine();
                    Console.WriteLine($"Compiling '{viewPath}'.");

                    using (var ms = new MemoryStream(File.ReadAllBytes(viewPath)))
                    {
                        var document = DocumentFactory.Create<TBaseClass, TModel>(ms, true);
                        initializeDocument?.Invoke(document);
                        document.Generate(model);

                        ms.Seek(0, SeekOrigin.Begin);

                        using (var fs = File.Create(documentPath))
                            ms.CopyTo(fs);
                    }

                    Console.WriteLine($"Succesfully generated '{documentPath}'.");

                    try
                    {
                        // Show the generated document.
                        Process.Start(documentPath);
                    }
                    catch
                    {
                        // Ignored.
                    }
                }
                catch (SharpDocxCompilationException e)
                {
                    Console.WriteLine(e.SourceCode);
                    Console.WriteLine(e.Errors);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }

                GC.Collect();
                Console.WriteLine("Press Esc to exit, any other key to retry . . .");
                keyInfo = Console.ReadKey(true);
            } while (keyInfo.Key != ConsoleKey.Escape);
        }
    }
}