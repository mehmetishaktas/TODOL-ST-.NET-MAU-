using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace TodoApp
{
    public class TodoViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<TodoItem> Tasks { get; set; }
        public ICommand AddTaskCommand { get; }
        public ICommand DeleteCommand { get; }

        private string _newTaskTitle;
        public string NewTaskTitle
        {
            get => _newTaskTitle;
            set
            {
                _newTaskTitle = value;
                OnPropertyChanged();
            }
        }

        public TodoViewModel()
        {
            Tasks = new ObservableCollection<TodoItem>();
            AddTaskCommand = new Command(AddTask);
            DeleteCommand = new Command<TodoItem>(DeleteTask);
        }

        private void AddTask()
        {
            if (!string.IsNullOrWhiteSpace(NewTaskTitle))
            {
                Tasks.Add(new TodoItem { Title = NewTaskTitle });
                NewTaskTitle = string.Empty;
            }
        }

        private void DeleteTask(TodoItem task)
        {
            if (Tasks.Contains(task))
            {
                Tasks.Remove(task);
            }
        }
       
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
       
    }
}
