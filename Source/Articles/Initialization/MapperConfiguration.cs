using Articles.Models;
using Articles.Services.Models;
using AutoMapper;

namespace Articles.Initialization
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