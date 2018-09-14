using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Runtime.CompilerServices;
using Articles.Models;
using Articles.Services.Models;
using AutoMapper;
using Nelibur.Sword.Extensions;
using Serilog;

namespace Articles.Services
{
    /// <summary>
    /// Сервис работы со статьями.
    /// </summary>
    public class ArticlesService : IArticlesService, IDisposable
    {
        /// <summary>
        /// Валидатор статей.
        /// </summary>
        private readonly IModelValidator<Article> validator;

        /// <summary>
        /// Контекст работы с данными.
        /// </summary>
        private readonly IDataContext context;

        /// <summary>
        /// Конструктор сервиса.
        /// </summary>
        /// <param name="context">Используемый контекст.</param>
        /// <param name="validator">Используемый валидатор статей.</param>
        public ArticlesService(IDataContext context, IModelValidator<Article> validator)
        {
            this.context = context;
            this.validator = validator;
        }

        /// <summary>
        /// Оббертка для выполнения операция со стандартным набором обработчиков ошибок.
        /// </summary>
        /// <typeparam name="T">Тип передаваемых на выходе данных.</typeparam>
        /// <param name="function">Выполняемая функция.</param>
        /// <param name="callerName">Имя вызывающего метода (используется для логирования).</param>
        /// <returns>Результат выполнения операции.</returns>
        private ResultDto<T> SafeExecute<T>(Func<ResultDto<T>> function, [CallerMemberName] string callerName = "")
        {
            const string ErrorLogTemplate = "Error accured while executing \"ArticleService\".{MethodName}";

            try
            {
                return function.Invoke();
            }
            catch (TimeoutException e)
            {
                Log.Error(e, ErrorLogTemplate, callerName);
                return ResultBuilder.Fault<T>("Database connection timeout. Please try again later");
            }
            catch (DbException e)
            {
                Log.Error(e, ErrorLogTemplate, callerName);
                return ResultBuilder.Fault<T>($"Unexpected database exception: {e.GetBaseException().Message}.\r\nPlease try again later");
            }
        }

        /// <inheritdoc />
        public ResultDto<ArticleDto[]> GetAll()
        {
            return SafeExecute(() =>
            {
                IEnumerable<Article> articles = context.Articles.GetCollection().OrderByDescending(a => a.Created);
                return ResultBuilder.Success(Mapper.Map<ArticleDto[]>(articles));
            });
        }

        /// <inheritdoc />
        public ResultDto<ArticleDto> Get(long id)
        {
            return SafeExecute(() =>
            {
                Article article = context.Articles.Get(id);
                ArticleDto data = Mapper.Map<ArticleDto>(article);
                if (article == null)
                {
                    return ResultBuilder.Fault<ArticleDto>("Article wasn't found");
                }

                IEnumerable<Comment> comments = context.Comments.GetForArticle(article.Id);
                data.Comments = Mapper.Map<CommentDto[]>(comments);
                return ResultBuilder.Success(data);
            });
        }

        /// <inheritdoc />
        public ResultDto<ArticleDto> Create(ArticleDto article)
        {
            return SafeExecute(() =>
            {
                ResultDto<ArticleDto> result = article
                    .ToOption()
                    .DoOnEmpty(() => result = ResultBuilder.Fault<ArticleDto>("The article is empty"))
                    .Map(a => Mapper.Map<Article>(article))
                    .Map(a =>
                    {
                        IEnumerable<string> errors = validator.GetErrors(context.Articles, a);
                        if (errors.Any())
                        {
                            return ResultBuilder.Fault<ArticleDto>($"Validation failure:\r\n\t{string.Join(";\r\n\t", errors)}");
                        }
                        else
                        {
                            var createdArticle = context.Articles.Create(a);
                            context.Commit();
                            return ResultBuilder.Success(Mapper.Map<ArticleDto>(createdArticle));
                        }
                    }).Value;

                return result;
            });
        }

        /// <inheritdoc />
        public ResultDto<ArticleDto> Update(ArticleDto article)
        {
            return SafeExecute(() =>
            {
                ResultDto<ArticleDto> result = article
                    .ToOption()
                    .DoOnEmpty(() => result = ResultBuilder.Fault<ArticleDto>("The article is empty"))
                    .Map(a => Mapper.Map<Article>(article))
                    .Map(a =>
                    {
                        IEnumerable<string> errors = validator.GetErrors(context.Articles, a);
                        if (errors.Any())
                        {
                            return ResultBuilder.Fault<ArticleDto>($"Validation failure:\r\n\t{string.Join(";\r\n\t", errors)}");
                        }
                        else
                        {
                            var updatedArticle = context.Articles.Update(a);
                            context.Commit();
                            return ResultBuilder.Success(Mapper.Map<ArticleDto>(updatedArticle));
                        }
                    }).Value;

                return result;
            });
        }

        /// <inheritdoc />
        public ResultDto<ArticleDto> Delete(long id)
        {
            return SafeExecute(() =>
            {
                context.Articles.Delete(id);
                context.Commit();
                return ResultBuilder.Success<ArticleDto>(null);
            });
        }

        /// <inheritdoc />
        public void Dispose()
        {
            context?.Dispose();
        }
    }
}
