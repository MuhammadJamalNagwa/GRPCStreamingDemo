using GRPCStreamingDemo.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GRPCStreamingDemo.Configuration;

internal class UserConfiguration : IEntityTypeConfiguration<UserEntity>
{
    public void Configure(EntityTypeBuilder<UserEntity> builder)
    {
        builder.ToTable("GRPC.User");

        builder.HasData(GetUsers());

        builder.Property(u => u.Email).IsRequired(false);

        builder.Property(u => u.Address).IsRequired(false);
    }

    public List<UserEntity> GetUsers()
    {
        List<UserEntity> usersList = new();

        int i = 1;
        while (i <= 1000)
        {
            usersList.Add(new()
            {
                Id = i,
                Name = $"User {i}",
                Age = i,
                Email = $"user{i}@grpc.demo",
                Address = $"Addresss {i}"
            });
            i++;
        }

        return usersList;
    }
}
