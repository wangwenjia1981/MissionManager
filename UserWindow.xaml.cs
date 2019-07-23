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
using System.Windows.Shapes;
using System.Xml;

namespace MissionManager
{
    /// <summary>
    /// UserWindow.xaml 的交互逻辑
    /// </summary>
    public partial class UserWindow : Window
    {

        public delegate void ReadUserDataDelegate();

        public event ReadUserDataDelegate readUserDataEvent;

        public UserWindow()
        {
            InitializeComponent();
            InitComboBox();
        }

        private void InitComboBox()
        {
            comboBox.Items.Clear();
            XmlDocument doc = new XmlDocument();
            doc.Load("data/Users.xml");
            XmlNodeList userList = doc.SelectNodes("/users/user");
            foreach(XmlElement user in userList)
            {
                comboBox.Items.Add(user.GetAttribute("name"));
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            String name = textBox.Text;
            if (string.IsNullOrWhiteSpace(name))
            {
                MessageBox.Show("角色名不能为空");
            }
            else
            {
                XmlDocument doc = new XmlDocument();
                doc.Load("data/Users.xml");
                XmlElement root = doc.DocumentElement;
                XmlElement newUserNode = (XmlElement)doc.CreateNode("element", "user", "");
                newUserNode.SetAttribute("name",name);
                newUserNode.AppendChild(doc.CreateNode("element", "dayMissions", ""));
                newUserNode.AppendChild(doc.CreateNode("element", "weekMissions", ""));
                root.AppendChild(newUserNode);
                doc.Save("data/Users.xml");
                InitComboBox();
                readUserDataEvent();
            }            
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            object item = comboBox.SelectedItem;
            if (item!=null)
            {
                XmlDocument doc = new XmlDocument();
                doc.Load("data/Users.xml");
                XmlElement root = doc.DocumentElement;
                String reg = "/users/user[@name='" + item.ToString() + "']";
                XmlNode userNode = root.SelectSingleNode(reg);
                root.RemoveChild(userNode);
                doc.Save("data/Users.xml");
                InitComboBox();
                readUserDataEvent();
            }
        }

    }
}
