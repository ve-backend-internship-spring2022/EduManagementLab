using EduManagementLab.Core.Services;
using IdentityModel;
using IdentityServer4.Validation;

namespace EduManagementLab.EfRepository
{
    public class CustomResourceOwnerPasswordValidator : IResourceOwnerPasswordValidator
    {
        private readonly UserService _userService;
        public CustomResourceOwnerPasswordValidator(UserService userService)
        {
            _userService = userService;
        }

        public Task ValidateAsync(ResourceOwnerPasswordValidationContext context)
        {
            if (_userService.ValidateCredentials(context.UserName, context.Password))
            {
                var user = _userService.GetUserUsername(context.UserName);
                context.Result = new GrantValidationResult(user.Id.ToString(), OidcConstants.AuthenticationMethods.Password);
            }

            return Task.FromResult(0);
        }
    }
}