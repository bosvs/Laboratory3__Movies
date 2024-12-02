using System.Collections.ObjectModel;
using System.Text.Json;

namespace JSONEditor;

public partial class AddBikePage : ContentPage
{
    public AddBikePage()
    {
        InitializeComponent();
    }

    /* Дія кнопки "Зберегти" */
    private async void SaveButtonHandler(object sender, EventArgs e)
    {
        // Валідація
        if (!RefactoryHelper.IsInvalidInputFieldsAlert(this, ModelEntry, WheelDiameterEntry, WeightEntry)[0])
        {
            await DisplayAlert("Увага!", "Обов'язкові поля вони на те і є обов'язкові:" +
            "\n\'Модель\', \'Діаметр коліс\' та \'Вага\'", "ОК");
            return;
        }
        else if (!RefactoryHelper.IsInvalidInputFieldsAlert(this, ModelEntry, WheelDiameterEntry, WeightEntry)[1])
        {
            await DisplayAlert("Увага!", "Поля: \'Діаметр дисків\' та \'Вага\' мають бути числами більшими за 0", "ОК");
            return;
        }

        // Створення нової авттівки
        var newBike = new Car
        {
            Model = ModelEntry.Text?.Trim(),
            Mark = MarkEntry.Text?.Trim(),
            WheelDiameter = WheelDiameterEntry.Text?.Trim(),
            Weight = WeightEntry.Text?.Trim(),
            Type = TypeEntry.Text?.Trim(),
            Description = DescriptionEntry.Text?.Trim()
        };

        // Загррузка автівок з файлу
        var filePath = Path.Combine(FileSystem.AppDataDirectory, MainPage.FilePath);
        ObservableCollection<Car> bikesList;

        if (File.Exists(filePath))
        {
            var json = await File.ReadAllTextAsync(filePath);
            bikesList = JsonSerializer.Deserialize<ObservableCollection<Car>>(json);
        }
        else bikesList = new ObservableCollection<Car>();

        // Пушнути ну автівку до списку
        bikesList.Add(newBike);

        // Збереження оновленого списку у файл
        try
        {
            RefactoryHelper.RewriteJson(bikesList);
        }
        catch (Exception ex)
        {
            await DisplayAlert("Помлкка", "Під час " +
                                   "\nПеревірте, що з вашим файлом", "ОК");
        }

        // Повернення до мейн сторінки
        await Navigation.PopAsync();
    }

    // кнопка відміни
    private async void CancelButtonHandler(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }
}
