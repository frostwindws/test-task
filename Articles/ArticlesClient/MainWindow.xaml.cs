using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using ArticlesClient.ArticlesService;
using ArticlesClient.Models;
using AutoMapper;

namespace ArticlesClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        /// <summary>
        /// Контейнер данных для работы с окном
        /// </summary>
        private readonly ViewDataContainer viewData;

        /// <summary>
        /// Конструктор основного окна приложения
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            InitializeModelsMapping();
            viewData = (ViewDataContainer)DataContext;
            RequestArticlesList();
        }

        /// <summary>
        /// Инициализация маппинга моделей
        /// </summary>
        private void InitializeModelsMapping()
        {
            Mapper.Initialize(c =>
            {
                c.CreateMap<ArticleData, ArticleView>();
                c.CreateMap<CommentData, CommentView>();
            });
        }

        /// <summary>
        /// Запросить список статей
        /// </summary>
        private void RequestArticlesList()
        {
            using (var articlesService = new ArticlesServiceClient())
            {
                ArticleData[] articles = articlesService.GetAll();
                viewData.ArticlesList = Mapper.Map<ObservableCollection<ArticleView>>(articles.OrderByDescending(a => a.Created));
            }

        }

        /// <summary>
        /// Обработать событие нажатия на кнопку списка статей
        /// </summary>
        /// <param name="sender">Кнопка-источник события</param>
        /// <param name="e">Аргементы события</param>
        private void ArticlesListButton_OnClick(object sender, RoutedEventArgs e)
        {
            // Контекстом кнопки является статья
            ArticleView article = (ArticleView)((Button)sender).DataContext;

            using (var articlesService = new ArticlesServiceClient())
            {
                viewData.CurrentArticle = Mapper.Map<ArticleView>(articlesService.Get(article.Id));
            }
        }
    }
}
