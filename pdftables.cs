using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

class MainClass
{
    const string format = "xlsx-single";
    const string apiKey = "YOUR_API_KEY";
    const string uploadURL = "https://pdftables.com/api?key="+apiKey+"&format="+format;

    static int Main(string[] args)
    {
        if (args.Length != 2) {
            Console.WriteLine("Usage: <PDF file name> <Output file name>");
            return 1;
        }

        Console.WriteLine("Uploading content...");

        var task = PDFToExcel(args[0], args[1]);
        task.Wait();

        Console.WriteLine("Response status {0} {1}", (int)task.Result, task.Result);

        if ((int)task.Result != 200)
        {
            return 1;
        }

        Console.WriteLine("Written " + new System.IO.FileInfo(args[1]).Length + " bytes");
        return 0;
    }

    static async Task<HttpStatusCode> PDFToExcel(string pdfFilename, string xlsxFilename)
    {
        using (var f = System.IO.File.OpenRead(pdfFilename))
        {
            var client = new HttpClient();
            var mpcontent = new MultipartFormDataContent();
            mpcontent.Add(new StreamContent(f));

            using (var response = await client.PostAsync(uploadURL, mpcontent))
            {
                if ((int)response.StatusCode == 200)
                {
                    using (
                        Stream contentStream = await response.Content.ReadAsStreamAsync(),
                        stream = File.Create(xlsxFilename))
                    {
                        await contentStream.CopyToAsync(stream);
                    }
                }
                return response.StatusCode;
            }
        }
    }
}
