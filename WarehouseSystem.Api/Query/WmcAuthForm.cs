using FluentValidation;

namespace WarehouseSystem.Query
{
    public class WmcAuthForm
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        
        public class Validator : AbstractValidator<WmcAuthForm>
        {
            public Validator()
            {
                RuleFor(faf => faf.UserName)
                    .NotEmpty();
                RuleFor(faf => faf.Password)
                    .NotEmpty();
            }
        }
    }
}