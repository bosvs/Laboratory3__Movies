using System.Collections.ObjectModel;
using System.Text.Json;

namespace JSONEditor
{
    public partial class MainPage : ContentPage
    {
        public static string FilePath { get; set; }

        public ObservableCollection<Car> CarsCollection { get; set; }

        public MainPage()
        {
            InitializeComponent();
            CarList.HeightRequest = App.WindowHeight * 0.6667;
            CarsCollection = new ObservableCollection<Car>();
            BindingContext = this;
        }

        /* Метод, що викликається на початку життєвого циклу програми */
        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (FilePath != null)
            {
                RefactoryHelper.UpdateCarList(this, CarsCollection);
            }
        }

        /* Дія кнопки "Обрати файл" */
        private async void OpenFileHandler(object sender, EventArgs e)
        {
            try
            {
                FilePath = await RefactoryHelper.ChooseFile(this, new[] { ".json" });
                if (FilePath == null)
                {
                    return;
                }
                else
                {
                    FilePathLabel.Text = $"Обрано: {FilePath}";
                    RefactoryHelper.UpdateCarList(this, CarsCollection);
                }
            }
            catch(Exception ex) 
            {
                await DisplayAlert("Помилка", "Файл відкрився некоректно", "ОК");
                return;
            }
        }

        /* Дія кнопки "Інформація" */
        private async void InfoButtonHandler(object sender, EventArgs e)
        {
            string studentInfo = "Лабораторна роботу виконав остапович Всеволод та чат гпт, обидва студенти групи К-26" +
                                 "\n\nУ цьому проджекті можна: відкривати, редагувати, видаляти та додавати інформацію, до json файлу" +
                                 " а також швидко виконувати пошук за різними параметрами." +
                                 "\n\nМашини - це предметна область  цього проєкту";
            await DisplayAlert("Про проєкт", studentInfo, "ОК");
        }

        /* Дія кнопки "+Велосипед" */
        private async void AddCarHandler(object sender, EventArgs e)
        {
            if (FilePath == null)
                await DisplayAlert("Помилка", "Не вдається додати вашу автівку в *.json файл!", "OK");
            else
            {
                await Navigation.PushAsync(new AddBikePage());
            }
        }

        /* Дія кнопки "Пошук" */
        private async void SearchHandler(object sender, EventArgs e)
        {
            if (CarsCollection == null || !CarsCollection.Any())
            {
                await DisplayAlert("Увага", "Список атівок пустий", "OK");
                return;
            }

            string modelFilter = ModelEntry.Text?.Trim().ToLower() ?? string.Empty;
            string markFilter = MarkEntry.Text?.Trim().ToLower() ?? string.Empty;
            string wheelDiameterFilter = (WheelDiameterEntry.Text?.Trim().ToLower() ?? string.Empty).Replace('.', ',');
            string weightFilter = (WeightEntry.Text?.Trim().ToLower() ?? string.Empty).Replace('.', ',');
            string typeFilter = TypeEntry.Text?.Trim().ToLower() ?? string.Empty;
            string descriptionFilter = DescriptionEntry.Text?.Trim().ToLower() ?? string.Empty;

            var filteredCars = CarsCollection.Where(car =>
                (string.IsNullOrEmpty(modelFilter) || car.Model.ToLower().Contains(modelFilter)) &&
                (string.IsNullOrEmpty(markFilter) || car.Mark.ToLower().Contains(markFilter)) &&
                (string.IsNullOrEmpty(wheelDiameterFilter) || Double.TryParse(wheelDiameterFilter, out var wheelDiameterFilterValue) && 
                Double.TryParse(car.WheelDiameter, out var wheelDiameter) && wheelDiameter == wheelDiameterFilterValue) &&
                (string.IsNullOrEmpty(weightFilter) || Double.TryParse(weightFilter, out var weightFilterValue) && 
                Double.TryParse(car.Weight, out var weight) && weight == weightFilterValue) &&
                (string.IsNullOrEmpty(typeFilter) || car.Type.ToLower().Contains(typeFilter)) &&
                (string.IsNullOrEmpty(descriptionFilter) || car.Description.ToLower().Contains(descriptionFilter))
            ).ToList();

            if (filteredCars.Any())
            {
                CarsCollectionView.ItemsSource = new ObservableCollection<Car>(filteredCars);
            }
            else
            {
                await DisplayAlert("Результат", "За вашим запитом нічого не знайдено", "OK");
                return;
            }
        }

        /* Дія кнопки "Очистити" */
        private void ClearFiltersHander(object sender, EventArgs e)
        {
            ModelEntry.Text = string.Empty;
            MarkEntry.Text = string.Empty;
            WheelDiameterEntry.Text = string.Empty;
            WeightEntry.Text = string.Empty;
            TypeEntry.Text = string.Empty;
            DescriptionEntry.Text = string.Empty;
        }

        /* Дія кнопки "Глянути" */
        private async void ViewDescriptionHandler(object sender, EventArgs e)
        {
            if (sender is Button button && button.BindingContext is Car car)
            {
                string description = string.IsNullOrWhiteSpace(car.Description) ? "Нічого" : car.Description;
                await DisplayAlert("Опис автівки", description, "OK");
            }
        }
        
        /* Дія кнопки "Редагувати"*/
        private async void EditCarHandler(object sender, EventArgs e)
        {
            if (sender is Button button && button.BindingContext is Car car)
            {
                await Navigation.PushAsync(new EditPage(car));
            }
        }
    }
}
