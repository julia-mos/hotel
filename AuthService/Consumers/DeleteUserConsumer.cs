using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AppDbContext;
using Entities;
using MassTransit;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Models;

namespace AuthService.Consumers
{
    public class DeleteUserConsumer : IConsumer<DeleteUserModel>
    {
        private readonly DatabaseContext _dbContext;
        private readonly UserManager<UserEntity> _userManager;


        public DeleteUserConsumer(DatabaseContext dbContext, UserManager<UserEntity> userManager)
        {
            _dbContext = dbContext;
            _userManager = userManager;
        }

        public async Task Consume(ConsumeContext<DeleteUserModel> context)
        {
            var user = await _userManager.FindByIdAsync(context.Message.Id);

            if(user == null || user.Deleted)
            {
                await context.RespondAsync(
                new ResponseEntity
                {
                    Code = HttpStatusCode.NotFound,
                    Message = "User not found"
                });

                return;
            }


            bool isOnlyAdmin = await _userManager.IsInRoleAsync(user, "Administrator") &&
                (await _userManager.GetUsersInRoleAsync("Administrator")).Where(x=>!x.Deleted).Count()==1;

            if (isOnlyAdmin)
            {
               await context.RespondAsync(
               new ResponseEntity
               {
                   Code = HttpStatusCode.BadRequest,
                   Message = "Cannot delete only admin"
               });

                return;
            }

            user.Deleted = true;

            user.FirstName = "Anon" + user.Id;
            user.LastName = "Anon" + user.Id;
            user.Email = "Anon" + user.Id;
            user.NormalizedEmail = "Anon" + user.Id;
            user.UserName = "Anon" + user.Id;
            user.NormalizedUserName = "Anon" + user.Id;

            await _dbContext.SaveChangesAsync();


            await context.RespondAsync(
                new ResponseEntity
                {
                    Code = HttpStatusCode.OK,
                    Message = "User removed"
                });

        }
        
    }
}
