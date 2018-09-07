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
                c.CreateMap<Article, ArticleData>();
                c.CreateMap<Comment, CommentData>();
            });
        }
    }
}