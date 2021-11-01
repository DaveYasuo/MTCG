using System;
using System.IO;
using System.Net;
using System.Net.Http;
using BusinessLogic;

namespace ServerModule.Http
{
    public class HttpListen
    {
        public static void HttpTask()
        {
            HttpClient client = new HttpClient();
            HttpListener listener = new HttpListener();
            listener.Prefixes.Add("http://localhost:10001/");
            listener.Start();
            int count = 0;
            while (true)
            {
                if (count == 100)
                {
                    break;
                }
                Console.WriteLine(count++);
                Console.WriteLine("Listening...");
                // Note: The GetContext method blocks while waiting for a request.
                HttpListenerContext context = listener.GetContext();
                HttpListenerRequest request = context.Request;
                //Console.WriteLine(request.RawUrl);
                string data = new StreamReader(context.Request.InputStream).ReadToEnd();
                //Console.WriteLine(data);
                RequestHandler.PrintData(data, request.RawUrl);

                // Obtain a response object.
                HttpListenerResponse response = context.Response;

                // Construct a response.
                string responseString = "Thank you, next!";
                byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseString);

                // Get a response stream and write the response to it.
                response.ContentLength64 = buffer.Length;
                using Stream output = response.OutputStream;
                output.Write(buffer, 0, buffer.Length);
                // You must close the output stream only if you don't use "using".
                // output.Close();
            }
            listener.Stop();
        }
    }
}
