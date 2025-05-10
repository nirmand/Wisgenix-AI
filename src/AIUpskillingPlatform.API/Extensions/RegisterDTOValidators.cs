using AIUpskillingPlatform.DTO.Validators;
using FluentValidation;
using FluentValidation.AspNetCore;

namespace AIUpskillingPlatform.API.Extensions;

public static class RegisterDTOValidatorsExtensions
{
    public static IServiceCollection RegisterDTOValidators(this IServiceCollection services)
    {
        // Subject validators
        services.AddValidatorsFromAssemblyContaining<CreateSubjectDtoValidator>();
        services.AddValidatorsFromAssemblyContaining<UpdateSubjectDtoValidator>();
        
        // Topic validators
        services.AddValidatorsFromAssemblyContaining<CreateTopicDtoValidator>();
        services.AddValidatorsFromAssemblyContaining<UpdateTopicDtoValidator>();
        
        // Question validators
        services.AddValidatorsFromAssemblyContaining<CreateQuestionDtoValidator>();
        services.AddValidatorsFromAssemblyContaining<UpdateQuestionDtoValidator>();
        
        services.AddFluentValidationAutoValidation();
        return services;
    }
}
