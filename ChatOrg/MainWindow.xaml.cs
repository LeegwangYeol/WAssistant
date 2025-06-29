using System.Printing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace ChatOrg
{
    public partial class MainWindow : Window
    {
        private bool isDarkTheme = false;

        public MainWindow()
        {
            InitializeComponent();
            ThemeButton.Click += ToggleTheme;
            SendButton.Click += SendMessage;
            CloseButton.Click += CloseButton_Click;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            // 애플리케이션 종료
            Application.Current.Shutdown();
        }

        private void ToggleTheme(object sender, RoutedEventArgs e)
        {
            isDarkTheme = !isDarkTheme;
            // 테마 전환 로직 예시 (실제로는 ResourceDictionary 교체 등)
            if (isDarkTheme)
            {
                // 다크 테마 적용 예시
                // (실제로는 배경색, 텍스트색 등 변경)
            }
            else
            {
                // 라이트 테마 적용 예시
            }
        }

        private void SendMessage(object sender, RoutedEventArgs e)
        {
            string message = InputBox.Text;
            if (!string.IsNullOrWhiteSpace(message))
            {
                Border msgBorder = new Border
                {
                    CornerRadius = new CornerRadius(8),
                    Background = new SolidColorBrush(Color.FromArgb(0xC0, 0xE3, 0xF2, 0xFE)),
                    Margin = new Thickness(0, 5, 0, 5),
                    Padding = new Thickness(10),
                    HorizontalAlignment = HorizontalAlignment.Right
                };
                StackPanel msgStack = new StackPanel { Orientation = Orientation.Horizontal };
                TextBlock msgText = new TextBlock { Text = message, VerticalAlignment = VerticalAlignment.Center };
                Ellipse userIcon = new Ellipse { Width = 24, Height = 24, Fill = new SolidColorBrush(Colors.Green), Margin = new Thickness(10, 0, 0, 0) };
                msgStack.Children.Add(msgText);
                msgStack.Children.Add(userIcon);
                msgBorder.Child = msgStack;
                MessageList.Children.Add(msgBorder);
                InputBox.Text = "";
            }
        }
    }
}
