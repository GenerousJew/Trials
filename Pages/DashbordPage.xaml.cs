using LiveCharts;
using LiveCharts.Wpf;
using NewDesktop.Classes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Timers;
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
    /// Логика взаимодействия для DashbordPage.xaml
    /// </summary>
    public partial class DashbordPage : Page
    {
        private HttpClient client = new HttpClient();
        private int selProjectId;

        public DashbordPage(int ProjectId)
        {
            InitializeComponent();

            selProjectId = ProjectId;

            GetInfo();

            Timer timer = new Timer();

            timer.Interval = 30000;
            timer.Elapsed += async ( sender, e ) => GetInfo();
            timer.AutoReset = true;
            timer.Enabled = true;
        }

        private async Task GetInfo()
        {
            var responce = await client.GetAsync("http://localhost:54640/api/Task?projectId=" + selProjectId);
            var json = await responce.Content.ReadAsStringAsync();
            var tasks = JsonSerializer.Deserialize<List<TaskClass>>(json);
            var statuses = tasks.ConvertAll(x => x.StatusName);

            CircleDiagram.Series = new LiveCharts.SeriesCollection()
            {
                new PieSeries()
                {
                    Title = "Закрытые — " + string.Format("{0:F2}", ((double)statuses.Where(x => x == "Закрыта").ToList().Count / (double)statuses.Count * 100)) + "%",
                    Values = new ChartValues<int> { statuses.Where(x => x == "Закрыта").ToList().Count }
                },
                new PieSeries()
                {
                    Title = "Открытые — " + string.Format("{0:F2}", ((double)statuses.Where(x => x == "Открыта").ToList().Count / (double)statuses.Count * 100)) + "%",
                    Values = new ChartValues<int> { statuses.Where(x => x == "Открыта").ToList().Count }
                },
                new PieSeries()
                {
                    Title = "В работе — " + string.Format("{0:F2}", ((double)statuses.Where(x => x == "В работе").ToList().Count / (double)statuses.Count * 100)) + "%",
                    Values = new ChartValues<int> { statuses.Where(x => x == "В работе").ToList().Count }
                }
            };

            FirstBlock.Text = tasks.Where(x => x.FinishActualTime == null).Count().ToString();

            SecondBlock.Text = tasks.Where(x => x.FinishActualTime == null && x.Deadline < DateTime.Now).Count().ToString();

            if(Convert.ToInt32(SecondBlock.Text) > 2)
            {
                SecondGrid.Background = Brushes.Red;
            }

            ThirdBlock.Text = tasks.Where(x => x.FinishActualTime == null && x.StartActualTime < DateTime.Now).Count().ToString();

            if(ThirdBlock.Text == "0" && DateTime.Now.Hour >= 9 && DateTime.Now.Hour <= 18 && (int)DateTime.Now.DayOfWeek > 0 && (int)DateTime.Now.DayOfWeek < 6)
            {
                ThirdGrid.Background = Brushes.Red;
            }

            FourthBlock.Text = tasks.Where(x => x.FinishActualTime == null && x.StartActualTime > DateTime.Now.AddDays((double)DateTime.Now.DayOfWeek)).Count().ToString();

            HashSet<string> employees = tasks.ConvertAll(x => x.ExecuriveEmployeeFullName).ToHashSet();

            var finishList = employees.OrderBy(x => tasks.Where(y => y.ExecuriveEmployeeFullName == x && y.FinishActualTime != null && ((DateTime)y.FinishActualTime).Month == DateTime.Now.Month)).ToList();

            for (int i = 0; i < 5; i++)
            {
                if(finishList.Count > i)
                {
                    FifthBlock.Text += (i + 1) + ". " + finishList[i];
                }
            }

            employees = tasks.ConvertAll(x => x.ExecuriveEmployeeFullName).ToHashSet();

            finishList = employees.OrderBy(x => tasks.Where(y => y.ExecuriveEmployeeFullName == x && y.Deadline != null && ((DateTime)y.Deadline).Month == DateTime.Now.Month && (y.FinishActualTime > y.Deadline || y.FinishActualTime == null && y.Deadline < DateTime.Now)).Count()).ToList();

            for (int i = 0; i < 5; i++)
            {
                if (finishList.Count > i)
                {
                    SixethBlock.Text += (i + 1) + ". " + finishList[i];
                }
            }

            GetHeatDiagram();
        }

        private async void EmployeeExport(object sender, RoutedEventArgs e)
        {
            var responce = await client.GetAsync("http://localhost:54640/api/Employee");
            var json = await responce.Content.ReadAsStringAsync();
            var employees = JsonSerializer.Deserialize<List<Employee>>(json);

            StreamWriter export = new StreamWriter("Employee.csv", false, Encoding.UTF8);

            export.WriteLine("Id, FirstName, MiddleName, LastName");

            foreach (var employee in employees)
            {
                export.Write(employee.Id);
                export.Write(", ");
                export.Write(employee.FirstName);
                export.Write(", ");
                export.Write(employee.MiddleName);
                export.Write(", ");
                export.Write(employee.LastName);

                export.WriteLine();
            }

            export.Close();

            MessageBox.Show("Экспорт сотрудников выполнен!");
        }

        private async void TasksExport(object sender, RoutedEventArgs e)
        {
            var responce = await client.GetAsync("http://localhost:54640/api/Task?projectId=" + selProjectId);
            var json = await responce.Content.ReadAsStringAsync();
            var tasks = JsonSerializer.Deserialize<List<TaskClass>>(json);

            StreamWriter export = new StreamWriter("Tasks.csv");

            export.WriteLine("Id, ProjectId, FullTitle, ShortTitle, Number, Description, ExecuriveEmployeeId, StatusId, CreatedTime, UpdatedTime, DeletedTime, Deadline, StartActualTime, FinishActualTime, PreviousTaskId");

            foreach (var task in tasks)
            {
                export.WriteLine($"{task.Id}, {task.ProjectId}, {task.FullTitle}, {task.ShortTitle}, {task.Number}, {task.Description}, {task.ExecuriveEmployeeId}, {task.StatusId}, {task.CreatedTime}, {task.UpdatedTime}, {task.DeletedTime}, {task.Deadline}, {task.StartActualTime}, {task.FinishActualTime}, {task.PreviousTaskId}");
            }

            export.Close();

            MessageBox.Show("Экспорт задач выполнен!");
        }

        private async void CloseTasksExport(object sender, RoutedEventArgs e)
        {
            var responce = await client.GetAsync("http://localhost:54640/api/Task?projectId=" + selProjectId);
            var json = await responce.Content.ReadAsStringAsync();
            var tasks = JsonSerializer.Deserialize<List<TaskClass>>(json);

            tasks.Where(x => x.FinishActualTime != null && ((DateTime)x.FinishActualTime).Month == DateTime.Now.Month);

            StreamWriter export = new StreamWriter("CloseTasks.csv", false, Encoding.UTF8);

            export.WriteLine("Id, ProjectId, FullTitle, ShortTitle, Number, Description, ExecuriveEmployeeId, StatusId, CreatedTime, UpdatedTime, DeletedTime, Deadline, StartActualTime, FinishActualTime, PreviousTaskId");

            foreach (var task in tasks)
            {
                export.WriteLine($"{task.Id}, {task.ProjectId}, {task.FullTitle}, {task.ShortTitle}, {task.Number}, {task.Description}, {task.ExecuriveEmployeeId}, {task.StatusId}, {task.CreatedTime}, {task.UpdatedTime}, {task.DeletedTime}, {task.Deadline}, {task.StartActualTime}, {task.FinishActualTime}, {task.PreviousTaskId}");
            }

            export.Close();

            MessageBox.Show("Экспорт задач выполнен!");
        }

        private async void OpenTasksExport(object sender, RoutedEventArgs e)
        {
            var responce = await client.GetAsync("http://localhost:54640/api/Task?projectId=" + selProjectId);
            var json = await responce.Content.ReadAsStringAsync();
            var tasks = JsonSerializer.Deserialize<List<TaskClass>>(json);

            tasks = tasks.Where(x => x.FinishActualTime == null && x.Deadline != null && ((DateTime)x.Deadline).Month == DateTime.Now.AddMonths(1).Month).ToList();

            StreamWriter export = new StreamWriter("OpenTasks.csv", false, Encoding.UTF8);

            export.WriteLine("Id, ProjectId, FullTitle, ShortTitle, Number, Description, ExecuriveEmployeeId, StatusId, CreatedTime, UpdatedTime, DeletedTime, Deadline, StartActualTime, FinishActualTime, PreviousTaskId");

            foreach (var task in tasks)
            {
                export.WriteLine($"{task.Id}, {task.ProjectId}, {task.FullTitle}, {task.ShortTitle}, {task.Number}, {task.Description}, {task.ExecuriveEmployeeId}, {task.StatusId}, {task.CreatedTime}, {task.UpdatedTime}, {task.DeletedTime}, {task.Deadline}, {task.StartActualTime}, {task.FinishActualTime}, {task.PreviousTaskId}");
            }

            export.Close();

            MessageBox.Show("Экспорт задач выполнен!");
        }

        private async void TwoWeekTasksExport(object sender, RoutedEventArgs e)
        {
            var responce = await client.GetAsync("http://localhost:54640/api/Task?projectId=" + selProjectId);
            var json = await responce.Content.ReadAsStringAsync();
            var tasks = JsonSerializer.Deserialize<List<TaskClass>>(json);

            tasks.Where(x => x.CreatedTime != null && x.CreatedTime > DateTime.Now.AddDays(-14));

            StreamWriter export = new StreamWriter("TwoWeekTasks.csv", false, Encoding.UTF8);

            export.WriteLine("Id, ProjectId, FullTitle, ShortTitle, Number, Description, ExecuriveEmployeeId, StatusId, CreatedTime, UpdatedTime, DeletedTime, Deadline, StartActualTime, FinishActualTime, PreviousTaskId");

            foreach (var task in tasks)
            {
                export.WriteLine($"{task.Id}, {task.ProjectId}, {task.FullTitle}, {task.ShortTitle}, {task.Number}, {task.Description}, {task.ExecuriveEmployeeId}, {task.StatusId}, {task.CreatedTime}, {task.UpdatedTime}, {task.DeletedTime}, {task.Deadline}, {task.StartActualTime}, {task.FinishActualTime}, {task.PreviousTaskId}");
            }

            export.Close();

            MessageBox.Show("Экспорт задач выполнен!");
        }

        private async void GetHeatDiagram()
        {
            HeatGrid.Children.Clear();
            HeatGrid.RowDefinitions.Clear();
            HeatGrid.ColumnDefinitions.Clear();

            var responce = await client.GetAsync("http://localhost:54640/api/Task?projectId=" + selProjectId);
            var json = await responce.Content.ReadAsStringAsync();
            var tasks = JsonSerializer.Deserialize<List<TaskClass>>(json);
            tasks = tasks.Where(x => x.FinishActualTime != null).ToList();

            int max = 0;
            var DateList = tasks.ConvertAll(x => (DateTime)x.FinishActualTime);

            foreach (var date in DateList)
            {
                int taskCount = tasks.Where(x => x.FinishActualTime == date).ToList().Count;

                if(taskCount > max)
                {
                    max = taskCount;
                }
            }

            var startDate = DateTime.Now.AddDays(-70);
            var nowDate = startDate;

            HeatGrid.ColumnDefinitions.Add(new ColumnDefinition());
            HeatGrid.RowDefinitions.Add(new RowDefinition());

            string month = nowDate.ToString("MMM");
            var newLabel = new Label() { Content = month };

            for (int i = 1; nowDate <= DateTime.Now; i++, nowDate = nowDate.AddDays(7))
            {
                HeatGrid.ColumnDefinitions.Add(new ColumnDefinition());

                if(i == 1 || nowDate.ToString("MMM") != month)
                {
                    month = nowDate.ToString("MMM");
                    newLabel = new Label() { Content = month };

                    Grid.SetColumn(newLabel, i);
                    Grid.SetRow(newLabel, 0);

                    HeatGrid.Children.Add(newLabel);
                }
                else
                {
                    Grid.SetColumnSpan(newLabel, i);
                }
            }

            for (int i = 0; i < 7; i++)
            {
                HeatGrid.RowDefinitions.Add(new RowDefinition());

                newLabel = new Label() { Content = (DayOfWeek)i };

                Grid.SetColumn(newLabel, 0);
                Grid.SetRow(newLabel, i + 1);

                HeatGrid.Children.Add(newLabel);
            }

            nowDate = startDate;

            for (int i = 1; i <= HeatGrid.ColumnDefinitions.Count; i++)
            {
                while(nowDate <= DateTime.Now)
                {
                    var newBorder = new Border() { BorderBrush = Brushes.Transparent, BorderThickness = new Thickness(1), Background = (Brush)(new BrushConverter().ConvertFrom("#b6bdff")) };

                    int taskCount = tasks.Where(x => ((DateTime)x.FinishActualTime).ToString("d") == nowDate.ToString("d")).ToList().Count;

                    double taskDiv = (double)taskCount / (double)max;

                    if(taskDiv > 0.2)
                    {
                        if (taskDiv > 0.4)
                        {
                            if (taskDiv > 0.6)
                            {
                                if (taskDiv > 0.8)
                                {
                                    newBorder.Background = (Brush)(new BrushConverter().ConvertFrom("#243aff"));
                                }
                                else
                                {
                                    newBorder.Background = (Brush)(new BrushConverter().ConvertFrom("#485bff"));
                                }
                            }
                            else
                            {
                                newBorder.Background = (Brush)(new BrushConverter().ConvertFrom("#6d7cff"));
                            }
                        }
                        else
                        {
                            newBorder.Background = (Brush)(new BrushConverter().ConvertFrom("#919cff"));
                        }
                    }

                    Grid.SetColumn(newBorder, i);
                    Grid.SetRow(newBorder, (int)nowDate.DayOfWeek + 1);

                    HeatGrid.Children.Add(newBorder);

                    nowDate = nowDate.AddDays(1);
                    
                    if(nowDate.DayOfWeek == 0)
                    {
                        break;
                    }
                }
            }
        }
    }
}
