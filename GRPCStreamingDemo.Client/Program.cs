using Grpc.Core;
using Grpc.Net.Client;
using GRPC.Service.Protos;
using static GRPC.Service.Protos.streamTest;

static async Task TestServerStream()
{
    GrpcChannel? grpcChannel = GrpcChannel.ForAddress("http://localhost:5238");
    streamTestClient client = new streamTest.streamTestClient(grpcChannel);
    AsyncServerStreamingCall<GetUserResponse> response = client.ServerStream(new GetUsersListRequest()
    {
        PageNumber = 1,
        PageSize = 10
    });

    while (await response.ResponseStream.MoveNext(CancellationToken.None))
    {
        Console.WriteLine($"{response.ResponseStream.Current.Id} - {response.ResponseStream.Current.Name}");
    }
    Console.WriteLine("Server Streaming Completed");
    await grpcChannel.ShutdownAsync();
}
static async Task TestClientStream()
{
    GrpcChannel? grpcChannel = GrpcChannel.ForAddress("http://localhost:5238");
    streamTestClient client = new streamTestClient(grpcChannel);
    AsyncClientStreamingCall<GetUserRequest, GetUsersListResponse> clientStreamRequest = client.ClientStream();
    int i = 1;
    while (i < 10)
    {
        await clientStreamRequest.RequestStream.WriteAsync(new GetUserRequest()
        {
            Id = i
        });
        i++;
    }
    await clientStreamRequest.RequestStream.CompleteAsync();

    Console.WriteLine("Client Streaming Completed");

    var response = await clientStreamRequest.ResponseAsync;


    Console.WriteLine(response);

    await grpcChannel.ShutdownAsync();
}
await TestServerStream();
await TestClientStream();