using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using ChatOrg.Chat;
using ChatOrg.Controls;

namespace ChatOrg
{
    public partial class MainWindow : Window
    {
        private bool isDarkTheme = false;
        private int messageCount = 0;
        private readonly object summaryLock = new object();
        private readonly ExcuteAPI _api;
        private TextBox? _summaryTextBox;
        private ScrollViewer? _summaryScrollViewer;
        private StackPanel? _messageList;
        private TextBox? _inputBox;
        private Button? _sendButton;
        private Button? _closeButton;
        private Button? _themeButton;

        public MainWindow()
        {
            InitializeComponent();
            _api = new ExcuteAPI();
            
            // 컨트롤 연결
            if (InputControl != null && MessageListControl != null)
            {
                InputControl.MessageListControl = MessageListControl;
            }
            
            // 컨트롤 초기화
            InitializeControls();
            
            // 이벤트 핸들러 연결
            if (_sendButton != null)
            {
                _sendButton.Click += (s, e) => SendMessage();
            }
            
            if (_closeButton != null)
            {
                _closeButton.Click += CloseButton_Click;
            }
            
            if (_themeButton != null)
            {
                _themeButton.Click += ToggleTheme;
            }
            
            if (_inputBox != null)
            {
                _inputBox.KeyDown += (s, e) =>
                {
                    if (e.Key == Key.Enter)
                    {
                        SendMessage();
                        e.Handled = true;
                    }
                };
            }
        }
        
        private void InitializeControls()
        {
            // 컨트롤 초기화
            if (HeaderControl != null)
            {
                _closeButton = HeaderControl.CloseButton;
                _themeButton = HeaderControl.ThemeButton;
            }
            
            if (InputControl != null)
            {
                _inputBox = InputControl.InputBox;
                _sendButton = InputControl.SendButton;
            }
            
            if (MessageListControl != null)
            {
                _messageList = MessageListControl.MessageList;
            }
            
            if (SummaryControl != null)
            {
                _summaryTextBox = SummaryControl.SummaryTextBox;
                _summaryScrollViewer = SummaryControl.SummaryScrollViewer;
            }
        }

        private void SendMessage()
        {
            if (_inputBox == null || string.IsNullOrWhiteSpace(_inputBox.Text)) return;
            
            string message = _inputBox.Text.Trim();
            _inputBox.Clear();
            
            // 메시지 전송 로직
            // TODO: 실제 메시지 전송 로직 구현
            
            // 대화 요약 업데이트
            UpdateSummary(message);
        }

        private void UpdateSummary(string userMessage)
        {
            if (_summaryTextBox == null) return;
            
            Dispatcher.Invoke(() =>
            {
                lock (summaryLock)
                {
                    try
                    {
                        messageCount++;
                        string newSummary = $"{messageCount}. {userMessage}\n";

                        // 기존 텍스트에 새 요약 추가
                        if (!string.IsNullOrEmpty(_summaryTextBox.Text))
                        {
                            _summaryTextBox.AppendText(newSummary);
                        }
                        else
                        {
                            _summaryTextBox.Text = newSummary;
                        }
                        
                        // 커서를 끝으로 이동하고 스크롤
                        _summaryTextBox.CaretIndex = _summaryTextBox.Text.Length;
                        _summaryTextBox.ScrollToEnd();
                        
                        // 스크롤 뷰어 업데이트
                        _summaryScrollViewer?.ScrollToEnd();
                    }
                    catch (Exception ex)
                    {
                        // 오류 처리
                        Console.WriteLine($"요약 업데이트 중 오류 발생: {ex.Message}");
                    }
                }
            });
        }

        private void ToggleTheme(object sender, RoutedEventArgs e)
        {
            isDarkTheme = !isDarkTheme;
            // TODO: 테마 전환 로직 구현
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
