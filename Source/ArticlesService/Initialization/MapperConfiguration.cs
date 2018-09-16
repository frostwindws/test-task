using Articles.Models;
using Articles.Services.Models;
using AutoMapper;

namespace ArticlesService.Initialization
{
    public class MapperConfiguration
    {
        public static void Init()
        {
            Mapper.Initialize(c =>
            {
                c.CreateMap<Article, ArticleDto>()
                    .ReverseMap();
                c.CreateMap<Comment, CommentDto>()
                    .ReverseMap()
                    .ForMember(d => d.Article, m => m.Ignore());
            });
        }
    }
}