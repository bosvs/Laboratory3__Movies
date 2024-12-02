using System.Collections.ObjectModel;
using System.Text.Json;

namespace JSONEditor;

public partial class EditPage : ContentPage
{
    private Bike Bike { get; set; }

    public EditPage(Bike bike)
    {
        InitializeComponent();
        Bike = bike;
        BindingContext = Bike;
    }

    /* Кнопка "Зберегти" */
    private async void SaveButtonHandler(object sender, EventArgs e)
    {
        try
        {
            if (Application.Current.MainPage is NavigationPage navigationPage &&
                navigationPage.RootPage is MainPage mainPage)
            {
                var bikeInCollection = mainPage.BikesCollection.FirstOrDefault(b => b.Model == Bike.Model);

                if (new[] { ModelEntry.Text, FrameMaterialEntry.Text, WheelDiameterEntry.Text, WeightEntry.Text, DescriptionEntry.Text }
                    .All(string.IsNullOrWhiteSpace))
                {
                    bool confirmDelete = await DisplayAlert("Видалення інформації", "Ви очно хочете видалити це", "Так", "Ні");
                    if (confirmDelete)
                    {
                        mainPage.BikesCollection.Remove(bikeInCollection);

                        RefactoryHelper.RewriteJson(mainPage.BikesCollection);

                        await DisplayAlert("Перемога", "Інформаціія видалено!", "OK");
                        await Navigation.PopAsync();
                        return;
                    }
                    else
                    {
                        await Navigation.PopAsync();
                    }
                }

                // Перевірка на наявність порожніх полів
                if (!RefactoryHelper.IsInvalidInputFieldsAlert(this, ModelEntry, WheelDiameterEntry, WeightEntry)[0])
                {
                    await DisplayAlert("Помилка!", "Будь ласка заповніть поля із зірочкою", "ОК");
                    return;
                }
                else if (!RefactoryHelper.IsInvalidInputFieldsAlert(this, ModelEntry, WheelDiameterEntry, WeightEntry)[1])
                {
                    await DisplayAlert("Помилка!", "Обов'язкові поля повинні містити правильні значення", "ОК");
                    return;
                }

                // Збереження даних
                bikeInCollection.Model = ModelEntry.Text?.Trim() ?? string.Empty;
                bikeInCollection.FrameMaterial = FrameMaterialEntry.Text?.Trim() ?? string.Empty;
                bikeInCollection.WheelDiameter = WheelDiameterEntry.Text?.Trim() ?? string.Empty;
                bikeInCollection.Weight = WeightEntry.Text?.Trim() ?? string.Empty;
                bikeInCollection.Type = TypeEntry.Text?.Trim() ?? string.Empty;
                bikeInCollection.Description = DescriptionEntry.Text?.Trim() ?? string.Empty;

                // Перезапис даних у файл
                RefactoryHelper.RewriteJson(mainPage.BikesCollection);

                await DisplayAlert("Перемога", "Запис успішно збережено!", "OK");

                await Navigation.PopAsync();
            }
            else
            {
                await DisplayAlert("Помилка", "Виникла помилка при отриманні головної сторінки.", "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Помилка", $"Сталася помилка при збереженні даних. Технічна помилка:\n{ex.Message}", "OK");
        }
    }

    /* Кнопка "Скасувати" */
    private async void CancelButtonHandler(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }
}
