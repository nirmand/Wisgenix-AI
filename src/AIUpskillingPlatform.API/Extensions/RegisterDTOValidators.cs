using AIUpskillingPlatform.DTO.Validators;
using FluentValidation;
using FluentValidation.AspNetCore;

namespace AIUpskillingPlatform.API.Extensions;

public static class RegisterDTOValidatorsExtensions
{
    public static IServiceCollection RegisterDTOValidators(this IServiceCollection services)
    {
        services.AddValidatorsFromAssemblyContaining<CreateSubjectDtoValidator>();
        services.AddValidatorsFromAssemblyContaining<UpdateSubjectDtoValidator>();
        services.AddValidatorsFromAssemblyContaining<CreateTopicDtoValidator>();
        services.AddValidatorsFromAssemblyContaining<UpdateTopicDtoValidator>();
        services.AddFluentValidationAutoValidation();
        return services;
    }
}
