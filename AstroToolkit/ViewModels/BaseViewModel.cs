using CommunityToolkit.Mvvm.ComponentModel;

namespace AstroToolkit.ViewModels
{
    public partial class BaseViewModel : ObservableObject
    {
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsNotBusy))]
        private bool _isBusy;

        [ObservableProperty]
        private string _title;

        [ObservableProperty]
        private string _errorMessage;

        [ObservableProperty]
        private bool _hasError;

        public bool IsNotBusy => !IsBusy;

        protected bool SetBusyState(bool isBusy, string errorMessage = null)
        {
            IsBusy = isBusy;
            
            if (!string.IsNullOrEmpty(errorMessage))
            {
                ErrorMessage = errorMessage;
                HasError = true;
            }
            else
            {
                ErrorMessage = string.Empty;
                HasError = false;
            }

            return !HasError;
        }

        protected async Task<bool> ExecuteAsync(Func<Task> operation, string errorHandler = null)
        {
            IsBusy = true;
            HasError = false;
            
            try
            {
                await operation();
                return true;
            }
            catch (Exception ex)
            {
                HasError = true;
                ErrorMessage = $"{errorHandler ?? "Error"}: {ex.Message}";
                Console.WriteLine($"Error in {GetType().Name}: {ex}");
                return false;
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
