using System;
using System.ComponentModel;
using ProjectManager.Models;

namespace ProjectManager.ViewModels
{
    public class ProjectDialogViewModel : INotifyPropertyChanged
    {
        private string _name;
        private string _description;
        private DateTime? _deadline;
        private string _notes;

        public Project Project { get; set; }

        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged(nameof(Name));
            }
        }

        public string Description
        {
            get => _description;
            set
            {
                _description = value;
                OnPropertyChanged(nameof(Description));
            }
        }

        public DateTime? Deadline
        {
            get => _deadline;
            set
            {
                _deadline = value;
                OnPropertyChanged(nameof(Deadline));
            }
        }

        public string Notes
        {
            get => _notes;
            set
            {
                _notes = value;
                OnPropertyChanged(nameof(Notes));
            }
        }

        public ProjectDialogViewModel()
        {
            Project = new Project();
        }

        public ProjectDialogViewModel(Project project)
        {
            Project = project;
            Name = project.Name;
            Description = project.Description;
            Deadline = project.Deadline;
            Notes = project.Notes;
        }

        public bool Validate()
        {
            if (string.IsNullOrWhiteSpace(Name))
            {
                return false;
            }
            return true;
        }

        public void Save()
        {
            if (!Validate())
            {
                throw new InvalidOperationException("Название проекта не может быть пустым");
            }

            Project.Name = Name;
            Project.Description = Description;
            Project.Deadline = Deadline;
            Project.Notes = string.IsNullOrWhiteSpace(Notes) ? null : Notes.Trim();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}


