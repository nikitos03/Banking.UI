using System;
using System.Data;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace Banking.UI
{
    public partial class AuthorizationForm : Form
    {
        DataBase dataBase = new DataBase();
        public AuthorizationForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Этот метод позволяет пользователю при нажатии кнопки "Войти", 
        /// перейти на следующую форму за счет его логина и пароля;
        /// </summary>

        private void EnterButton_Click(object sender, EventArgs e)
        {
            var loginUser = Login.Text; 
            var passwordUser = Password.Text; 

            SqlDataAdapter adapter = new SqlDataAdapter();
            DataTable table = new DataTable();

            string str = $"select id_user, login_user, password_user, is_admin from register where login_user = '{loginUser}' and password_user = '{passwordUser}'";
           

            SqlCommand command = new SqlCommand(str, dataBase.getConnection()); 

            adapter.SelectCommand = command; 
            adapter.Fill(table); 

            if (table.Rows.Count == 1) 
            {
                var user = new checkUser(table.Rows[0].ItemArray[1].ToString(), Convert.ToBoolean(table.Rows[0].ItemArray[3]));

                MessageBox.Show("Вы успешно вошли!", "Успешно", MessageBoxButtons.OK, MessageBoxIcon.Information);
                MenuForm menuform = new MenuForm(user);
                this.Hide();
                menuform.ShowDialog();
            }
            else
                MessageBox.Show("Такого аккаунта не существует", "Аккаунта не существует", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        /// <summary>
        /// Этот метод позволяет пользователю выйти из программы;
        /// </summary>

        private void ExitButton_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        /// <summary>
        /// Этот метод позволяет пользователю зарегистрировать аккаунт;
        /// </summary>

        private void RegistrationButton_Click(object sender, EventArgs e)
        {
            var login = LoginReg.Text;
            var pass = PassReg.Text;

            string str = $"insert into register(login_user, password_user, is_admin) values ('{login}', '{pass}', 0)";

            SqlCommand command = new SqlCommand(str, dataBase.getConnection());

            dataBase.openConnection();

            if(command.ExecuteNonQuery() == 1)
            {
                MessageBox.Show("Аккаунт успешно создан!", "Успех");
                AuthorizationForm form = new AuthorizationForm();
                this.Hide();
                form.ShowDialog();
            }
            else
            {
                MessageBox.Show("Аккаунт не создан!");
            }
            dataBase.closeConnection();
        }


        /// <summary>
        /// Этот метод проверяет пользователя на логин и пароль
        /// </summary>

        private Boolean checkuser() // проверка пользователя
        {
            var login = LoginReg.Text;
            var pass = PassReg.Text;

            SqlDataAdapter adapter = new SqlDataAdapter();
            DataTable table = new DataTable();
            string str = $"select id_user, login_user, password_user, is_admin from register where login_user = '{login}' and password_user '{pass}'";

            SqlCommand command = new SqlCommand(str, dataBase.getConnection());

            adapter.SelectCommand = command;
            adapter.Fill(table);

            if(table.Rows.Count > 0)
            {
                MessageBox.Show("Пользователь уже существует");
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
