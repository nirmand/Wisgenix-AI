using System;
using Wisgenix.Repositories;
using Wisgenix.Repositories.Interfaces;

namespace Wisgenix.API.Extensions;

public static class InjectRepositoriesExtensions
{
    public static IServiceCollection InjectRepositories(this IServiceCollection services)
    {
        services.AddScoped<ITopicRepository, TopicRepository>();
        services.AddScoped<IQuestionRepository, QuestionRepository>();
        services.AddScoped<IQuestionOptionRepository, QuestionOptionRepository>();
        services.AddScoped<ISubjectRepository, SubjectRepository>();
        return services;
    }
}
