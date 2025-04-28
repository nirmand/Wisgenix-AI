using System;
using AIUpskillingPlatform.Repositories;
using AIUpskillingPlatform.Repositories.Interfaces;

namespace AIUpskillingPlatform.API.Extensions;

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
