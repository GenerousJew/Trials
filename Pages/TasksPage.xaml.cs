using NewDesktop.Classes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
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

namespace NewDesktop.Pages
{
    /// <summary>
    /// Логика взаимодействия для TasksPage.xaml
    /// </summary>
    public partial class TasksPage : Page
    {
        public HttpClient Client = new HttpClient();
        private List<TaskClass> tasks;
        private TaskClass selectedTask = null;
        private List<Employee> employees;
        private List<TaskStatusClass> statuses;
        public int SelProjectId = 0;

        public TasksPage(int ProjectId)
        {
            InitializeComponent();

            SelProjectId = ProjectId;

            GetTasks(ProjectId);
            GetEmployees();
            GetStatuses();
        }

        public async void GetTasks(int ProjectId)
        {
            try
            {
                var responce = await Client.GetAsync("http://localhost:54640/api/Task?projectId=" + Convert.ToString(ProjectId));
                var json = await responce.Content.ReadAsStreamAsync();
                var TaskList = JsonSerializer.Deserialize<List<TaskClass>>(json);

                var FinishTaskList = TaskList.Where(x => x.StatusName == "В работе" && x.Deadline < DateTime.Now).ToList();
                FinishTaskList.AddRange(TaskList.Where(x => x.StatusName == "Открыта" && x.Deadline < DateTime.Now).ToList());
                FinishTaskList.AddRange(TaskList.Where(x => x.StatusName == "В работе" && x.Deadline >= DateTime.Now).ToList());
                FinishTaskList.AddRange(TaskList.Where(x => x.StatusName == "Открыта" && x.Deadline >= DateTime.Now).ToList());

                if(SearchText.Text == "" || SearchText.Text == null)
                {
                    TaskView.ItemsSource = FinishTaskList;
                    tasks = FinishTaskList;
                }
                else
                {
                    var SearchList = tasks.Where(x => x.FullTitle.Contains(SearchText.Text)).ToList();
                    SearchList.AddRange(tasks.Where(x => !x.FullTitle.Contains(SearchText.Text) && x.Description != null && x.Description.Contains(SearchText.Text)).ToList());

                    TaskView.ItemsSource = SearchList;
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Ошибка подключения к серверу");
                throw;
            }
        }

        private void SearchClick(object sender, RoutedEventArgs e)
        {
            var SearchList = tasks.Where(x => x.FullTitle.Contains(SearchText.Text)).ToList();
            SearchList.AddRange(tasks.Where(x => !x.FullTitle.Contains(SearchText.Text) && x.Description != null && x.Description.Contains(SearchText.Text)).ToList());

            TaskView.ItemsSource = SearchList;
        }

        private async void TaskSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedTaskFromList = (sender as ListView).SelectedItem as TaskClass;
            
            if (selectedTaskFromList != null)
            {
                var responce = await Client.GetAsync("http://localhost:54640/api/StatusHistory?taskId=" + selectedTaskFromList.Id);
                var json = await responce.Content.ReadAsStringAsync();
                var statusHistory = JsonSerializer.Deserialize<List<StatusHistory>>(json);

                TaskInfoPanel.Visibility = Visibility.Visible;
                TaskInfoPanel.DataContext = selectedTaskFromList;

                TaskScroll.Width = 700;

                StatusBox.ItemsSource = statuses;
                TaskEmployee.ItemsSource = employees;
                StatusHistoryGrid.ItemsSource = statusHistory;

                StatusBox.SelectedItem = statuses.FirstOrDefault(x => x.Id == selectedTaskFromList.StatusId);
                TaskEmployee.SelectedItem = employees.FirstOrDefault(x => x.Id == selectedTaskFromList.ExecuriveEmployeeId);
            }

            this.selectedTask = selectedTaskFromList;
        }

        private void AddClick(object sender, RoutedEventArgs e)
        {
            TaskInfoPanel.Visibility = Visibility.Visible;
            TaskInfoPanel.DataContext = new TaskClass();

            TaskScroll.Width = 700;

            StatusBox.ItemsSource = statuses;
            TaskEmployee.ItemsSource = employees;
        }

        private async void DeleteClick(object sender, RoutedEventArgs e)
        {
            var confirmation = MessageBox.Show("Вы уверены, что хотите удалить данную задачу?", "", MessageBoxButton.YesNoCancel, MessageBoxImage.Warning);

            if(((int)confirmation) == 6)
            {
                if (selectedTask != null)
                {
                    var responce = await Client.DeleteAsync("http://localhost:54640/api/Task/" + Convert.ToString(selectedTask.Id));

                    TaskInfoPanel.Visibility = Visibility.Hidden;
                    TaskInfoPanel.DataContext = null;

                    TaskScroll.Width = 1000;

                    GetTasks(SelProjectId);
                }
                else
                {
                    MessageBox.Show("Задача не выбрана");
                }
            }
        }

        private async void SaveClick(object sender, RoutedEventArgs e)
        {
            var EditTask = TaskInfoPanel.DataContext as TaskClass;

            try
            {
                EditTask.StatusId = (StatusBox.SelectedItem as TaskStatusClass).Id;
                EditTask.ExecuriveEmployeeId = (TaskEmployee.SelectedItem as Employee).Id;

                if (EditTask.Id > 0)
                {
                    var json = JsonSerializer.Serialize(EditTask);

                    var responce = await Client.PutAsync("http://localhost:54640/api/Task", new StringContent(json, Encoding.UTF8, "application/json"));

                    GetTasks(SelProjectId);
                }
                else
                {
                    EditTask.ProjectId = SelProjectId;

                    var json = JsonSerializer.Serialize(EditTask);

                    var responce = await Client.PostAsync("http://localhost:54640/api/Task", new StringContent(json, Encoding.UTF8, "application/json"));

                    GetTasks(SelProjectId);
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Ошибка! Проверьте введенные данные");
            }
        }

        public async void GetEmployees()
        {
            var responce = await Client.GetAsync("http://localhost:54640/api/Employee");
            var employeesList = await responce.Content.ReadAsStringAsync();
            var json = JsonSerializer.Deserialize<List<Employee>>(employeesList);

            employees = json;
        }

        public async void GetStatuses()
        {
            var responce = await Client.GetAsync("http://localhost:54640/api/TaskStatus");
            var statusList = await responce.Content.ReadAsStringAsync();
            var json = JsonSerializer.Deserialize<List<TaskStatusClass>>(statusList);

            statuses = json;
        }
    }
}
