using System.Printing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using ChatOrg;

namespace ChatOrg
{
    public partial class MainWindow : Window
    {
        private bool isDarkTheme = false;
        private int messageCount = 0;
        private readonly object summaryLock = new object();
        private TextBox? _summaryTextBox;
        private ScrollViewer? _summaryScrollViewer;

        // Initialize UI components after they are loaded
        private void InitializeCustomComponents()
        {
            _summaryTextBox = FindName("SummaryTextBox") as TextBox;
            _summaryScrollViewer = FindName("SummaryScrollViewer") as ScrollViewer;
        }

        public MainWindow()
        {
            InitializeComponent();
            ThemeButton.Click += ToggleTheme;
            SendButton.Click += (s, e) => SendMessage();
            CloseButton.Click += CloseButton_Click;

            // Initialize components after the window is loaded
            this.Loaded += (s, e) => InitializeCustomComponents();

            var exe = new ExcuteAPI.ExcuteAPI();

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
                    try
                    {
                        if (_summaryTextBox == null) return;
                        
                        messageCount++;
                        string newSummary = $"{messageCount}. {userMessage}\n";

                        // Append new summary to existing text
                        if (!string.IsNullOrEmpty(_summaryTextBox.Text))
                        {
                            _summaryTextBox.AppendText(newSummary);
                        }
                        else
                        {
                            _summaryTextBox.Text = newSummary;
                        }
                        
                        // Move cursor to the end and scroll to the end
                        _summaryTextBox.CaretIndex = _summaryTextBox.Text.Length;
                        _summaryTextBox.ScrollToEnd();
                        
                        // Ensure the scroll viewer updates
                        _summaryScrollViewer?.ScrollToEnd();
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"Error updating summary: {ex.Message}");
                    }
                }
            });
        }
        
private readonly object _sendMessageLock = new object();
private bool _isProcessing = false;

        private async void SendMessage(object? sender = null, RoutedEventArgs? e = null)
        {
            string message = InputBox.Text.Trim();
            if (!string.IsNullOrWhiteSpace(message))
            {
                lock (_sendMessageLock)
                {
                    if (_isProcessing)
                    {
                        MessageBox.Show("이전 요청을 처리 중입니다. 잠시만 기다려주세요.", "알림", 
                            MessageBoxButton.OK, MessageBoxImage.Information);
                        return;
                    }
                    _isProcessing = true;
                }
                
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

                // Clear input
                InputBox.Text = "";

                // In a real app, you would process the message and get a response here
                // For now, we'll simulate a simple response
                try
                {
                    // API 호출 및 응답 기다리기
                    string response = await ExcuteAPI.ExcuteAPI.SendMessageAsync(message);

                    // AI 응답 UI에 추가
                    Border aiMsgBorder = new Border
                    {
                        CornerRadius = new CornerRadius(8),
                        Background = new SolidColorBrush(Color.FromArgb(0xC0, 0xE9, 0xE9, 0xEB)),
                        Margin = new Thickness(0, 5, 0, 5),
                        Padding = new Thickness(10),
                        HorizontalAlignment = HorizontalAlignment.Left
                    };
                    TextBlock aiMsgText = new TextBlock
                    {
                        Text = response,
                        TextWrapping = TextWrapping.Wrap,
                        Foreground = Brushes.Black
                    };
                    aiMsgBorder.Child = aiMsgText;
                    MessageList.Children.Add(aiMsgBorder);

                    // 요약에 추가
                    UpdateSummary(response);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"메시지 전송 중 오류가 발생했습니다: {ex.Message}", "오류",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
                finally
                {
                    lock (_sendMessageLock)
                    {
                        _isProcessing = false;
                    }
                }
                // Add assistant response to chat
                // Dispatcher.Invoke(() =>
                // {
                //     Border responseBorder = new Border
                //     {
                //         CornerRadius = new CornerRadius(8),
                //         Background = new SolidColorBrush(Color.FromArgb(0xC0, 0xFF, 0xFF, 0xFF)),
                //         Margin = new Thickness(0, 5, 0, 5),
                //         Padding = new Thickness(10),
                //         HorizontalAlignment = HorizontalAlignment.Left
                //     };
                //     StackPanel responseStack = new StackPanel { Orientation = Orientation.Horizontal };
                //     Ellipse assistantIcon = new Ellipse { Width = 24, Height = 24, Fill = new SolidColorBrush(Colors.Blue), Margin = new Thickness(0, 0, 10, 0) };
                //     TextBlock responseText = new TextBlock { Text = response, VerticalAlignment = VerticalAlignment.Center };
                //     responseStack.Children.Add(assistantIcon);
                //     responseStack.Children.Add(responseText);
                //     responseBorder.Child = responseStack;
                //     MessageList.Children.Add(responseBorder);

                //     // No need to add assistant response to summary

                //     // Scroll to bottom
                //     MessageScrollViewer.ScrollToEnd();
                // });
            }
        }
    }
}
