using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ChatOrg.Models
{
    public class ChatMessage : INotifyPropertyChanged
    {
        private string _message;
        private bool _isUserMessage;
        private DateTime _timestamp;

        public string Message
        {
            get => _message;
            set => SetProperty(ref _message, value);
        }

        public bool IsUserMessage
        {
            get => _isUserMessage;
            set => SetProperty(ref _isUserMessage, value);
        }

        public DateTime Timestamp
        {
            get => _timestamp;
            set => SetProperty(ref _timestamp, value);
        }

        public ChatMessage()
        {
            Timestamp = DateTime.Now;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }
    }
}
