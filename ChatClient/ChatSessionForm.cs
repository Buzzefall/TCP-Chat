using System;
using System.Text;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using DataPackaging;

namespace ChatClient
{
    public partial class ChatSessionForm : Form
    {
        // Ссылка на структуру данных клиента
        public ChatSession CurrentSession { get; set; }

        // Уменьшаем кол-во вызовов конструкторов строк
        private readonly StringBuilder chatBuilder = new StringBuilder();

        // Код для обновления формы чата (View) через клиентскую модель
        public void UpdateChat(TextMessage message)
        {
            chatBuilder.Append($"[{message.TimeStamp}] {message.From}: {message.Text}");
            chatBuilder.Append(Environment.NewLine);
            ChatTextBox.Text = chatBuilder.ToString();
            ChatTextBox.Update();
        }

        public void UpdateOnlineList(OnlineList list)
        {
            var builder = new StringBuilder();

            for (int i = 0; i < list.Count; i++)
            {
                builder.Append(list[i]);
                builder.Append(Environment.NewLine);
            }

            OnlineTextBox.Text = builder.ToString();
            OnlineTextBox.Update();

            OnlineLabel.Text = $"Users online: {list.Count}";
            OnlineLabel.Update();
        }

        public ChatSessionForm(ChatClientForm owner)
        {
            InitializeComponent();

            Owner = owner;
            CurrentSession = owner.CurrentSession;
            CurrentSession.sessionForm = this;

            MessageBox.ForeColor = Color.DarkGray;
            MessageBox.Text = "Type something here...";
            MessageBox.Update();
        }

        //public ChatSessionForm()
        //{
        //    InitializeComponent();

        //    //CurrentSession = (Owner as ChatClientForm).CurrentSession;
        //    //CurrentSession.sessionForm = this;
        //}


        // Далее - обработчики событий, работающие с клиентской моделью ChatSession (Control)
        private void MessageBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != (char) Keys.Enter) return;
            
            e.Handled = true;

            if (string.IsNullOrWhiteSpace(MessageBox.Text) || MessageBox.Text == "\0")
            {
                return;
            }

            var message = new TextMessage()
            {
                Text = MessageBox.Text
            };

            CurrentSession?.SendMessage(message);

            MessageBox.Clear();
            MessageBox.Update();
        }

        private void MessageBox_Enter(object sender, EventArgs e)
        {
            if (MessageBox.Text == "Type something here...")
            {
                MessageBox.ForeColor = Color.Black;
                MessageBox.Clear();
                MessageBox.Update();
            }
            else
            {
                MessageBox.Select(MessageBox.Text.Length, 0);
            }
        }

        private void MessageBox_Leave(object sender, EventArgs e)
        {
            if (MessageBox.Text == null || MessageBox.Text == "\0")
            {
                return;
            }

            MessageBox.ForeColor = Color.DarkGray;
            MessageBox.Text = "Type something here...";
            MessageBox.Update();
        }

        private void ChatSessionForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Owner.Visible = true;
            CurrentSession?.End();
            (Owner as ChatClientForm)?.NewChatSession();
        }

        private void ChatTextBox_TextChanged(object sender, EventArgs e)
        {
            Utils.SendMessage(ChatTextBox.Handle, Utils.WM_VSCROLL, (IntPtr) Utils.SB_BOTTOM, IntPtr.Zero);
        }

    }

    public class Utils
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int SendMessage(IntPtr hWnd, int wMsg, IntPtr wParam, IntPtr lParam);

        public const int WM_VSCROLL = 0x115;
        public const int SB_BOTTOM = 7;

        /// <summary>
        /// Scrolls the vertical scroll bar of a multi-line text box to the bottom.
        /// </summary>
        /// <param name="tb">The text box to scroll</param>
        public static void ScrollToBottom(TextBox tb)
        {
            SendMessage(tb.Handle, WM_VSCROLL, (IntPtr)SB_BOTTOM, IntPtr.Zero);
        }
    }
}
