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
        private void RefreshReportsBtn_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedStudent != null)
            {
                LoadReports(_selectedStudent.Id);
                ClearReportForm();
            }
            else
            {
                ReportsGrid.ItemsSource = null;
            }
        }
        private void LoadData()
        {
            try
            {
                var students = _context.Students
                    .Include(s => s.Company)
                    .Include(s => s.Reports) 
                    .ToList();
                StudentsGrid.ItemsSource = students;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void LoadReports(int studentId)
        {
            try
            {
                var reports = _context.Reports
                    .Where(r => r.StudentId == studentId)
                    .Include(r => r.Company)
                    .ToList();
                ReportsGrid.ItemsSource = reports;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки отчётов: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void LoadCompanies()
        {
            try
            {
                var companies = _context.Companies.ToList();
                CompanyCombo.ItemsSource = companies;
                ReportCompanyCombo.ItemsSource = companies; 
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки компаний: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void RoleRadio_Click(object sender, RoutedEventArgs e)
        {
            bool isAdmin = AdminRoleRadio.IsChecked == true;

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
            catch (DbUpdateException dbEx)
            {
                string errorMessage = "Ошибка базы данных при добавлении студента.\n\n";
                
                if (dbEx.InnerException != null)
                {
                    errorMessage += $"Детали ошибки: {dbEx.InnerException.Message}\n\n";
                    
                    if (dbEx.InnerException.InnerException != null)
                    {
                        errorMessage += $"Дополнительная информация: {dbEx.InnerException.InnerException.Message}";
                    }
                }
                
                errorMessage += "\nВозможные причины:\n" +
                               "- Email уже существует в базе данных (нарушение уникальности)\n" +
                               "- Нарушение целостности данных (компания была удалена)\n" +
                               "- Проблемы с подключением к базе данных\n" +
                               "- Превышение максимальной длины строковых полей (Имя/Фамилия до 50 символов, Email/Пароль до 100, Университет/Специальность до 100)";
                
                MessageBox.Show(errorMessage, "Ошибка базы данных", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                string errorMessage = $"Ошибка добавления: {ex.Message}\n\n";
                
                if (ex.InnerException != null)
                {
                    errorMessage += $"Детали: {ex.InnerException.Message}";
                }
                
                MessageBox.Show(errorMessage, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
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
            catch (DbUpdateException dbEx)
            {
                string errorMessage = "Ошибка базы данных при обновлении студента.\n\n";
                
                if (dbEx.InnerException != null)
                {
                    errorMessage += $"Детали ошибки: {dbEx.InnerException.Message}\n\n";
                    
                    if (dbEx.InnerException.InnerException != null)
                    {
                        errorMessage += $"Дополнительная информация: {dbEx.InnerException.InnerException.Message}";
                    }
                }
                
                errorMessage += "\nВозможные причины:\n" +
                               "- Email уже используется другим студентом (нарушение уникальности)\n" +
                               "- Нарушение целостности данных (компания была удалена)\n" +
                               "- Проблемы с подключением к базе данных\n" +
                               "- Превышение максимальной длины строковых полей (Имя/Фамилия до 50 символов, Email/Пароль до 100, Университет/Специальность до 100)";
                
                MessageBox.Show(errorMessage, "Ошибка базы данных", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                string errorMessage = $"Ошибка обновления: {ex.Message}\n\n";
                
                if (ex.InnerException != null)
                {
                    errorMessage += $"Детали: {ex.InnerException.Message}";
                }
                
                MessageBox.Show(errorMessage, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
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
            catch (DbUpdateException dbEx)
            {
                string errorMessage = "Ошибка базы данных при удалении студента.\n\n";
                
                if (dbEx.InnerException != null)
                {
                    errorMessage += $"Детали ошибки: {dbEx.InnerException.Message}\n\n";
                    
                    if (dbEx.InnerException.InnerException != null)
                    {
                        errorMessage += $"Дополнительная информация: {dbEx.InnerException.InnerException.Message}";
                    }
                }
                
                errorMessage += "\nВозможные причины:\n" +
                               "- Студент имеет связанные отчёты (нарушение ссылочной целостности)\n" +
                               "- Студент уже был удалён из базы данных\n" +
                               "- Проблемы с подключением к базе данных";
                
                MessageBox.Show(errorMessage, "Ошибка базы данных", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                string errorMessage = $"Ошибка удаления: {ex.Message}\n\n";
                
                if (ex.InnerException != null)
                {
                    errorMessage += $"Детали: {ex.InnerException.Message}";
                }
                
                MessageBox.Show(errorMessage, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
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
                LoadReports(student.Id);
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
            // Валидация email: проверка наличия символа @
            if (!EmailBox.Text.Contains("@"))
            {
                MessageBox.Show("Email должен содержать символ '@'!", "Ошибка валидации", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            // Проверка уникальности email
            var existingStudent = _context.Students
                .FirstOrDefault(s => s.Email == EmailBox.Text && s.Id != _selectedStudent?.Id);
            if (existingStudent != null)
            {
                MessageBox.Show("Email уже зарегистрирован! Пожалуйста, используйте другой email.", "Ошибка валидации", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            if (string.IsNullOrWhiteSpace(PasswordBox.Text))
            {
                MessageBox.Show("Введите пароль!", "Ошибка валидации", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            return true;
        }
        private void DeleteReportBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ReportsGrid.SelectedItem is Report report)
                {
                    var result = MessageBox.Show($"Вы уверены, что хотите удалить отчёт \"{report.Topic}\"?",
                        "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);

                    if (result == MessageBoxResult.Yes)
                    {
                        _context.Reports.Remove(report);
                        _context.SaveChanges();

                        if (_selectedStudent != null)
                        {
                            LoadReports(_selectedStudent.Id);
                        }

                        MessageBox.Show("Отчёт успешно удален!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
                else
                {
                    MessageBox.Show("Выберите отчёт для удаления!", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (DbUpdateException dbEx)
            {
                string errorMessage = "Ошибка базы данных при удалении отчёта.\n\n";
                
                if (dbEx.InnerException != null)
                {
                    errorMessage += $"Детали ошибки: {dbEx.InnerException.Message}\n\n";
                    
                    if (dbEx.InnerException.InnerException != null)
                    {
                        errorMessage += $"Дополнительная информация: {dbEx.InnerException.InnerException.Message}";
                    }
                }
                
                errorMessage += "\nВозможные причины:\n" +
                               "- Отчёт уже был удалён из базы данных\n" +
                               "- Нарушение ссылочной целостности\n" +
                               "- Проблемы с подключением к базе данных";
                
                MessageBox.Show(errorMessage, "Ошибка базы данных", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                string errorMessage = $"Ошибка удаления: {ex.Message}\n\n";
                
                if (ex.InnerException != null)
                {
                    errorMessage += $"Детали: {ex.InnerException.Message}";
                }
                
                MessageBox.Show(errorMessage, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ReportsGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ReportsGrid.SelectedItem is Report report)
            {
                ReportDatePicker.SelectedDate = report.SubmissionDate;
                ReportTopicBox.Text = report.Topic;
                ReportGradeBox.Text = report.Grade.ToString();

                if (report.Company != null && ReportCompanyCombo.Items.Count > 0)
                {
                    ReportCompanyCombo.SelectedItem = ReportCompanyCombo.Items.Cast<Company>()
                        .FirstOrDefault(c => c.Id == report.CompanyId);
                }
            }
        }
        private void AddReportBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_selectedStudent == null)
                {
                    MessageBox.Show("Выберите студента!", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Валидация оценки
                if (!int.TryParse(ReportGradeBox.Text, out int grade))
                {
                    MessageBox.Show("Оценка должна быть числом!", "Ошибка валидации", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (grade < 0 || grade > 100)
                {
                    MessageBox.Show("Оценка должна быть в диапазоне от 0 до 100!", "Ошибка валидации", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var selectedCompany = ReportCompanyCombo.SelectedItem as Company;

                var report = new Report
                {
                    SubmissionDate = ReportDatePicker.SelectedDate ?? DateTime.Now,
                    Topic = ReportTopicBox.Text,
                    Grade = grade,
                    StudentId = _selectedStudent.Id,
                    CompanyId = selectedCompany?.Id  
                };

                _context.Reports.Add(report);
                _context.SaveChanges();

                LoadReports(_selectedStudent.Id);
                ClearReportForm();
                MessageBox.Show("Отчёт успешно добавлен!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (DbUpdateException dbEx)
            {
                string errorMessage = "Ошибка базы данных при добавлении отчёта.\n\n";
                
                if (dbEx.InnerException != null)
                {
                    errorMessage += $"Детали ошибки: {dbEx.InnerException.Message}\n\n";
                    
                    if (dbEx.InnerException.InnerException != null)
                    {
                        errorMessage += $"Дополнительная информация: {dbEx.InnerException.InnerException.Message}";
                    }
                }
                
                errorMessage += "\nВозможные причины:\n" +
                               "- Нарушение целостности данных (студент или компания были удалены)\n" +
                               "- Проблемы с подключением к базе данных\n" +
                               "- Превышение максимальной длины строковых полей";
                
                MessageBox.Show(errorMessage, "Ошибка базы данных", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                string errorMessage = $"Ошибка: {ex.Message}\n\n";
                
                if (ex.InnerException != null)
                {
                    errorMessage += $"Детали: {ex.InnerException.Message}";
                }
                
                MessageBox.Show(errorMessage, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void ClearReportForm()
        {
            ReportDatePicker.SelectedDate = null;
            ReportTopicBox.Clear();
            ReportGradeBox.Clear();
            ReportCompanyCombo.SelectedItem = null;
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
