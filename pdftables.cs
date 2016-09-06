using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

class Program
{
    static string uploadURL = "{{ .FullURL }}/api?key={{ .DisplayAPIKey }}&format=xml";

    static void Main()
    {
        var task = PDFToTable(@"C:\temp\your_test_pdf.pdf");
        task.Wait();

        Console.Write(task.Result);
        Console.WriteLine("Press enter to continue...");
        Console.ReadLine();
    }

    static async Task<string> PDFToTable(string filename)
    {
        using (var f = System.IO.File.OpenRead(filename))
        {
            var client = new HttpClient();
            var upload = new StreamContent(f);
            var mpcontent = new MultipartFormDataContent();
            Console.WriteLine("Uploading content...");
            mpcontent.Add(upload);

            using (var response = await client.PostAsync(uploadURL, mpcontent))
            {
                Console.WriteLine("Response status {0} {1}",
                  (int)response.StatusCode, response.StatusCode);

                using (var content = response.Content)
                {
                    return await content.ReadAsStringAsync();
                }
            }
        }
    }
}
