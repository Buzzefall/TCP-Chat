using System;
using System.Drawing;
using System.Windows.Forms;

namespace ChatClient
{
    public partial class ChatClientForm : Form
    {   
        /// Объект, реализующий взаимодействие клиента с сервером.
        public ChatSession CurrentSession;

        //public delegate void NewChatSessionCallback();
        //public delegate void UpdateViewCallback(string info, Color color);
        //public delegate ChatSessionForm SwitchToChatCallback();

        //public event NewChatSessionCallback NewChatSessionEvent;
        //public event UpdateViewCallback UpdateViewEvent;
        //public event SwitchToChatCallback FormSwitchToChatEvent;

        //private readonly FormUpdateView UpdateViewEvent;
        //private readonly FormSwitchToChat SwitchToChatEvent;
        //private readonly FormNewChatSession NewChatSessionEvent;

        public ChatClientForm()
        {
            StartPosition = FormStartPosition.CenterScreen;
            InitializeComponent();

            //NewChatSessionEvent += NewChatSession;
            //UpdateViewEvent += UpdateView;
            //FormSwitchToChatEvent += SwitchToChat;
        }


        /// Method that updates View of login form
        public void UpdateView(string info, Color color)
        {
            InfoLabel.ForeColor = color;
            InfoLabel.Text = info;
            InfoLabel.Update();
        }

        /// Method that creates chat Form
        public ChatSessionForm SwitchToChat()
        {
            Visible = false;

            var sessionForm = new ChatSessionForm(this);
            sessionForm.Show();

            return sessionForm;
        }

        /// Create new ChatSession service
        public void NewChatSession()
        {
            // NameBox.Clear();
            // LoginBox.Clear();
            // PasswordBox.Clear();
            InfoLabel.Text = "";
            CurrentSession = new ChatSession(this);
        }

        /// Authorization request
        private void SignIn()
        {
            if (string.IsNullOrWhiteSpace(PasswordBox.Text) ||
                PasswordBox.Text == "" || PasswordBox.Text == "\0" ||
                string.IsNullOrWhiteSpace(LoginBox.Text) ||
                LoginBox.Text == "" || LoginBox.Text == "\0")
            {
                return;
            }

            CurrentSession.SignIn(NameBox.Text, LoginBox.Text, PasswordBox.Text);
            PasswordBox.Clear();
        }

        // Обработчики событий, управляющих моделью клиента (Control)

        private void LoginButton_Click(object sender, EventArgs e)
        {
            SignIn();            
        }


        private void ChatClientForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            CurrentSession.Dispose();
        }

        private void PasswordBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != (char) Keys.Enter) return;
            
            e.Handled = true;

            SignIn();
        }

        private void LoginBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != (char) Keys.Enter) return;
            
            e.Handled = true;

            SignIn();
        }

        private void NameBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != (char) Keys.Enter) return;
            
            e.Handled = true;

            SignIn();
        }

        private void ChatClientForm_Load(object sender, EventArgs e)
        {
            // Initializing ChatSession service.
            // WARNING: Do not do this in a constructor since control's construction supposed async and can lead to InvalidOperationException
            // immediately after Form.Invoke method call in NewChatSession. Here, controls guaranteed to be constructed already.
            NewChatSession();
        }
    }
}
