using PdfSharp.Pdf;
using PdfSharp.Pdf.Content;
using PdfSharp.Pdf.Content.Objects;
using PdfSharp.Pdf.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Pdf_Processor.Services.PdfService
{
    public class PdfService : IPdfService
    {

        public PdfService(
            )
        {

        }

        public async Task<object> Search(string fileName, string query)
        {
            string filePath = Path.Combine(
                Directory.GetCurrentDirectory() ,
                "wwwroot\\pdf-files",
                fileName
                );

            if (!File.Exists(filePath))
            {
                return null;
            }

            List<object> pdfPagesContent = new ();

            using (PdfDocument pdfDocument = PdfReader.Open(filePath, PdfDocumentOpenMode.Import))
            {
                for (int i = 0; i < pdfDocument.PageCount; i++)
                {
                    string text = string.Join("", pdfDocument.Pages[i].ExtractText().ToArray());
                    text = text.Replace("\u0000", string.Empty);

                    pdfPagesContent.Add(
                        new
                        {
                            page = i + 1,
                            text
                        }
                    ) ;
                }
            }

            return pdfPagesContent;
        }

        
    }

    public static class PdfSharpExtension
    {
        public static IEnumerable<string> ExtractText(this PdfPage page)
        {
            var content = ContentReader.ReadContent(page);
            var text = content.ExtractText();

            return text;
        }

        public static IEnumerable<string> ExtractText(this CObject cObject)
        {
            if (cObject is COperator)
            {
                var cOperator = cObject as COperator;
                if (cOperator.OpCode.Name == OpCodeName.Tj.ToString() ||
                    cOperator.OpCode.Name == OpCodeName.TJ.ToString())
                {
                    foreach (var cOperand in cOperator.Operands)
                        foreach (var txt in ExtractText(cOperand))
                            yield return txt;
                }

            }
            else if (cObject is CSequence)
            {
                var cSequence = cObject as CSequence;
                foreach (var element in cSequence)
                    foreach (var txt in ExtractText(element))
                        yield return txt;
            }
            else if (cObject is CString)
            {
                var cString = cObject as CString;
                yield return cString.Value;
            }
        }
    }
}
