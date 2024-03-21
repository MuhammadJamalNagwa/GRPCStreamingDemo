using Azure.Core;
using Grpc.Core;
using GRPC.Service.Protos;
using GRPCStreamingDemo.Data;
using GRPCStreamingDemo.Models;
using Microsoft.EntityFrameworkCore;

namespace GRPCStreamingDemo.Services;

public class StreamTestService : streamTest.streamTestBase
{
    private readonly ApplicationDbContext _dbContext;
    private readonly ILogger<StreamTestService> _logger;
    public StreamTestService(ApplicationDbContext dbContext, ILogger<StreamTestService> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }
    public override async Task<GetUsersListResponse> ClientStream(IAsyncStreamReader<GetUserRequest> requestStream, ServerCallContext context)
    {
        List<int> userIds = new();
        GetUsersListResponse response = new();

        while (await requestStream.MoveNext())
        {
            userIds.Add(requestStream.Current.Id);
        }

        List<UserEntity> selectedUsers = await _dbContext.Users.Where(u => userIds.Any(ru => ru == u.Id)).ToListAsync();

        response.UserList.AddRange(selectedUsers.Select(u => new GetUserResponse()
        {
            Id = u.Id,
            Address = u.Address,
            Age = u.Age,
            Email = u.Email,
            Name = u.Name
        }));

        return await Task.FromResult(response);
    }
    public override async Task ServerStream(GetUsersListRequest request, IServerStreamWriter<GetUserResponse> responseStream, ServerCallContext context)
    {
        var usersList = _dbContext.Users.Take(request.PageSize).Select(u => new GetUserResponse()
        {
            Id = u.Id,
            Address = u.Address,
            Age = u.Age,
            Email = u.Email,
            Name = u.Name
        });
        foreach (var item in usersList)
        {
            await responseStream.WriteAsync(item);
        }
    }
}
