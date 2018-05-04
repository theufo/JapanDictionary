using System.ComponentModel;
using System.Runtime.CompilerServices;
using JapanDictionary.Annotations;

namespace JapanDictionary.Common
{
    public sealed class StatusService : INotifyPropertyChanged
    {
        private string _text;

        private StatusService()
        {
            // Initialize.
        }

        public string Text
        {
            get => _text;
            set
            {
                _text = value;
                OnPropertyChanged(nameof(Text));
            }
        }

        public static StatusService Instance { get; } = new StatusService();

        public event PropertyChangedEventHandler PropertyChanged;

        public void SetReady()
        {
            Text = "Ready";
        }

        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}