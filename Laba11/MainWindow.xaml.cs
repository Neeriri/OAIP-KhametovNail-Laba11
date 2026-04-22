using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Microsoft.EntityFrameworkCore;
using Laba11.Data;
using Laba11.Models;

namespace Laba11
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private AppDbContext _context;
        private Student _selectedStudent;

        public MainWindow()
        {
            InitializeComponent();
            _context = new AppDbContext();
            LoadData();
            LoadCompanies();
        }

        private void LoadData()
        {
            try
            {
                var students = _context.Students
                    .Include(s => s.Company)
                    .ToList();
                StudentsGrid.ItemsSource = students;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadCompanies()
        {
            try
            {
                var companies = _context.Companies.ToList();
                CompanyCombo.ItemsSource = companies;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки компаний: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void RoleRadio_Click(object sender, RoutedEventArgs e)
        {
            bool isAdmin = AdminRoleRadio.IsChecked == true;
            
            // Для администратора доступны все операции
            // Для студента только просмотр
            FirstNameBox.IsEnabled = isAdmin;
            LastNameBox.IsEnabled = isAdmin;
            DateOfBirthPicker.IsEnabled = isAdmin;
            EmailBox.IsEnabled = isAdmin;
            PasswordBox.IsEnabled = isAdmin;
            UniversityBox.IsEnabled = isAdmin;
            SpecialtyBox.IsEnabled = isAdmin;
            CompanyCombo.IsEnabled = isAdmin;
            AddBtn.IsEnabled = isAdmin;
            UpdateBtn.IsEnabled = isAdmin;
            DeleteBtn.IsEnabled = isAdmin;
        }

        private void AddBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!ValidateInput()) return;

                var student = new Student
                {
                    FirstName = FirstNameBox.Text,
                    LastName = LastNameBox.Text,
                    DateOfBirth = DateOfBirthPicker.SelectedDate ?? DateTime.Now,
                    Email = EmailBox.Text,
                    Password = PasswordBox.Text,
                    University = UniversityBox.Text,
                    Specialty = SpecialtyBox.Text,
                    CompanyId = (CompanyCombo.SelectedItem as Company)?.Id
                };

                _context.Students.Add(student);
                _context.SaveChanges();

                ClearForm();
                LoadData();
                MessageBox.Show("Студент успешно добавлен!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка добавления: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void UpdateBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_selectedStudent == null)
                {
                    MessageBox.Show("Выберите студента для обновления!", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (!ValidateInput()) return;

                _selectedStudent.FirstName = FirstNameBox.Text;
                _selectedStudent.LastName = LastNameBox.Text;
                _selectedStudent.DateOfBirth = DateOfBirthPicker.SelectedDate ?? DateTime.Now;
                _selectedStudent.Email = EmailBox.Text;
                _selectedStudent.Password = PasswordBox.Text;
                _selectedStudent.University = UniversityBox.Text;
                _selectedStudent.Specialty = SpecialtyBox.Text;
                _selectedStudent.CompanyId = (CompanyCombo.SelectedItem as Company)?.Id;

                _context.SaveChanges();

                ClearForm();
                LoadData();
                MessageBox.Show("Данные студента успешно обновлены!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка обновления: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DeleteBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_selectedStudent == null)
                {
                    MessageBox.Show("Выберите студента для удаления!", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var result = MessageBox.Show($"Вы уверены, что хотите удалить студента {_selectedStudent.FirstName} {_selectedStudent.LastName}?", 
                    "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    _context.Students.Remove(_selectedStudent);
                    _context.SaveChanges();

                    ClearForm();
                    LoadData();
                    MessageBox.Show("Студент успешно удален!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка удаления: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void StudentsGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (StudentsGrid.SelectedItem is Student student)
            {
                _selectedStudent = student;
                FirstNameBox.Text = student.FirstName;
                LastNameBox.Text = student.LastName;
                DateOfBirthPicker.SelectedDate = student.DateOfBirth;
                EmailBox.Text = student.Email;
                PasswordBox.Text = student.Password;
                UniversityBox.Text = student.University;
                SpecialtyBox.Text = student.Specialty;
                
                if (student.Company != null)
                {
                    CompanyCombo.SelectedItem = CompanyCombo.Items.Cast<Company>()
                        .FirstOrDefault(c => c.Id == student.CompanyId);
                }
            }
        }

        private void RefreshBtn_Click(object sender, RoutedEventArgs e)
        {
            LoadData();
            ClearForm();
        }

        private bool ValidateInput()
        {
            if (string.IsNullOrWhiteSpace(FirstNameBox.Text))
            {
                MessageBox.Show("Введите имя!", "Ошибка валидации", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            if (string.IsNullOrWhiteSpace(LastNameBox.Text))
            {
                MessageBox.Show("Введите фамилию!", "Ошибка валидации", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            if (string.IsNullOrWhiteSpace(EmailBox.Text))
            {
                MessageBox.Show("Введите email!", "Ошибка валидации", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            if (string.IsNullOrWhiteSpace(PasswordBox.Text))
            {
                MessageBox.Show("Введите пароль!", "Ошибка валидации", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            return true;
        }

        private void ClearForm()
        {
            FirstNameBox.Clear();
            LastNameBox.Clear();
            DateOfBirthPicker.SelectedDate = null;
            EmailBox.Clear();
            PasswordBox.Clear();
            UniversityBox.Clear();
            SpecialtyBox.Clear();
            CompanyCombo.SelectedItem = null;
            _selectedStudent = null;
        }
    }
}
