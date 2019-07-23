using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
using System.Xml;
using static MissionManager.UserWindow;

namespace MissionManager
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            readUserData();
        }

        private void readUserData()
        {
            XmlDocument doc = new XmlDocument();
            doc.Load("data/Users.xml");
            XmlNodeList list = doc.SelectNodes("/users/user");
            tabControl1.Items.Clear();

            foreach (XmlElement item in list)
            {
                String name = item.GetAttribute("name");
                TabItem tabItem = new TabItem();
                tabItem.Header = name;
                tabControl1.Items.Add(tabItem);

                tabItem.Content = this.createTabControlItem(item);
            }
        }

        private Grid createTabControlItem(XmlElement item)
        {
            Grid grid = new Grid();
            ColumnDefinition columnDefinition1 = new ColumnDefinition();
            ColumnDefinition columnDefinition2 = new ColumnDefinition();
            grid.ColumnDefinitions.Add(columnDefinition1);
            grid.ColumnDefinitions.Add(columnDefinition2);
            grid.Margin = new Thickness(0, 5, 15, 80);

            GridSplitter gridSplitter = new GridSplitter();
            gridSplitter.HorizontalAlignment = HorizontalAlignment.Left;
            gridSplitter.Width = 5;

            XmlNodeList weekMissionList = item.SelectNodes("weekMissions/mission");
            ScrollViewer leftScrollViewer = this.createScrollViewer(weekMissionList, "left");
            StackPanel leftStackPanel = (StackPanel)leftScrollViewer.Content;
            ((Grid)leftStackPanel.Children[0]).Margin = new Thickness(0, 0, 5, 0);

            XmlNodeList dayMissionList = item.SelectNodes("dayMissions/mission");
            ScrollViewer rigthScrollViewer = this.createScrollViewer(dayMissionList, "right");
            StackPanel rightStackPanel = (StackPanel)rigthScrollViewer.Content;
            ((Grid)rightStackPanel.Children[0]).Margin = new Thickness(10, 0, 0, 0);

            grid.Children.Add(leftScrollViewer);
            grid.Children.Add(gridSplitter);
            grid.Children.Add(rigthScrollViewer);
            Grid.SetColumn(leftScrollViewer, 0);
            Grid.SetColumn(gridSplitter, 1);
            Grid.SetColumn(rigthScrollViewer, 1);


            return grid;
        }


        private ScrollViewer createScrollViewer(XmlNodeList missionList, String location)
        {
            StackPanel stackPanel = new StackPanel();
            Grid grid = new Grid();
            grid.VerticalAlignment = VerticalAlignment.Top;
            ColumnDefinition columnDefinition = new ColumnDefinition();
            grid.ColumnDefinitions.Add(columnDefinition);

            RowDefinition textBlockRowDefinition = new RowDefinition();
            grid.RowDefinitions.Add(textBlockRowDefinition);
            TextBlock textBlock = new TextBlock();
            textBlock.Height = 30;
            textBlock.VerticalAlignment = VerticalAlignment.Center;
            textBlock.HorizontalAlignment = HorizontalAlignment.Center;
            textBlock.Foreground = Brushes.Red;
            textBlock.FontSize = 20;
            grid.Children.Add(textBlock);
            textBlock.SetValue(Grid.RowProperty, 0);
            textBlock.SetValue(Grid.ColumnProperty, 0);


            if (location == "left")
            {
                grid.Margin = new Thickness(0, 0, 5, 0);
                textBlock.Text = "周常";
            }
            else
            {
                grid.Margin = new Thickness(10, 0, 0, 0);
                textBlock.Text = "日常";
            }


            for (int i = 0; i < missionList.Count; i++)
            {
                XmlElement dayMission = (XmlElement)missionList[i];
                RowDefinition rowDefinition = new RowDefinition();
                grid.RowDefinitions.Add(rowDefinition);
                Button button = new Button();

                button.SetValue(Button.StyleProperty, Application.Current.Resources["NoMouseOverButtonStyle"]);

                button.Height = 30;
                button.Content = dayMission.GetAttribute("name");
                String status = dayMission.GetAttribute("status");
                if (status == "0")
                {
                    button.Background = Brushes.White;
                }
                else
                {
                    button.Background = Brushes.LimeGreen;
                }
                button.Click += new RoutedEventHandler(this.buttonClick);
                grid.Children.Add(button);
                button.SetValue(Grid.RowProperty, i + 1);
                button.SetValue(Grid.ColumnProperty, 0);
            }
            stackPanel.Children.Add(grid);
            ScrollViewer scrollViewer = new ScrollViewer();
            scrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            scrollViewer.Content = stackPanel;
            return scrollViewer;
        }


        protected void buttonClick(object sender, EventArgs e)
        {
            Button button = (Button)sender;
            String user = ((TabItem)((Grid)((ScrollViewer)((StackPanel)((Grid)button.Parent).Parent).Parent).Parent).Parent).Header.ToString();
            String mission = button.Content.ToString();
            String missionType = ((TextBlock)((Grid)button.Parent).Children[0]).Text;

            XmlDocument doc = new XmlDocument();
            doc.Load("data/Users.xml");
            String reg = "/users/user[@name='" + user + "']";
            XmlElement element = (XmlElement)doc.SelectSingleNode(reg);
            XmlNode missionsNode = null;
            if (missionType == "日常")
            {
                missionsNode = element.SelectSingleNode("dayMissions");
            }
            else
            {
                missionsNode = element.SelectSingleNode("weekMissions");
            }
            reg = "mission[@name='" + mission + "']";
            XmlElement missionNode = (XmlElement)missionsNode.SelectSingleNode(reg);
            String status = missionNode.GetAttribute("status");
            if (status == "0")
            {
                missionNode.SetAttribute("status", "1");
            }
            else if (status == "1")
            {
                missionNode.SetAttribute("status", "0");
            }

            doc.Save("data/Users.xml");
            this.ReloadUserMissionInfo(element);
        }


        public void ReloadUserMissionInfo(XmlElement element)
        {
            int count = tabControl1.Items.Count;
            for (int i = 0; i < count; i++)
            {
                TabItem tabItem = (TabItem)tabControl1.Items[i];
                if (tabItem.Header.ToString() == element.GetAttribute("name"))
                {
                    tabItem.Content = this.createTabControlItem(element);
                }
            }

        }

        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            UserWindow userWindow = new UserWindow();
            userWindow.readUserDataEvent += new ReadUserDataDelegate(readUserData);
            userWindow.ShowDialog();
        }
    }
}
