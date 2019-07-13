using System.IO;
using SharpDocx;

#if NET35
using SharpDocx.Extensions;
#endif

namespace Inheritance
{
    internal class Program
    {
        private static readonly string BasePath =
            Path.GetDirectoryName(typeof(Program).Assembly.Location) + @"/../../../../..";

        private static void Main()
        {
            var viewPath = $"{BasePath}/Views/Inheritance.cs.docx";
            var documentPath = $"{BasePath}/Documents/Inheritance.docx";

#if DEBUG
            Ide.Start<MyDocument, object>(viewPath, documentPath, null, f => f.MyProperty = "The code");
#else

            File.Copy(viewPath, documentPath, true);

            using (var targetStream = File.Open(documentPath, FileMode.Create))
            {
                var myDocument = DocumentFactory.Create<MyDocument, object>(targetStream);
                myDocument.MyProperty = "The Code";
                myDocument.Generate();
            }
#endif
        }
    }
}