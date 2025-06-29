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
        private int messageCount = 0;
        private readonly object summaryLock = new object();

        public MainWindow()
        {
            InitializeComponent();
            ThemeButton.Click += ToggleTheme;
            SendButton.Click += (s, e) => SendMessage();
            CloseButton.Click += CloseButton_Click;
        }

        private void InputBox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                SendMessage();
                e.Handled = true;
            }
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

        private void UpdateSummary(string userMessage)
        {
            Dispatcher.Invoke(() =>
            {
                lock (summaryLock)
                {
                    messageCount++;
                    string newSummary = $"{messageCount}. {userMessage}\n";

                    // Append new summary to existing text
                    if (!string.IsNullOrEmpty(SummaryTextBlock.Text))
                    {
                        SummaryTextBlock.Text += newSummary;
                    }
                    else
                    {
                        SummaryTextBlock.Text = newSummary;
                    }
                }
            });
        }

        private void SendMessage(object? sender = null, RoutedEventArgs? e = null)
        {
            string message = InputBox.Text.Trim();
            if (!string.IsNullOrWhiteSpace(message))
            {
                // Add user message to chat
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
                
                // Add to summary
                UpdateSummary(message);
                
                // Clear input
                InputBox.Text = "";
                
                // In a real app, you would process the message and get a response here
                // For now, we'll simulate a simple response
                string response = $"{message}에 대한 응답입니다.";
                
                // Add assistant response to chat
                Dispatcher.Invoke(() =>
                {
                    Border responseBorder = new Border
                    {
                        CornerRadius = new CornerRadius(8),
                        Background = new SolidColorBrush(Color.FromArgb(0xC0, 0xFF, 0xFF, 0xFF)),
                        Margin = new Thickness(0, 5, 0, 5),
                        Padding = new Thickness(10),
                        HorizontalAlignment = HorizontalAlignment.Left
                    };
                    StackPanel responseStack = new StackPanel { Orientation = Orientation.Horizontal };
                    Ellipse assistantIcon = new Ellipse { Width = 24, Height = 24, Fill = new SolidColorBrush(Colors.Blue), Margin = new Thickness(0, 0, 10, 0) };
                    TextBlock responseText = new TextBlock { Text = response, VerticalAlignment = VerticalAlignment.Center };
                    responseStack.Children.Add(assistantIcon);
                    responseStack.Children.Add(responseText);
                    responseBorder.Child = responseStack;
                    MessageList.Children.Add(responseBorder);
                    
                    // No need to add assistant response to summary
                    
                    // Scroll to bottom
                    MessageScrollViewer.ScrollToEnd();
                });
            }
        }
    }
}
