using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace TodoApp
{
    public class GroupedTodoItems : ObservableCollection<TodoItem>
    {
        public string Name { get; private set; }

        public GroupedTodoItems(string name)
        {
            Name = name;
        }
    }

    public class TodoViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<GroupedTodoItems> GroupedTasks { get; set; }
        public ObservableCollection<string> Categories { get; set; }
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

        private string _selectedCategory;
        public string SelectedCategory
        {
            get => _selectedCategory;
            set
            {
                _selectedCategory = value;
                OnPropertyChanged();
            }
        }

        private TodoDatabase database;

        public TodoViewModel()
        {
            GroupedTasks = new ObservableCollection<GroupedTodoItems>();
            Categories = new ObservableCollection<string> { "Spor", "Ders", "İş", "Kişisel" };
            AddTaskCommand = new Command(async () => await AddTask());
            DeleteCommand = new Command<TodoItem>(async (item) => await DeleteTask(item));

            string dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Todo.db3");
            database = new TodoDatabase(dbPath);

            LoadTasks();
        }

        private async void LoadTasks()
        {
            var items = await database.GetItemsAsync();
            GroupedTasks.Clear();

            var groupedItems = items.GroupBy(t => t.Category);
            foreach (var group in groupedItems)
            {
                var groupItems = new GroupedTodoItems(group.Key);
                foreach (var item in group)
                {
                    item.PropertyChanged += async (s, e) => await UpdateTask((TodoItem)s);
                    groupItems.Add(item);
                }
                GroupedTasks.Add(groupItems);
            }
        }

        private async Task AddTask()
        {
            if (!string.IsNullOrWhiteSpace(NewTaskTitle) && !string.IsNullOrWhiteSpace(SelectedCategory))
            {
                var newTask = new TodoItem { Title = NewTaskTitle, Category = SelectedCategory };
                await database.SaveItemAsync(newTask);
                newTask.PropertyChanged += async (s, e) => await UpdateTask((TodoItem)s);

                var group = GroupedTasks.FirstOrDefault(g => g.Name == SelectedCategory);
                if (group == null)
                {
                    group = new GroupedTodoItems(SelectedCategory);
                    GroupedTasks.Add(group);
                }
                group.Add(newTask);
                NewTaskTitle = string.Empty;
            }
        }

        private async Task UpdateTask(TodoItem task)
        {
            await database.UpdateItemAsync(task);
        }

        private async Task DeleteTask(TodoItem task)
        {
            foreach (var group in GroupedTasks)
            {
                if (group.Contains(task))
                {
                    group.Remove(task);
                    if (group.Count == 0)
                    {
                        GroupedTasks.Remove(group);
                    }
                    break;
                }
            }
            await database.DeleteItemAsync(task);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
