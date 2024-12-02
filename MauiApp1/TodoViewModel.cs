using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
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

        private TodoDatabase database;

        public TodoViewModel()
        {
            Tasks = new ObservableCollection<TodoItem>();
            AddTaskCommand = new Command(async () => await AddTask());
            DeleteCommand = new Command<TodoItem>(async (item) => await DeleteTask(item));

            string dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Todo.db3");
            database = new TodoDatabase(dbPath);

            LoadTasks();
        }

        private async void LoadTasks()
        {
            var items = await database.GetItemsAsync();
            foreach (var item in items)
            {
                Tasks.Add(item);
            }
        }

        private async Task AddTask()
        {
            if (!string.IsNullOrWhiteSpace(NewTaskTitle))
            {
                var newTask = new TodoItem { Title = NewTaskTitle };
                await database.SaveItemAsync(newTask);
                Tasks.Add(newTask);
                NewTaskTitle = string.Empty;
            }
        }

        private async Task DeleteTask(TodoItem task)
        {
            if (Tasks.Contains(task))
            {
                Tasks.Remove(task);
                await database.DeleteItemAsync(task);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
