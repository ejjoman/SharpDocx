using System.IO;
using SharpDocx;

namespace Tutorial
{
    internal class Program
    {
        private static readonly string BasePath =
            Path.GetDirectoryName(typeof(Program).Assembly.Location) + @"/../../../../..";

        private static void Main()
        {
            var viewPath = $"{BasePath}/Views/Tutorial.cs.docx";
            var documentPath = $"{BasePath}/Documents/Tutorial.docx";
            var imageDirectory = $"{BasePath}/Images";

#if DEBUG
            Ide.Start<DocumentBase<object>, object>(viewPath, documentPath, null, f => f.ImageDirectory = imageDirectory);
#else

            File.Copy(viewPath, documentPath, true);

            using (var targetStream = File.Open(documentPath, FileMode.Create))
            {
                var document = DocumentFactory.Create<object>(targetStream);
                document.ImageDirectory = imageDirectory;
                document.Generate();
            }
#endif
        }
    }
}