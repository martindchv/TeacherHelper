using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace TeacherHelper.Models.DTOs.Admin.Subjects
{
    public class ThemeDTO : INotifyPropertyChanged
    {
        private ThemeDTO _nextTheme;
        private ThemeDTO _previousTheme;

        public ThemeDTO()
        {
            ChildThemes = new ObservableCollection<ThemeDTO>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public ThemeDTO ParentTheme { get; set; }
        public ThemeDTO PreviousTheme
        {
            get { return this._previousTheme; }
            set
            {
                this._previousTheme = value;
                RaisePropertyChanged();
            }
        }
        public ThemeDTO NextTheme
        {
            get { return this._nextTheme; }
            set
            {
                this._nextTheme = value;
                RaisePropertyChanged();
            }
        }
        public ObservableCollection<ThemeDTO> ChildThemes { get; set; }
        public int TreeDepth { get; set; }

        private object padlock = new object();
        private event PropertyChangedEventHandler propertyChanged;
        public event PropertyChangedEventHandler PropertyChanged
        {
            add
            {
                lock (this.padlock)
                {
                    propertyChanged += value;
                }
            }
            remove
            {
                lock (this.padlock)
                {
                    propertyChanged -= value;
                }

            }
        }
        private void RaisePropertyChanged([CallerMemberName]string propertyName = null)
        {
            if (propertyName == null)
            {
                throw new ArgumentNullException(nameof(propertyName));
            }

            propertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
