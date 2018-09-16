using Articles.Models;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;
using PropertyGeneration = NHibernate.Mapping.ByCode.PropertyGeneration;

namespace ArticlesService.Nhibernate.Mapping
{
    /// <summary>
    /// Класс маппинга для статьи
    /// </summary>
    internal class ArticleMap : ClassMapping<Article>
    {
        /// <summary>
        /// Конструктор маппинга
        /// </summary>
        public ArticleMap()
        {
            Table("articles");
            Id(a => a.Id, m => { m.Generator(Generators.Sequence, g => g.Params(new { sequence = "seq_id" })); });
            Property(a => a.Title, m =>
            {
                m.NotNullable(true);
                m.Length(200);
            });
            Property(a => a.Author, m =>
            {
                m.NotNullable(true);
                m.Update(false);
                m.Length(200);
            });
            Property(a => a.Content, m => m.NotNullable(true));
            Property(a => a.Created, m => m.Generated(PropertyGeneration.Always));
        }
    }
}