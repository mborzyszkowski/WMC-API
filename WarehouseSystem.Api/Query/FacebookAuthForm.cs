using FluentValidation;

namespace WarehouseSystem.Query
{
    public class FacebookAuthForm
    {
        public string Token { get; set; }

        public class Validator : AbstractValidator<FacebookAuthForm>
        {
            public Validator()
            {
                RuleFor(faf => faf.Token)
                    .NotEmpty();
            }
        }
    }
}