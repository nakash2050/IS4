using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.Extensions;
using IdentityServer4.Models;
using IdentityServer4.Services;
using IdentityServerHost.Quickstart.UI;
using Microsoft.AspNetCore.Identity;

namespace IdentityServer
{
    // This is to load claims in the Access Token
    public class IdentityProfileService : IProfileService
    {   
        public async Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            var subjectId = context.Subject.GetSubjectId();
            var clientId = context.Client.ClientId;

            var testUser = TestUsers.Users.Find(user => user.SubjectId == subjectId);

            if (testUser != null)
            {
                context.IssuedClaims = testUser.Claims.ToList();
            }

            await Task.FromResult(context);
        }

        public async Task IsActiveAsync(IsActiveContext context)
        {
            var sub = context.Subject.GetSubjectId();
            var user = ""; // await _userManager.FindByIdAsync(sub);
            context.IsActive = user != null;

            await Task.FromResult(context);
        }
    }
}