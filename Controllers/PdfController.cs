using DinkToPdf;
using DinkToPdf.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace ConstructApp.Controllers
{
    public class PdfController : Controller
    {
        private readonly IConverter _converter;

        public PdfController(IConverter converter)
        {
            _converter = converter;
        }
        public IActionResult GeneratePdf()
        {
            var globalSetting = new GlobalSettings
            {
                ColorMode = ColorMode.Color,
                Orientation = Orientation.Portrait,
                PaperSize = PaperKind.A4,
                Margins = new MarginSettings { Top = 10 },
                DocumentTitle = "PDF Report"
            };

            var objectSettings = new ObjectSettings
            {
                PagesCount = true,
                HtmlContent = GetHtmlString(),
                WebSettings = { DefaultEncoding = "utf-8" },
                HeaderSettings = { FontName = "Arial", FontSize = 9, Right = "Page [page] of [toPage]", Line = true },
                FooterSettings = { FontName = "Arial", FontSize = 9, Line = true, Center = "Report Footer" }
            };

            var pdf = new HtmlToPdfDocument()
            {
                GlobalSettings = globalSetting,
                Objects = { objectSettings }
            };

            var file = _converter.Convert(pdf);

            return File(file, "application/pdf\", \"Report.pdf");
        }

        private String GetHtmlString()
        {
            return @"
            <html>
                <head>
                    <style>
                        body { font-family: 'Arial', sans-serif; }
                        .header { text-align: center; }
                        .footer { text-align: center; }
                        .content { text-align: left; }
                    </style>
                </head>
                <body>
                    <div class='header'><h1>PDF Report</h1></div>
                    <div class='content'>
                        <p>This is a sample PDF.</p>
                    </div>
                    <div class='footer'>
                        <p>Report Footer</p>
                    </div>
                </body>
            </html>";
        }
    }
}
