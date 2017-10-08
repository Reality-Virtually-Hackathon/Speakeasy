using System.ComponentModel;

namespace UB.SimpleCS.Models
{
    /// <summary>
    /// Base class for entities. All rest api ingoingand outcoming entities must be inherited from this class
    /// </summary>
    public abstract class BaseDto : INotifyPropertyChanged
    {
        /// <summary>
        /// If class needs to be validated before sending to the network you can override and fill its content here.
        /// </summary>
        public virtual void Validate()
        {
            //For example, check class properties and throw an exception if something is wrong
        }

        /// <summary>
        /// For future binding purposes
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
