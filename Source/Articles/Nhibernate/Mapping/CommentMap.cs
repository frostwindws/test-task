using Articles.Models;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;

namespace Articles.Nhibernate.Mapping
{
    /// <summary>
    /// Класс маппинга для комментария
    /// </summary>
    internal class CommentMap: ClassMapping<Comment>
    {
        /// <summary>
        /// Конструктор маппинга
        /// </summary>
        public CommentMap()
        {
            Table("comments");
            Id(a => a.Id, m => { m.Generator(Generators.Sequence, g => g.Params(new { sequence = "seq_id" })); });
            Property(a => a.Author,
                m =>
                {
                    m.Update(false);
                    m.NotNullable(true);
                    m.Length(200);
                });
            Property(a => a.Content, m => m.NotNullable(true));
            Property(a => a.Created, m => m.Generated(PropertyGeneration.Always));

            Property(a => a.ArticleId, m => m.Generated(PropertyGeneration.Always));
            ManyToOne(c => c.Article, m =>
            {
                m.Update(false);
                m.Cascade(Cascade.DeleteOrphans);
                m.Column("articleid");
            });
        }
    }
}