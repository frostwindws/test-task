using ArticlesClient.Connected_Services.ArticlesService;
using ArticlesClient.Models;
using AutoMapper;

namespace ArticlesClient.Utils
{
    /// <summary>
    /// Класс-помошник для работы маппера.
    /// </summary>
    public static class AutomapHelper
    {
        public static void InitMapping()
        {
            Mapper.Initialize(config =>
            {
                config.CreateMap<CommentView, CommentDto>()
                    .ForMember(d => d.ExtensionData, m => m.Ignore())
                    .ReverseMap();
                config.CreateMap<CommentView, Connected_Services.CommentsService.CommentDto>()
                    .ForMember(d => d.ExtensionData, m => m.Ignore())
                    .ReverseMap();
                config.CreateMap<ArticleView, ArticleDto>()
                    .ForMember(d => d.ExtensionData, m => m.Ignore())
                    .ReverseMap();
            });
        }
    }
}
