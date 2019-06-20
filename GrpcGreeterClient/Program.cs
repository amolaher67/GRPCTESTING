using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Grpc.Net.Client;
using GrpcGreeter;



namespace GrpcGreeterClient
{
    class Program
    {
        static async Task Main(string[] args)
        {
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport",true);
            var httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri("http://localhost:50051");
            var client = GrpcClient.Create<Greeter.GreeterClient>(httpClient);

            var reply = await client.SayHelloAsync(
                              new HelloRequest { Name = "GreeterClient" });
                                 
            Console.WriteLine("Greeting: " + reply.Message);

            using (var call = client.lotsOfReplies(new HelloRequest { Name = "GreeterClient" }))
            {
                while(await call.ResponseStream.MoveNext())
                {
                    var replystreammsg = call.ResponseStream.Current;
                    Console.WriteLine("Received " + replystreammsg.ToString());
                }
            }

            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}
