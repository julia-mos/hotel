using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuthService.Database;
using Entities;
using MassTransit;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Consumers
{
    public class UserConsumer : IConsumer<UserListEntity>
    {
        private readonly AppDbContext _dbContext;
        private readonly UserManager<UserEntity> _userManager;


        public UserConsumer(AppDbContext dbContext, UserManager<UserEntity> userManager)
        {
            _dbContext = dbContext;
            _userManager = userManager;
        }

        public async Task Consume(ConsumeContext<UserListEntity> context)
        {
            if(context.Message.users==null || context.Message.users.Count == 0)
            {
                var users = await _dbContext.Users.ToArrayAsync();

                await context.RespondAsync(users);
            }
            else
            {
                var users = await _dbContext.Users.ToListAsync();

                var response = users.Where(x => context.Message.users.Exists(z => z.Id == x.Id)).ToArray();

                await context.RespondAsync(response);
            }
            
        }
        
    }
}
