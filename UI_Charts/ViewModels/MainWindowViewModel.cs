using Prism.Mvvm;

namespace UI_Charts.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private string _title = "Редактор блок-схем";

        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }
    }
}