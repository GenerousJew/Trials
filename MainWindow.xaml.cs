using NewDesktop.Classes;
using NewDesktop.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Resources.Extensions;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace NewDesktop
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public HttpClient Client = new HttpClient();
        public List<Page> PageList = new List<Page>()
        {
            new DashbordPage(1),
            new GantPage(1),
            new TasksPage(1)
        };

        public MainWindow()
        {
            InitializeComponent();
            GetProjects();

            Properties.Settings.Default.AssemblyNumber++;
            Properties.Settings.Default.Save();

            VersionLabel.Content = "1.0." + Convert.ToString(Properties.Settings.Default.AssemblyNumber);
            ActualPage.Content = PageList[Properties.Settings.Default.OpenPage];

            switch (Properties.Settings.Default.OpenPage)
            {
                case 0:
                    this.Title = "Проектный офис" + " — Дашборд";
                    break;
                case 1:
                    this.Title = "Проектный офис" + " — Гант";
                    break;
                case 2:
                    this.Title = "Проектный офис" + " — Задачи";
                    break;
            }
        }

        private void OpenDashboardClick(object sender, RoutedEventArgs e)
        {
            ActualPage.Content = PageList[0];
            this.Title = "Проектный офис" + " — Дашборд";

            Properties.Settings.Default.OpenPage = 0;
            Properties.Settings.Default.Save();
        }

        private void OpenTasksClick(object sender, RoutedEventArgs e)
        {
            ActualPage.Content = PageList[2];
            this.Title = "Проектный офис" + " — Задачи";

            Properties.Settings.Default.OpenPage = 2;
            Properties.Settings.Default.Save();
        }

        private void OpenGantClick(object sender, RoutedEventArgs e)
        {
            ActualPage.Content = PageList[1];
            this.Title = "Проектный офис" + " — Гант";

            Properties.Settings.Default.OpenPage = 1;
            Properties.Settings.Default.Save();
        }

        public async void GetProjects()
        {
            try
            {
                var responce = await Client.GetAsync("http://localhost:54640/api/Project");
                var json = await responce.Content.ReadAsStreamAsync();
                var projectList = JsonSerializer.Deserialize<List<Project>>(json);

                ProjectList.ItemsSource = projectList;
            }
            catch (Exception)
            {
                MessageBox.Show("Ошибка подключения к серверу");
                throw;
            }
        }

        private void ProjectSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var SelectedProject = (sender as ListView).SelectedItem as Project; 

            PageList[2] = new TasksPage(SelectedProject.Id);
            PageList[0] = new DashbordPage(SelectedProject.Id);
            PageList[1] = new GantPage(SelectedProject.Id);

            if (Properties.Settings.Default.OpenPage == 2)
            {
                ActualPage.Content = PageList[2];
                this.Title = "Проектный офис" + " — Задачи";
            }
            else if(Properties.Settings.Default.OpenPage == 0)
            {
                ActualPage.Content = PageList[0];
                this.Title = "Проектный офис" + " — Дашборд";
            }
            else
            {
                ActualPage.Content = PageList[1];
                this.Title = "Проектный офис" + " — Гант";
            }
        }
    }
}
