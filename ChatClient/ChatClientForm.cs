using System;
using System.Drawing;
using System.Windows.Forms;

namespace ChatClient
{
    public partial class ChatClientForm : Form
    {   
        /// Объект, реализующий взаимодействие клиента с сервером.
        public ChatSession CurrentSession;

        /// Метод для обновления View логин-формы через клиентскую модель.
        public async void UpdateView(string info, Color color)
        {
            InfoLabel.ForeColor = color;
            InfoLabel.Text = info;
            InfoLabel.Update();
        }

        /// Метод, создающий форму для чат-сессии
        public ChatSessionForm SwitchToChat()
        {
            Visible = false;

            var sessionForm = new ChatSessionForm(this);
            sessionForm.Show();

            return sessionForm;
        }

        /// Метод, открывающий новое соединение с сервером
        public void NewChatSession()
        {
            // NameBox.Clear();
            // LoginBox.Clear();
            // PasswordBox.Clear();
            InfoLabel.Text = "";
            CurrentSession = new ChatSession(this);
        }

        /// Запрос серверу на авторизацию
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
        public ChatClientForm()
        {
            StartPosition = FormStartPosition.CenterScreen;
            InitializeComponent();

            // Инициализируем объект с логикой и данными для чата
            NewChatSession();
        }

        private void LoginButton_Click(object sender, EventArgs e)
        {
            SignIn();            
        }


        private void ChatClientForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            CurrentSession.End();
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
    }
}
