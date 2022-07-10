using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppDbContext;
using Entities;
using MassTransit;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Models.AuthService;

namespace AuthService.Consumers
{
    public class UserConsumer : IConsumer<UserListEntity>
    {
        private readonly DatabaseContext _dbContext;
        private readonly UserManager<UserEntity> _userManager;


        public UserConsumer(DatabaseContext dbContext, UserManager<UserEntity> userManager)
        {
            _dbContext = dbContext;
            _userManager = userManager;
        }

        public async Task Consume(ConsumeContext<UserListEntity> context)
        {
            if(context.Message.users==null || context.Message.users.Count == 0)
            {
                var users = await _dbContext
                    .Users
                    .Where(x=>!x.Deleted)
                    .Select(x => new GetUserModel() { Id = x.Id, FirstName = x.FirstName, LastName = x.LastName, Email = x.Email })
                    .ToArrayAsync();

                await context.RespondAsync(users);
            }
            else
            {
                var users = await _dbContext
                    .Users
                    .Where(x=>!x.Deleted)
                    .Select(x => new GetUserModel() { Id = x.Id, FirstName = x.FirstName, LastName = x.LastName, Email = x.Email })
                    .ToListAsync();

                var response = users
                    .Where(x => context.Message.users.Exists(z => z.Id == x.Id))
                    .ToArray();

                await context.RespondAsync(response);
            }
            
        }
        
    }
}
