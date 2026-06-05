using System;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace net10;

public static class Net10
{
    public static int Test(string[] args)
    {
        return RunServerAsync(args).GetAwaiter().GetResult();
    }

    private static async Task<int> RunServerAsync(string[] args)
    {
        var prefix = args.Length > 0 ? args[0] : "http://127.0.0.1:8080/";

        using var listener = new HttpListener();
        listener.Prefixes.Add(prefix);

        Console.CancelKeyPress += (_, e) =>
        {
            e.Cancel = true;
            listener.Stop();
        };

        listener.Start();

        Console.WriteLine($"HttpListener WebSocket test server listening on {prefix}");
        Console.WriteLine("Try connecting to ws://127.0.0.1:8080/ws from Unity WebGL.");
        Console.WriteLine("Press Ctrl+C to stop.");

        while (listener.IsListening)
        {
            HttpListenerContext context;

            try
            {
                context = await listener.GetContextAsync();
            }
            catch (HttpListenerException) when (listener.IsListening == false)
            {
                break;
            }
            catch (ObjectDisposedException)
            {
                break;
            }

            _ = Task.Run(() => HandleRequestAsync(context));
        }

        return 0;
    }

    private static async Task HandleRequestAsync(HttpListenerContext context)
    {
        var request = context.Request;
        var response = context.Response;

        Console.WriteLine();
        Console.WriteLine($"[{DateTimeOffset.Now:O}] {request.HttpMethod} {request.Url}");
        Console.WriteLine($"IsWebSocketRequest: {request.IsWebSocketRequest}");

        foreach (var headerName in request.Headers.AllKeys)
            Console.WriteLine($"{headerName}: {request.Headers[headerName]}");

        AddCorsHeaders(response);

        try
        {
            if (string.Equals(request.HttpMethod, "OPTIONS", StringComparison.OrdinalIgnoreCase))
            {
                response.StatusCode = (int)HttpStatusCode.NoContent;
                response.KeepAlive = false;
                response.Close();
                Console.WriteLine("Responded: 204 No Content (CORS preflight)");
                return;
            }

            if (request.IsWebSocketRequest)
            {
                await WriteTextResponseAsync(
                    response,
                    HttpStatusCode.Forbidden,
                    "WebSocket upgrade rejected by HttpListener server.\nThis response intentionally contains a body.\n");

                Console.WriteLine("Responded: 403 Forbidden (WebSocket rejected with body)");
                return;
            }

            await WriteTextResponseAsync(
                response,
                HttpStatusCode.OK,
                "HttpListener WebSocket test server is running.\nConnect with WebSocket to /ws to receive a rejected handshake response.\n");

            Console.WriteLine("Responded: 200 OK");
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);

            if (response.OutputStream.CanWrite)
            {
                await WriteTextResponseAsync(
                    response,
                    HttpStatusCode.InternalServerError,
                    ex.ToString());
            }
        }
    }

    private static void AddCorsHeaders(HttpListenerResponse response)
    {
        response.Headers["Access-Control-Allow-Origin"] = "*";
        response.Headers["Access-Control-Allow-Methods"] = "GET, POST, OPTIONS";
        response.Headers["Access-Control-Allow-Headers"] = "*";
        response.Headers["Access-Control-Max-Age"] = "86400";
        response.Headers["Vary"] = "Origin";
    }

    private static async Task WriteTextResponseAsync(
        HttpListenerResponse response,
        HttpStatusCode statusCode,
        string body)
    {
        var bytes = Encoding.UTF8.GetBytes(body);

        response.StatusCode = (int)statusCode;
        response.ContentType = "text/plain; charset=utf-8";
        response.ContentLength64 = bytes.Length;
        response.KeepAlive = false;

        await response.OutputStream.WriteAsync(bytes);
        response.Close();
    }
}
