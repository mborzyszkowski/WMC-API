using FluentValidation;

namespace WarehouseSystem.Query
{
    public class ProductForm
    {
        public string ManufacturerName { get; set; }
        public string ModelName { get; set; }
        public double? Price { get; set; }
        
        public class Validator : AbstractValidator<ProductForm>
        {
            public Validator()
            {
                RuleFor(p => p.ManufacturerName)
                    .NotEmpty();
                
                RuleFor(p => p.ModelName)
                    .NotEmpty();
                
                RuleFor(p => p.Price)
                    .NotNull()
                    .GreaterThan(0);
            }
        }
    }
}