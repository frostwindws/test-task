using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using Articles.Messaging.Converters;
using Articles.Models;
using Articles.Services.Models;
using AutoMapper;
using Serilog;

namespace Articles.Services.Executors
{
    /// <summary>
    /// Провайдер исполнителей функций сервисов.
    /// </summary>
    public class ServiceExecutorsProvider : IExecutorsProvider
    {
        private readonly DataContextFactory contextFactory;
        private readonly IMessageBodyConverter converter;
        private readonly IModelValidator<Article> articleValidator;
        private readonly IModelValidator<Comment> commentValidator;
        private Dictionary<string, ICommandExecutor> executors;

        /// <summary>
        /// Конструктор провайдера.
        /// </summary>
        /// <param name="contextFactory">Фабрика формирования контекстов.</param>
        /// <param name="converter">Конвертер тела сообщения.</param>
        /// <param name="articleValidator">Валидатор статей.</param>
        /// <param name="commentValidator">Валидатор комментариев.</param>
        public ServiceExecutorsProvider(DataContextFactory contextFactory, IMessageBodyConverter converter, IModelValidator<Article> articleValidator, IModelValidator<Comment> commentValidator)
        {
            this.contextFactory = contextFactory;
            this.converter = converter;
            this.articleValidator = articleValidator;
            this.commentValidator = commentValidator;
        }

        /// <summary>
        /// Набор исполнителей.
        /// </summary>
        private Dictionary<string, ICommandExecutor> Executors => executors ?? (executors = InitExecutorsCollection());

        /// <summary>
        /// Получить исполнителя по его имени.
        /// </summary>
        /// <param name="name">Имя исполнителя.</param>
        public ICommandExecutor Get(string name)
        {
            return Executors.TryGetValue(name, out ICommandExecutor executor) ? executor : null;
        }

        private Dictionary<string, ICommandExecutor> InitExecutorsCollection()
        {
            return new ICommandExecutor[]
            {
                new CommandExecutor("article-create", WrapInConvertion<ArticleDto, ResultDto<ArticleDto>>(CreateArticle)),
                new CommandExecutor("article-update", WrapInConvertion<ArticleDto, ResultDto<ArticleDto>>(UpdateArticle)),
                new CommandExecutor("article-delete", WrapInConvertion<ArticleDto, ResultDto<ArticleDto>>(DeleteArticle)),
                new CommandExecutor("comment-create", WrapInConvertion<CommentDto, ResultDto<CommentDto>>(CreateComment)),
                new CommandExecutor("comment-update", WrapInConvertion<CommentDto, ResultDto<CommentDto>>(UpdateComment)),
                new CommandExecutor("comment-delete", WrapInConvertion<CommentDto, ResultDto<CommentDto>>(DeleteComment))
            }.ToDictionary(e => e.Name);
        }

        /// <summary>
        /// Оббертка метода в дополнительную конвертацию.
        /// </summary>
        /// <typeparam name="TIn">Тип входящего параметра метода.</typeparam>
        /// <typeparam name="TOut">Тип исходящего параметра метода.</typeparam>
        /// <param name="func">Исходная функция.</param>
        /// <returns>Функция оббертки в конвертацию.</returns>
        private Func<byte[], byte[]> WrapInConvertion<TIn, TOut>(Func<TIn, TOut> func)
        {
            const string ErrorLogTemplate = "Error accured while executing {MethodName}";
            return (body) =>
            {
                TIn input = converter.FromBody<TIn>(body);
                try
                {
                    TOut result = func.Invoke(input);
                    return converter.ToBody(result);
                }
                catch (TimeoutException e)
                {
                    Log.Error(e, ErrorLogTemplate, func.Method.Name);
                }
                catch (DbException e)
                {
                    Log.Error(e, ErrorLogTemplate, func.Method.Name);
                }

                return null;
            };
        }

        /// <summary>
        /// Метод создания статьи.
        /// </summary>
        /// <param name="article">Создаваемая статья.</param>
        /// <returns>Результат выполнения операции.</returns>
        private ResultDto<ArticleDto> CreateArticle(ArticleDto article)
        {
            using (IDataContext context = contextFactory.GetContext())
            {
                if (article == null)
                {
                    return ResultBuilder.Fault<ArticleDto>("The article is empty");
                }

                Article dbArticle = Mapper.Map<Article>(article);
                IEnumerable<string> errors = articleValidator.GetErrors(context.Articles, dbArticle);
                if (errors.Any())
                {
                    return ResultBuilder.Fault<ArticleDto>($"Validation failure:\r\n\t{string.Join(";\r\n\t", errors)}");
                }
                else
                {
                    var createdArticle = context.Articles.Create(dbArticle);
                    context.Commit();
                    return ResultBuilder.Success(Mapper.Map<ArticleDto>(createdArticle));
                }
            }
        }

        /// <summary>
        /// Метод обновления статьи.
        /// </summary>
        /// <param name="article">Обновляемая статья.</param>
        /// <returns>Результат выполнения операции.</returns>
        private ResultDto<ArticleDto> UpdateArticle(ArticleDto article)
        {
            using (IDataContext context = contextFactory.GetContext())
            {
                if (article == null)
                {
                    return ResultBuilder.Fault<ArticleDto>("The article is empty");
                }

                Article dbArticle = Mapper.Map<Article>(article);
                IEnumerable<string> errors = articleValidator.GetErrors(context.Articles, dbArticle);
                if (errors.Any())
                {
                    return ResultBuilder.Fault<ArticleDto>($"Validation failure:\r\n\t{string.Join(";\r\n\t", errors)}");
                }
                else
                {
                    var updatedArticle = context.Articles.Update(dbArticle);
                    context.Commit();
                    return ResultBuilder.Success(Mapper.Map<ArticleDto>(updatedArticle));
                }
            }
        }

        /// <summary>
        /// Метод удаления статьи.
        /// </summary>
        /// <param name="article">Удаляемая статья.</param>
        /// <returns>Результат выполнения операции.</returns>
        private ResultDto<ArticleDto> DeleteArticle(ArticleDto article)
        {
            using (IDataContext context = contextFactory.GetContext())
            {
                context.Articles.Delete(article.Id);
                context.Commit();
                return ResultBuilder.Success<ArticleDto>(null);
            }
        }

        /// <summary>
        /// Метод создания комментария.
        /// </summary>
        /// <param name="comment">Создаваемый коментарий.</param>
        /// <returns>Результат выполнения операции.</returns>
        private ResultDto<CommentDto> CreateComment(CommentDto comment)
        {
            using (IDataContext context = contextFactory.GetContext())
            {
                if (comment == null)
                {
                    return ResultBuilder.Fault<CommentDto>("The comment is empty");
                }

                Comment dbComment = Mapper.Map<Comment>(comment);
                IEnumerable<string> errors = commentValidator.GetErrors(context.Comments, dbComment);
                if (errors.Any())
                {
                    return ResultBuilder.Fault<CommentDto>($"Validation failure:\r\n\t{string.Join(";\r\n\t", errors)}");
                }
                else
                {
                    if (dbComment.Article == null)
                    {
                        dbComment.Article = new Article { Id = comment.ArticleId };
                    }

                    var createdComment = context.Comments.Create(dbComment);
                    context.Commit();
                    return ResultBuilder.Success(Mapper.Map<CommentDto>(createdComment));
                }
            }
        }

        /// <summary>
        /// Метод обновления комментария.
        /// </summary>
        /// <param name="comment">Обновляемый комментарий.</param>
        /// <returns>Результат выполнения операции.</returns>
        private ResultDto<CommentDto> UpdateComment(CommentDto comment)
        {
            using (IDataContext context = contextFactory.GetContext())
            {
                if (comment == null)
                {
                    return ResultBuilder.Fault<CommentDto>("The comment is empty");
                }

                Comment dbComment = Mapper.Map<Comment>(comment);
                IEnumerable<string> errors = commentValidator.GetErrors(context.Comments, dbComment);
                if (errors.Any())
                {
                    return ResultBuilder.Fault<CommentDto>($"Validation failure:\r\n\t{string.Join(";\r\n\t", errors)}");
                }
                else
                {
                    var updatedComment = context.Comments.Update(dbComment);
                    context.Commit();
                    return ResultBuilder.Success(Mapper.Map<CommentDto>(updatedComment));
                }
            }
        }

        /// <summary>
        /// Удаление комментария.
        /// </summary>
        /// <param name="comment">Удаляемый коментарий.</param>
        /// <returns>Результат выполнения операции.</returns>
        private ResultDto<CommentDto> DeleteComment(CommentDto comment)
        {
            using (IDataContext context = contextFactory.GetContext())
            {
                context.Comments.Delete(comment.Id);
                context.Commit();
                return ResultBuilder.Success<CommentDto>(null);
            }
        }
    }
}