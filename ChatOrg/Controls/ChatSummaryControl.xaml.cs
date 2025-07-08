using System.Windows;
using System.Windows.Controls;

namespace ChatOrg.Controls
{
    public partial class ChatSummaryControl : UserControl
    {
        public ChatSummaryControl()
        {
            InitializeComponent();
        }

        public void UpdateSummary(string summary)
        {
            if (SummaryTextBox != null)
            {
                if (!string.IsNullOrEmpty(SummaryTextBox.Text))
                {
                    SummaryTextBox.Text += "\n\n";
                }
                SummaryTextBox.Text += summary;
                
                // 스크롤을 가장 아래로 이동
                SummaryScrollViewer.ScrollToEnd();
            }
        }
    }
}
