using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Threading.Tasks;
using ChatOrg.Chat;

namespace ChatOrg.Controls
{
    public partial class ChatInputControl : UserControl
    {
        public ChatMessageListControl? MessageListControl { get; set; }

        public ChatInputControl()
        {
            InitializeComponent();
            SendButton.Click += SendButton_Click;
        }

        private async void SendButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(InputBox.Text)) return;
            
            try
            {
                SendButton.IsEnabled = false;
                string userInput = InputBox.Text;
                
                // 사용자 메시지를 메시지 리스트에 추가
                if (MessageListControl != null)
                {
                    MessageListControl.AddUserMessage(userInput);
                }
                
                InputBox.Clear();
                
                // API 호출
                string result = await ExcuteAPI.SendMessageAsync(userInput);
                
                // AI 응답을 메시지 리스트에 추가
                if (MessageListControl != null)
                {
                    MessageListControl.AddAssistantMessage(result);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"오류가 발생했습니다: {ex.Message}", "오류", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                SendButton.IsEnabled = true;
            }
        }

        private void InputBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && !Keyboard.IsKeyDown(Key.LeftShift) && !Keyboard.IsKeyDown(Key.RightShift))
            {
                SendButton_Click(sender, e);
                e.Handled = true;
            }
        }
    }
}
