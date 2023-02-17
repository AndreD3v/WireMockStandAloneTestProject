// just create a console application, and replace the program.cs file with below code
// by using http://localhost:{{port}}/data e.g. within Postman you can validate if the
// standalone stubs works fine!

using Newtonsoft.Json;
using System.Net;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;
using WireMockStandAloneTestProject.Models;

int port;
if (args.Length == 0 || !int.TryParse(args[0], out port))
    port = 8079;

var server = WireMockServer.Start(port);
Console.WriteLine("WireMockServer running at {0}", string.Join(",", server.Ports));

server.Given(Request.Create()
        .WithPath("/api/*")
        .UsingGet()
        .WithParam("name", "Andre"))
    .AtPriority(10)
    .RespondWith(Response.Create()
        .WithStatusCode(200)
        .WithHeader("Content-Type", "application/json")
        .WithBodyAsJson(new UserModel()));

server.Given(Request.Create()
        .WithPath("/api/*")
        .UsingGet())
    .AtPriority(20)
    .RespondWith(Response.Create()
        .WithStatusCode(200)
        .WithHeader("Content-Type", "application/json")
        .WithBody(@"{ ""msg"": ""used get with wildcard""}")
    );

server.Given(Request.Create()
        .WithPath(x => x.Contains("abc"))
        .UsingGet())
    .AtPriority(10)
    .RespondWith(Response.Create()
        .WithStatusCode(200)
        .WithHeader("Content-Type", "application/json")
        .WithBody(@"{ ""result"": ""path contained 'abc'""}"));

server.Given(Request.Create()
        .WithPath("/api/data")
        .UsingPost()
        .WithBody(x => x.Contains("newspark")))
    .AtPriority(40)
    .RespondWith(Response.Create()
        .WithStatusCode(201)
        .WithHeader("Content-Type", "application/json")
        .WithBody(@"{ ""result"": ""data posted with newspark - 201""}"));

server.Given(Request.Create()
        .WithPath("/api/data")
        .UsingPost())
    .AtPriority(40)
    .RespondWith(Response.Create()
        .WithStatusCode(201)
        .WithHeader("Content-Type", "application/json")
        .WithBody(@"{ ""result"": ""data posted without certain content - 201""}")
    );

server.Given(Request.Create()
        .WithPath("/api/data")
        .UsingDelete())
    .AtPriority(50)
    .RespondWith(Response.Create()
        .WithStatusCode(HttpStatusCode.NoContent));

Console.WriteLine("Press any key to stop the server");
Console.ReadKey();

Console.WriteLine("Displaying all requests");
var allRequests = server.LogEntries;
Console.WriteLine(JsonConvert.SerializeObject(allRequests, Formatting.Indented));

Console.WriteLine("Press any key to quit");
Console.ReadKey();