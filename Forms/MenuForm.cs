using System;
using System.Data;
using System.Linq;
using System.Net;
using System.Windows.Forms;
using System.Xml.Linq;
using System.IO;
using System.Data.SqlClient;

namespace Banking.UI
{
    public partial class MenuForm : Form
    {
        private readonly checkUser _user;
        DataBase dataBase = new DataBase();

        public MenuForm(checkUser user)
        {
            InitializeComponent();

            _user = user;

            WebClient client = new WebClient();
            var xml = client.DownloadString("https://www.cbr-xml-daily.ru/daily.xml");
            XDocument xdoc = XDocument.Parse(xml);
            var el = xdoc.Element("ValCurs").Elements("Valute");
            string dollar = el.Where(x => x.Attribute("ID").Value == "R01235").Select(x => x.Element("Value").Value).FirstOrDefault();
            string eur = el.Where(x => x.Attribute("ID").Value == "R01239").Select(x => x.Element("Value").Value).FirstOrDefault();
            string cny = el.Where(x => x.Attribute("ID").Value == "R01375").Select(x => x.Element("Value").Value).FirstOrDefault();
            string datcron = el.Where(x => x.Attribute("ID").Value == "R01215").Select(x => x.Element("Value").Value).FirstOrDefault();
            string turlin = el.Where(x => x.Attribute("ID").Value == "R01700J").Select(x => x.Element("Value").Value).FirstOrDefault();
            string cheshkron = el.Where(x => x.Attribute("ID").Value == "R01760").Select(x => x.Element("Value").Value).FirstOrDefault();
            string japanIen = el.Where(x => x.Attribute("ID").Value == "R01820").Select(x => x.Element("Value").Value).FirstOrDefault();
            string rumlen = el.Where(x => x.Attribute("ID").Value == "R01585F").Select(x => x.Element("Value").Value).FirstOrDefault();

            DollarText.Text = (dollar);
            EuroText.Text = (eur);
            CnyText.Text = (cny);
            cron.Text = (datcron);
            lir.Text = (turlin);
            checs.Text = (cheshkron);
            ien.Text = (japanIen);
            lea.Text = (rumlen);
        }

        /// <summary>
        /// Этот метод делает кнопки активными только для администратора;
        /// </summary>

        private void IsAdmin()
        {
            UserStatus.Enabled = _user.IsAdmin;
            AdminPanel.Enabled = _user.IsAdmin;
        }

        private void MenuForm_Load(object sender, EventArgs e)
        {
            UserStatus.Text = $"{_user.Login}: {_user.Status}";
            IsAdmin();
        }

        /// <summary>
        /// Этот метод позволяет выйти из программы;
        /// </summary>

        private void Exit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        /// <summary>
        /// Этот метод на данный момент не запрограммирован в коде;
        /// </summary>

        private void Memorandum_Click(object sender, EventArgs e)
        {
            
        }

        /// <summary>
        /// Этот метод позволяет администратору перейти на его форму;
        /// </summary>

        private void AdminPanel_Click(object sender, EventArgs e)
        {
            AdminPanelForm menuform = new AdminPanelForm(_user);
            this.Hide();
            menuform.ShowDialog();
            this.Show();
        }

        /// <summary>
        /// Этот метод позволяет пользователю оставить запись в txt файле;
        /// </summary>

        private void Record_Click(object sender, EventArgs e)
        {
            string path = Titles.Text + ".txt";
            if (!File.Exists(path))
            {
                StreamWriter file = new StreamWriter(path);
                file.WriteLine(Note.Text);
                file.Close();
                MessageBox.Show("Запись успешно создана");
            }
            else
            {
                MessageBox.Show("Запись не создана, напишите заглавие");
            }
        }

        /// <summary>
        /// Этот метод позволяет пользователю открывать txt файл;
        /// </summary>

        private void Open_Click(object sender, EventArgs e)
        {
            OpenFileDialog file = new OpenFileDialog();
            file.Filter = "txt files (*.txt)|*.txt|All Files (*.*)|*.*";
            if (file.ShowDialog() == DialogResult.OK)
            {
                Titles.Text = file.FileName;
                Note.Text = File.ReadAllText(file.FileName);
            }
        }

        /// <summary>
        /// Этот метод позволяет администратору сохранять курс валюты в базе данных;
        /// </summary>
        /// 
        private void Save_Click(object sender, EventArgs e)
        {
            string kursvaluta = "insert into KursValuta (Dollar, Euro, Chinese_yuan, Danish_kroner, Turkish_lira, Czech_crowns, Japanese_yen, Romanian_flax)" +
            $" values " +
            $"(@Dollar, @Euro, @Chinese_yuan, @Danish_kroner, @Turkish_lira, @Czech_crowns, @Japanese_yen, @Romanian_flax)";
            SqlCommand command = new SqlCommand(kursvaluta, dataBase.getConnection());
            dataBase.openConnection();
            command.Parameters.AddWithValue("Dollar", DollarText.Text);
            command.Parameters.AddWithValue("Euro", EuroText.Text);
            command.Parameters.AddWithValue("Chinese_yuan", CnyText.Text);
            command.Parameters.AddWithValue("Danish_kroner", cron.Text);
            command.Parameters.AddWithValue("Turkish_lira", lir.Text);
            command.Parameters.AddWithValue("Czech_crowns", checs.Text);
            command.Parameters.AddWithValue("Japanese_yen", ien.Text);
            command.Parameters.AddWithValue("Romanian_flax", lea.Text);
            command.ExecuteNonQuery();
            dataBase.closeConnection();
        }
    }
}
