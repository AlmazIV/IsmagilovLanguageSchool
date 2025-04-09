using Microsoft.Win32;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace Исмагилов_ЯШ
{
    public partial class AddEditPage : Page
    {
        private Client currentPeople = new Client();

        public AddEditPage(Client selectedPeople)
        {
            InitializeComponent();

            if (selectedPeople != null)
            {
                currentPeople = selectedPeople;
            }

            DataContext = currentPeople;

            if (currentPeople.GenderCode == "м")
                MaleRBtn.IsChecked = true;
            else if (currentPeople.GenderCode == "ж")
                FemaleRBtn.IsChecked = true;

            if (currentPeople.ID == 0)
            {
                IDTBox.Visibility = Visibility.Hidden;
                IDTBox.Visibility = Visibility.Hidden;
                currentPeople.Birthday = DateTime.Now;
                currentPeople.RegistrationDate = DateTime.Now; // Если есть такое поле
            }
        }

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder errors = new StringBuilder();

            // Регулярное выражение для проверки ФИО (буквы, пробел и дефис)
            string fioPattern = @"^[a-zA-Zа-яА-ЯёЁ\s-]+$";

            // Проверки для фамилии
            if (string.IsNullOrWhiteSpace(currentPeople.LastName))
                errors.AppendLine("Укажите фамилию клиента");
            else
            {
                if (currentPeople.LastName.Length > 50)
                    errors.AppendLine("Превышен лимит символов фамилии");
                else if (!Regex.IsMatch(currentPeople.LastName, fioPattern))
                    errors.AppendLine("Фамилия может содержать только буквы, пробел и дефис");
            }

            // Проверки для имени
            if (string.IsNullOrWhiteSpace(currentPeople.FirstName))
                errors.AppendLine("Укажите имя клиента");
            else
            {
                if (currentPeople.FirstName.Length > 50)
                    errors.AppendLine("Превышен лимит символов имени");
                else if (!Regex.IsMatch(currentPeople.FirstName, fioPattern))
                    errors.AppendLine("Имя может содержать только буквы, пробел и дефис");
            }

            // Проверки для отчества
            if (string.IsNullOrWhiteSpace(currentPeople.Patronymic))
                errors.AppendLine("Укажите отчество клиента");
            else
            {
                if (currentPeople.Patronymic.Length > 50)
                    errors.AppendLine("Превышен лимит символов отчества");
                else if (!Regex.IsMatch(currentPeople.Patronymic, fioPattern))
                    errors.AppendLine("Отчество может содержать только буквы, пробел и дефис");
            }

            if (string.IsNullOrWhiteSpace(BirthdayDPicker.Text))
                errors.AppendLine("Укажите дату рождения клиента");

            if (string.IsNullOrWhiteSpace(currentPeople.Email))
            {
                errors.AppendLine("Укажите email клиента");
            }
            else if (!Regex.IsMatch(currentPeople.Email, @"^[a-zA-Z]+@[a-zA-Z]+\.[a-zA-Z]{2,}$"))
            {
                errors.AppendLine("Укажите корректный email клиента");
            }

            // Проверка для поля телефона:
            // Допустимы только цифры, символы '+', '-', '(', ')' и пробел.
            if (string.IsNullOrWhiteSpace(currentPeople.Phone))
            {
                errors.AppendLine("Укажите номер клиента");
            }
            else if (!Regex.IsMatch(currentPeople.Phone.Trim(), @"^[0-9+\-\(\) ]+$"))
            {
                errors.AppendLine("Номер телефона может содержать только цифры, символы '+', '-', '(', ')' и пробел.");
            }

            // Обновление даты рождения
            currentPeople.Birthday = Convert.ToDateTime(BirthdayDPicker.Text);

            // Установка пола
            currentPeople.GenderCode = FemaleRBtn.IsChecked == true ? "ж" : "м";

            if (errors.Length > 0)
            {
                MessageBox.Show(errors.ToString());
                return;
            }

            // Добавление нового клиента
            if (currentPeople.ID == 0)
            {
                ИсмагиловЯШEntities.GetContext().Client.Add(currentPeople);
            }

            try
            {
                ИсмагиловЯШEntities.GetContext().SaveChanges();
                MessageBox.Show("Информация сохранена");
                Manager.MainFrame.GoBack();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }

        private string GetProjectRootDirectory()
 {
     // Путь к исполняемому файлу (bin/Debug)
     string exePath = System.Reflection.Assembly.GetExecutingAssembly().Location;
     // Поднимаемся на 3 уровня: bin/Debug → bin → Корень проекта
     return System.IO.Path.GetDirectoryName(System.IO.Path.GetDirectoryName(System.IO.Path.GetDirectoryName(exePath)));
 }
        private void ChangePhoto_Click(object sender, RoutedEventArgs e)
        {
            string projectDirectory = GetProjectRootDirectory();
            string clientsFolderPath = System.IO.Path.Combine(projectDirectory, "Клиенты");

            // Создаем папку, если её нет
            if (!Directory.Exists(clientsFolderPath))
            {
                Directory.CreateDirectory(clientsFolderPath);
            }

            OpenFileDialog myOpenFileDialog = new OpenFileDialog
            {
                InitialDirectory = clientsFolderPath
            };

            if (myOpenFileDialog.ShowDialog() == true)
            {
                string selectedFilePath = myOpenFileDialog.FileName;

                // Сохраняем относительный путь ОТНОСИТЕЛЬНО КОРНЯ ПРОЕКТА
                currentPeople.PhotoPath = System.IO.Path.Combine("Клиенты", System.IO.Path.GetFileName(selectedFilePath));

                // Загружаем изображение по полному пути
                PhotoPeople.Source = new BitmapImage(new Uri(selectedFilePath));
            }
        }



    }
}
