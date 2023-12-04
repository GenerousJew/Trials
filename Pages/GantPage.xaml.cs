using NewDesktop.Classes;
using System;
using System.Collections.Generic;
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
    /// Логика взаимодействия для GantPage.xaml
    /// </summary>
    public partial class GantPage : Page
    {
        private int selectedProject;
        private List<TaskClass> tasks;
        private HttpClient client = new HttpClient();
        private DateTime startDate;
        private DateTime endDate;

        public GantPage(int SelectedProject)
        {
            InitializeComponent();

            GetTasks();

            TimeBox.ItemsSource = new List<string>() { "1 неделя", "2 недели", "1 месяц", "1 год" };
            TimeBox.SelectedIndex = 0;

            selectedProject = SelectedProject;
        }

        public async void GetTasks()
        {
            var responce = await client.GetAsync("http://localhost:54640/api/Task");
            var json = await responce.Content.ReadAsStreamAsync();

            tasks = JsonSerializer.Deserialize<List<TaskClass>>(json);

            GetGant();
        }

        private void GetTimeRange()
        {
            if (TimeBox.SelectedIndex == 0)
            {
                startDate = DateTime.Now.AddDays(-((int)DateTime.Now.DayOfWeek));
                endDate = DateTime.Now.AddDays(6 - ((int)DateTime.Now.DayOfWeek));
            }
            else if (TimeBox.SelectedIndex == 1)
            {
                startDate = DateTime.Now.AddDays(-((int)DateTime.Now.DayOfWeek) - 7);
                endDate = DateTime.Now.AddDays(6 - ((int)DateTime.Now.DayOfWeek));
            }
            else if (TimeBox.SelectedIndex == 2)
            {
                startDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                endDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month));
            }
            else
            {
                startDate = new DateTime(DateTime.Now.Year, 1, 1);
                endDate = new DateTime(DateTime.Now.Year, 12, 31);
            }

            TimeRange.Content = startDate.ToString("d") + "—" + endDate.ToString("d");

            GetTasks();
        }

        private void LeftCLick(object sender, RoutedEventArgs e)
        {
            if (TimeBox.SelectedIndex == 0)
            {
                startDate = startDate.AddDays(-7);
                endDate = endDate.AddDays(-7);
            }
            else if (TimeBox.SelectedIndex == 1)
            {
                startDate = startDate.AddDays(-14);
                endDate = endDate.AddDays(-14);
            }
            else if (TimeBox.SelectedIndex == 2)
            {
                startDate = startDate.AddMonths(-1);
                endDate = new DateTime(startDate.Year, startDate.Month, DateTime.DaysInMonth(startDate.Year, startDate.Month));
            }
            else
            {
                startDate = startDate.AddYears(-1);
                endDate = endDate.AddYears(-1);
            }

            TimeRange.Content = startDate.ToString("d") + "—" + endDate.ToString("d");

            GetTasks();
        }

        private void RightClick(object sender, RoutedEventArgs e)
        {
            if (TimeBox.SelectedIndex == 0)
            {
                startDate = startDate.AddDays(7);
                endDate = endDate.AddDays(7);
            }
            else if (TimeBox.SelectedIndex == 1)
            {
                startDate = startDate.AddDays(14);
                endDate = endDate.AddDays(14);
            }
            else if (TimeBox.SelectedIndex == 2)
            {
                startDate = startDate.AddMonths(1);
                endDate = new DateTime(startDate.Year, startDate.Month, DateTime.DaysInMonth(startDate.Year, startDate.Month));
            }
            else
            {
                startDate = startDate.AddYears(1);
                endDate = endDate.AddYears(1);
            }

            TimeRange.Content = startDate.ToString("d") + "—" + endDate.ToString("d");

            GetTasks();
        }

        private void ImportCLick(object sender, RoutedEventArgs e)
        {

        }

        private void CloseClick(object sender, RoutedEventArgs e)
        {
            (Application.Current.MainWindow.FindName("ActualPage") as Frame).Content = new DashbordPage(1);
            Application.Current.MainWindow.Title = "Проектный офис" + " — Дашборд";

            Properties.Settings.Default.OpenPage = 0;
            Properties.Settings.Default.Save();
        }

        private void TimeBoxSelected(object sender, RoutedEventArgs e)
        {
            GetTimeRange();
        }

        private void GetGant()
        {
            GantGrid.Children.Clear();
            GantGrid.RowDefinitions.Clear();
            GantGrid.ColumnDefinitions.Clear();

            var TaskList = tasks.Where(x => x.ProjectId == selectedProject).ToList();

            GantGrid.RowDefinitions.Add(new RowDefinition());
            GantGrid.ColumnDefinitions.Add(new ColumnDefinition());

            DateTime DateNow = startDate;

            for (int i = 0; DateNow <= endDate; i++, DateNow = DateNow.AddDays(1))
            {
                GantGrid.ColumnDefinitions.Add(new ColumnDefinition());

                var DayBorder = new Border() { BorderBrush = Brushes.Black, BorderThickness = new Thickness(1), Margin = new Thickness(MarginSlider.Value / 2, 0, MarginSlider.Value / 2, 0) };
                var DayLabel = new Label()
                {
                    HorizontalContentAlignment = HorizontalAlignment.Center,
                    VerticalContentAlignment = VerticalAlignment.Center,
                    Content = DateNow.ToString("d") + "\n" + DateNow.ToString("ddd")
                };

                if ((int)DateNow.DayOfWeek == 0 || (int)DateNow.DayOfWeek == 6)
                {
                    DayBorder.Background = Brushes.IndianRed;
                }

                if (DateNow.ToString("d") == DateTime.Now.ToString("d"))
                {
                    DayBorder.Background = Brushes.LightGreen;
                }

                DayBorder.Child = DayLabel;

                Grid.SetRow(DayBorder, 0);
                Grid.SetColumn(DayBorder, i + 1);

                GantGrid.Children.Add(DayBorder);
            }

            foreach (var task in TaskList)
            {
                GetRow(task);
            }
        }

        private void GetRow(TaskClass nowTask)
        {
            GantGrid.RowDefinitions.Add(new RowDefinition());

            var TitleTaskBorder = new Border() { 
                BorderBrush = Brushes.Black, 
                BorderThickness = new Thickness(1), 
                Margin = new Thickness(MarginSlider.Value / 2, 0, MarginSlider.Value / 2, 0), 
                DataContext = nowTask 
            };
            var TitleTaskLabel = new Label()
            {
                HorizontalContentAlignment = HorizontalAlignment.Center,
                VerticalContentAlignment = VerticalAlignment.Center,
                Content = nowTask.FullTitle
            };

            TitleTaskBorder.MouseDown += GetTime;

            TitleTaskBorder.Child = TitleTaskLabel;

            Grid.SetRow(TitleTaskBorder, GantGrid.RowDefinitions.Count - 1);
            Grid.SetColumn(TitleTaskBorder, 0);

            GantGrid.Children.Add(TitleTaskBorder);

            var nowDate = startDate;

            for (int i = 1; nowDate <= endDate; i++, nowDate = nowDate.AddDays(1))
            {
                var TaskBorder = new Border() { BorderBrush = Brushes.Black, BorderThickness = new Thickness(1), Margin = new Thickness(MarginSlider.Value / 2, 0, MarginSlider.Value / 2, 0) };

                if ((int)nowDate.DayOfWeek == 0 || (int)nowDate.DayOfWeek == 6)
                {
                    TaskBorder.Background = Brushes.IndianRed;
                }

                if (nowDate.ToString("d") == DateTime.Now.ToString("d"))
                {
                    TaskBorder.Background = Brushes.LightGreen;
                }

                if (nowTask.StartActualTime != null)
                {
                    if (nowTask.FinishActualTime != null)
                    {
                        if (nowDate >= nowTask.StartActualTime && nowDate <= nowTask.FinishActualTime)
                        {
                            TaskBorder.Background = Brushes.LightBlue;

                            if (nowDate.ToString("d") == endDate.ToString("d") && nowDate < nowTask.FinishActualTime)
                            {
                                var ContinueLabel = new Label() { 
                                    Content = "→", 
                                    HorizontalContentAlignment = HorizontalAlignment.Center, 
                                    VerticalContentAlignment = VerticalAlignment.Center 
                                };

                                TaskBorder.Child = ContinueLabel;
                            }

                            if (nowDate.ToString("d") == startDate.ToString("d") && nowDate > nowTask.StartActualTime)
                            {
                                var ContinueLabel = new Label()
                                {
                                    Content = "←",
                                    HorizontalContentAlignment = HorizontalAlignment.Center,
                                    VerticalContentAlignment = VerticalAlignment.Center
                                };

                                TaskBorder.Child = ContinueLabel;
                            }
                        }
                    }
                    else
                    {
                        if (nowDate >= nowTask.StartActualTime && nowDate <= nowTask.Deadline)
                        {
                            TaskBorder.Background = Brushes.LightBlue;

                            if (nowDate.ToString("d") == endDate.ToString("d") && nowDate < nowTask.Deadline)
                            {
                                var ContinueLabel = new Label()
                                {
                                    Content = "→",
                                    HorizontalContentAlignment = HorizontalAlignment.Center,
                                    VerticalContentAlignment = VerticalAlignment.Center
                                };

                                TaskBorder.Child = ContinueLabel;
                            }

                            if (nowDate.ToString("d") == startDate.ToString("d") && nowDate > nowTask.StartActualTime)
                            {
                                var ContinueLabel = new Label()
                                {
                                    Content = "←",
                                    HorizontalContentAlignment = HorizontalAlignment.Center,
                                    VerticalContentAlignment = VerticalAlignment.Center
                                };

                                TaskBorder.Child = ContinueLabel;
                            }
                        }
                    }
                }
                else
                {
                    if (nowDate >= nowTask.CreatedTime && nowDate <= nowTask.Deadline)
                    {
                        TaskBorder.Background = Brushes.LightBlue;

                        if (nowDate.ToString("d") == endDate.ToString("d") && nowDate < nowTask.Deadline)
                        {
                            var ContinueLabel = new Label()
                            {
                                Content = "→",
                                HorizontalContentAlignment = HorizontalAlignment.Center,
                                VerticalContentAlignment = VerticalAlignment.Center
                            };

                            TaskBorder.Child = ContinueLabel;
                        }

                        if (nowDate.ToString("d") == startDate.ToString("d") && nowDate > nowTask.CreatedTime)
                        {
                            var ContinueLabel = new Label()
                            {
                                Content = "←",
                                HorizontalContentAlignment = HorizontalAlignment.Center,
                                VerticalContentAlignment = VerticalAlignment.Center
                            };

                            TaskBorder.Child = ContinueLabel;
                        }
                    }
                }

                Grid.SetRow(TaskBorder, GantGrid.RowDefinitions.Count - 1);
                Grid.SetColumn(TaskBorder, i);

                GantGrid.Children.Add(TaskBorder);
            }
        }

        private void GetTime(object sender, MouseButtonEventArgs e)
        {
            var SendTask = (sender as Border).DataContext as TaskClass;

            if(SendTask.StartActualTime != null)
            {
                if(SendTask.FinishActualTime != null)
                {
                    var Time = (DateTime)SendTask.FinishActualTime - (DateTime)SendTask.StartActualTime;
                    string MessageText = "";

                    if(Time.Days > 0)
                    {
                        if(Time.Minutes > 30)
                        {
                            if(Time.Hours != 23)
                            {
                                MessageText += Time.Days + " дней " + (Time.Hours + 1) + " часов";
                            }
                        }
                        else
                        {
                            if(Time.Hours != 0)
                            {
                                MessageText += Time.Days + " дней " + Time.Hours + " часов";
                            }
                            else
                            {
                                MessageText += Time.Days + " дней";
                            }
                        }
                    }
                    else if(Time.Hours > 0)
                    {
                        if (Time.Seconds > 30)
                        {
                            if (Time.Minutes != 59)
                            {
                                MessageText += Time.Hours + " часов " + (Time.Minutes + 1) + " минут";
                            }
                        }
                        else
                        {
                            if (Time.Minutes != 0)
                            {
                                MessageText += Time.Hours + " часов " + Time.Minutes + " минут";
                            }
                            else
                            {
                                MessageText += Time.Hours + " часов";
                            }
                        }
                    }
                    else if(Time.Minutes > 0)
                    {
                        if (Time.Milliseconds > 500)
                        {
                            if (Time.Seconds != 59)
                            {
                                MessageText += Time.Minutes + " минут " + (Time.Seconds + 1) + " секунд";
                            }
                        }
                        else
                        {
                            if (Time.Minutes != 0)
                            {
                                MessageText += Time.Minutes + " минут " + Time.Seconds + " секунд";
                            }
                            else
                            {
                                MessageText += Time.Minutes + " минут";
                            }
                        }
                    }
                    else
                    {
                        MessageText += Time.Seconds + " секунд";
                    }

                    MessageBox.Show(MessageText, "Фактически потраченное время");
                }
                else
                {
                    var Time = (DateTime)SendTask.Deadline - (DateTime)SendTask.StartActualTime;
                    string MessageText = "";

                    if (Time.Days > 0)
                    {
                        if (Time.Minutes > 30)
                        {
                            if (Time.Hours != 23)
                            {
                                MessageText += Time.Days + " дней " + (Time.Hours + 1) + " часов";
                            }
                        }
                        else
                        {
                            if (Time.Hours != 0)
                            {
                                MessageText += Time.Days + " дней " + Time.Hours + " часов";
                            }
                            else
                            {
                                MessageText += Time.Days + " дней";
                            }
                        }
                    }
                    else if (Time.Hours > 0)
                    {
                        if (Time.Seconds > 30)
                        {
                            if (Time.Minutes != 59)
                            {
                                MessageText += Time.Hours + " часов " + (Time.Minutes + 1) + " минут";
                            }
                        }
                        else
                        {
                            if (Time.Minutes != 0)
                            {
                                MessageText += Time.Hours + " часов " + Time.Minutes + " минут";
                            }
                            else
                            {
                                MessageText += Time.Hours + " часов";
                            }
                        }
                    }
                    else if (Time.Minutes > 0)
                    {
                        if (Time.Milliseconds > 500)
                        {
                            if (Time.Seconds != 59)
                            {
                                MessageText += Time.Minutes + " минут " + (Time.Seconds + 1) + " секунд";
                            }
                        }
                        else
                        {
                            if (Time.Minutes != 0)
                            {
                                MessageText += Time.Minutes + " минут " + Time.Seconds + " секунд";
                            }
                            else
                            {
                                MessageText += Time.Minutes + " минут";
                            }
                        }
                    }
                    else
                    {
                        MessageText += Time.Seconds + " секунд";
                    }

                    MessageBox.Show(MessageText, "Время до дедлайна");
                }
            }
            else
            {
                var Time = (DateTime)SendTask.Deadline - (DateTime)SendTask.CreatedTime;
                string MessageText = "";

                if (Time.Days > 0)
                {
                    if (Time.Minutes > 30)
                    {
                        if (Time.Hours != 23)
                        {
                            MessageText += Time.Days + " дней " + (Time.Hours + 1) + " часов";
                        }
                    }
                    else
                    {
                        if (Time.Hours != 0)
                        {
                            MessageText += Time.Days + " дней " + Time.Hours + " часов";
                        }
                        else
                        {
                            MessageText += Time.Days + " дней";
                        }
                    }
                }
                else if (Time.Hours > 0)
                {
                    if (Time.Seconds > 30)
                    {
                        if (Time.Minutes != 59)
                        {
                            MessageText += Time.Hours + " часов " + (Time.Minutes + 1) + " минут";
                        }
                    }
                    else
                    {
                        if (Time.Minutes != 0)
                        {
                            MessageText += Time.Hours + " часов " + Time.Minutes + " минут";
                        }
                        else
                        {
                            MessageText += Time.Hours + " часов";
                        }
                    }
                }
                else if (Time.Minutes > 0)
                {
                    if (Time.Milliseconds > 500)
                    {
                        if (Time.Seconds != 59)
                        {
                            MessageText += Time.Minutes + " минут " + (Time.Seconds + 1) + " секунд";
                        }
                    }
                    else
                    {
                        if (Time.Minutes != 0)
                        {
                            MessageText += Time.Minutes + " минут " + Time.Seconds + " секунд";
                        }
                        else
                        {
                            MessageText += Time.Minutes + " минут";
                        }
                    }
                }
                else
                {
                    MessageText += Time.Seconds + " секунд";
                }

                MessageBox.Show(MessageText, "Планируемое время");
            }
        }

        private void MarginChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            GetTasks();
        }
    }
}
