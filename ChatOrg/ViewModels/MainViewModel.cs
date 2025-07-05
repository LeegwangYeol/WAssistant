using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using ChatOrg.Commands;
using ChatOrg.Models;
using ChatOrg.Services;

namespace ChatOrg.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private readonly IChatService _chatService;
        private string _inputMessage;
        private string _summary;
        private bool _isDarkTheme;
        private bool _isProcessing;
        private int _messageCount;

        public ObservableCollection<ChatMessage> Messages { get; } = new ObservableCollection<ChatMessage>();
        public string InputMessage
        {
            get => _inputMessage;
            set => SetProperty(ref _inputMessage, value);
        }

        public string Summary
        {
            get => _summary;
            set => SetProperty(ref _summary, value);
        }

        public bool IsDarkTheme
        {
            get => _isDarkTheme;
            set => SetProperty(ref _isDarkTheme, value);
        }

        public ICommand SendMessageCommand { get; private set; }
        public ICommand ToggleThemeCommand { get; private set; }
        public ICommand CloseApplicationCommand { get; private set; }

        public MainViewModel(IChatService chatService)
        {
            _chatService = chatService ?? throw new ArgumentNullException(nameof(chatService));
            InitializeCommands();
        }

        private void InitializeCommands()
        {
            SendMessageCommand = new RelayCommand(async _ => await SendMessageAsync(), _ => !string.IsNullOrWhiteSpace(InputMessage) && !_isProcessing);
            ToggleThemeCommand = new RelayCommand(_ => ToggleTheme());
            CloseApplicationCommand = new RelayCommand(_ => System.Windows.Application.Current.Shutdown());
        }

        private async Task SendMessageAsync()
        {
            if (_isProcessing) return;
            
            var userMessage = new ChatMessage
            {
                Message = InputMessage,
                IsUserMessage = true
            };
            
            Messages.Add(userMessage);
            UpdateSummary($"{++_messageCount}. {InputMessage}\n");
            
            var userInput = InputMessage;
            InputMessage = string.Empty;
            
            try
            {
                _isProcessing = true;
                CommandManager.InvalidateRequerySuggested();
                
                // AI 응답 요청
                var aiResponse = await _chatService.SendMessageAsync(userInput);
                
                var aiMessage = new ChatMessage
                {
                    Message = aiResponse,
                    IsUserMessage = false
                };
                
                Messages.Add(aiMessage);
                UpdateSummary($"{++_messageCount}. {aiResponse}\n");
            }
            catch (Exception ex)
            {
                var errorMessage = new ChatMessage
                {
                    Message = $"오류가 발생했습니다: {ex.Message}",
                    IsUserMessage = false
                };
                Messages.Add(errorMessage);
            }
            finally
            {
                _isProcessing = false;
                CommandManager.InvalidateRequerySuggested();
            }
        }

        private void ToggleTheme()
        {
            IsDarkTheme = !IsDarkTheme;
            // 여기에 테마 변경 로직 추가 예정
        }

        private void UpdateSummary(string message)
        {
            if (string.IsNullOrEmpty(Summary))
                Summary = message;
            else
                Summary += message;
        }
    }
}
