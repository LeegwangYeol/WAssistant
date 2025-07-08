using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace ChatOrg.Controls
{
    public partial class ChatMessageListControl : UserControl
    {
        public ChatMessageListControl()
        {
            InitializeComponent();
        }

        public void AddUserMessage(string message)
        {
            var border = new Border
            {
                CornerRadius = new CornerRadius(8),
                Background = (Brush)FindResource("UserMessageBackground"),
                Margin = new Thickness(0, 5, 0, 5),
                Padding = new Thickness(10)
            };

            var stackPanel = new StackPanel 
            { 
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Right
            };

            var textBlock = new TextBlock
            {
                Text = message,
                TextWrapping = TextWrapping.Wrap,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(0, 0, 10, 0)
            };

            var ellipse = new Ellipse
            {
                Width = 24,
                Height = 24,
                Fill = Brushes.Green,
                Margin = new Thickness(10, 0, 0, 0)
            };

            stackPanel.Children.Add(textBlock);
            stackPanel.Children.Add(ellipse);
            border.Child = stackPanel;

            MessageList.Children.Add(border);
            ScrollToBottom();
        }

        public void AddAssistantMessage(string message)
        {
            var border = new Border
            {
                CornerRadius = new CornerRadius(8),
                Background = (Brush)FindResource("AssistantMessageBackground"),
                Margin = new Thickness(0, 5, 0, 5),
                Padding = new Thickness(10)
            };

            var stackPanel = new StackPanel 
            { 
                Orientation = Orientation.Horizontal 
            };

            var ellipse = new Ellipse
            {
                Width = 24,
                Height = 24,
                Fill = Brushes.Blue,
                Margin = new Thickness(0, 0, 10, 0)
            };

            var textBlock = new TextBlock
            {
                Text = message,
                TextWrapping = TextWrapping.Wrap,
                VerticalAlignment = VerticalAlignment.Center
            };

            stackPanel.Children.Add(ellipse);
            stackPanel.Children.Add(textBlock);
            border.Child = stackPanel;

            MessageList.Children.Add(border);
            ScrollToBottom();
        }

        private void ScrollToBottom()
        {
            if (MessageScrollViewer != null)
            {
                MessageScrollViewer.ScrollToBottom();
            }
        }
    }
}
