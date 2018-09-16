using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Runtime.CompilerServices;
using Articles.Models;
using Articles.Services.Executors;
using Articles.Services.Models;
using AutoMapper;
using Nelibur.Sword.Extensions;
using Serilog;

namespace Articles.Services
{
    /// <summary>
    /// Сервис работы с комментариями к статьям.
    /// </summary>
    public class CommentsService : ICommentsService, IDisposable
    {
        /// <summary>
        /// Валидатор комментариев.
        /// </summary>
        private readonly IModelValidator<Comment> validator;

        /// <summary>
        /// Контекст работы с данными.
        /// </summary>
        private readonly IDataContext context;

        /// <summary>
        /// Исполнитель команд обновления данных
        /// </summary>
        private readonly UpdateCommandsInvoker commandsInvoker;

        /// <summary>
        /// Конструктор сервиса.
        /// </summary>
        /// <param name="context">Используемый контекст.</param>
        /// <param name="validator">Валидатор для комментариев.</param>
        public CommentsService(IDataContext context, IModelValidator<Comment> validator)
        {
            this.context = context;
            this.validator = validator;
            commandsInvoker = new UpdateCommandsInvoker();
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
            const string ErrorLogTemplate = "Error accured while executing \"CommentsService\".{MethodName}";
            try
            {
                return function.Invoke();
            }
            catch (TimeoutException e)
            {
                Log.Error(e, ErrorLogTemplate, callerName);
                return ResultBuilder.Fault<T>("Database connection timeout. Please try again later");
            }
            catch (ArgumentException e)
            {
                Log.Error(e, ErrorLogTemplate, callerName);
                return ResultBuilder.Fault<T>(e.GetBaseException().Message);
            }
            catch (DbException e)
            {
                Log.Error(e, ErrorLogTemplate, callerName);
                return ResultBuilder.Fault<T>($"Unexpected database exception: {e.GetBaseException().Message}.\r\nPlease try again later");
            }
        }

        /// <summary>
        /// Получить список комментариев.
        /// </summary>
        /// <returns>Возвращает массив всех имеющихся статей.</returns>
        public ResultDto<CommentDto[]> GetAll()
        {
            return SafeExecute(() =>
            {
                IOrderedEnumerable<Comment> comments = context.Comments.GetCollection().OrderByDescending(a => a.Created);
                return ResultBuilder.Success(Mapper.Map<CommentDto[]>(comments));
            });
        }

        /// <summary>
        /// Получить отдельный комментарий.
        /// </summary>
        /// <param name="id">Идентификатор комментария.</param>
        /// <returns>Возвращает объект запрашиваемого комментария.</returns>
        public ResultDto<CommentDto> Get(long id)
        {
            return SafeExecute(() =>
            {
                Comment comment = context.Comments.Get(id);
                CommentDto data = Mapper.Map<CommentDto>(comment);
                return comment == null
                    ? ResultBuilder.Fault<CommentDto>("Comment wasn't found")
                    : ResultBuilder.Success(data);
            });
        }

        /// <summary>
        /// Запрос списка комментариев к статье.
        /// </summary>
        /// <param name="id">Идентификатор статьи.</param>
        /// <returns>Возвращает массив комментариев к указанной статье.</returns>
        public ResultDto<CommentDto[]> GetForArticle(long articleid)
        {
            return SafeExecute(() =>
            {
                IEnumerable<Comment> comments = context.Comments.GetForArticle(articleid).OrderByDescending(a => a.Created);
                return ResultBuilder.Success(Mapper.Map<CommentDto[]>(comments));
            });
        }

        /// <summary>
        /// Создание комментария.
        /// </summary>
        /// <param name="comment">Создаваемый комментарий.</param>
        /// <returns></returns>
        public ResultDto<CommentDto> Create(CommentDto comment)
        {
            return SafeExecute(() => {
                Comment dbComment = Mapper.Map<Comment>(comment);
                dbComment = commandsInvoker.ExecuteForComment("comment-create", context, dbComment, validator);
                return ResultBuilder.Success(Mapper.Map<CommentDto>(dbComment));
            });
        }

        /// <summary>
        /// Обновление имеющегося комментария.
        /// </summary>
        /// <param name="comment">Обновляемый комментарий.</param>
        /// <returns></returns>
        public ResultDto<CommentDto> Update(CommentDto comment)
        {
            return SafeExecute(() => {
                Comment dbComment = Mapper.Map<Comment>(comment);
                dbComment = commandsInvoker.ExecuteForComment("comment-update", context, dbComment, validator);
                return ResultBuilder.Success(Mapper.Map<CommentDto>(dbComment));
            });
        }

        /// <summary>
        /// Удаление комментария.
        /// </summary>
        /// <param name="id">Идентификатор удаляемого комментария.</param>
        public ResultDto<CommentDto> Delete(long id)
        {
            return SafeExecute(() => {
                Comment dbComment = new Comment() { Id = id };
                commandsInvoker.ExecuteForComment("comment-delete", context, dbComment, validator);
                return ResultBuilder.Success<CommentDto>(null);
            });
        }

        /// <summary>
        /// Освобождение ресурсов.
        /// </summary>
        public void Dispose()
        {
            context?.Dispose();

        }
    }
}
