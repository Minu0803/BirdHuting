using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using WpfApp2.Model;

namespace WpfApp2.ViewModel
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<IObject> ObjectList { get; }
        public ObservableCollection<int> GridIndex { get; } = new ObservableCollection<int>(Enumerable.Range(0, 15));

        private bool _isGameRunning = false;
        private Thread _gameThread;
        private ManualResetEvent _loopWait = new ManualResetEvent(false);
        private int _score;

        private bool _isHunting = false;
        private bool _isBirdCreating = false;
        public bool IsHunting
        {
            get => _isHunting;
            set
            {
                if (_isHunting != value)
                {
                    _isHunting = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool IsBirdCreating
        {
            get => _isBirdCreating;
            set
            {
                if (_isBirdCreating != value)
                {
                    _isBirdCreating = value;
                    OnPropertyChanged();
                }
            }
        }
        public int Score
        {
            get => _score;
            set
            {
                if (_score != value)
                {
                    _score = value;
                    OnPropertyChanged();
                }
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;

        public ICommand ToggleHuntingCommand => new RelayCommand(x => IsHunting = !IsHunting);
        public ICommand ToggleBirdCreationCommand => new RelayCommand(x => IsBirdCreating = !IsBirdCreating);

        public MainViewModel()
        {
            ObjectList = new ObservableCollection<IObject>
            {
                new Hunter { Name = "사냥꾼", Image = LoadImage("Images/사냥꾼.jpg"), Row = 2, Column = 0 }
            };

            StartGameLoop(); 
        }

        private BitmapImage LoadImage(string path)
        {
            BitmapImage bmp = new BitmapImage();
            bmp.BeginInit();
            bmp.UriSource = new Uri(path, UriKind.Relative);
            bmp.CacheOption = BitmapCacheOption.OnLoad;
            bmp.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
            bmp.EndInit();
            return bmp;
        }

        private void StartGameLoop()
        {
            _isGameRunning = true;

            _gameThread = new Thread(GameLoop);
            _gameThread.IsBackground = true;
            _gameThread.Start();
        }

        private void GameLoop()
        {
            var rand = new Random();
            DateTime lastHunterMove = DateTime.Now;
            DateTime lastBirdCreate = DateTime.Now;
            TimeSpan hunterInterval = TimeSpan.FromSeconds(2); // 사냥꾼은 2초마다 동작
            TimeSpan birdInterval = TimeSpan.FromSeconds(3); // 새는 3초마다 동작

            while (_isGameRunning)
            {
                if (IsHunting && (DateTime.Now - lastHunterMove) >= hunterInterval) // 마지막 이동 이후 2초가 지났다면
                {
                    Application.Current.Dispatcher.BeginInvoke(new Action(() => MoveHunter(rand)));
                    lastHunterMove = DateTime.Now;
                }

                if (IsBirdCreating && (DateTime.Now - lastBirdCreate) >= birdInterval && ObjectList.Count < 15) // 마지막 이동 이후 3초가 지났다면
                {
                    Application.Current.Dispatcher.BeginInvoke(new Action(() => CreateBird(rand)));
                    lastBirdCreate = DateTime.Now;
                }
                _loopWait.WaitOne(100); 
            }
        }

        private void MoveHunter(Random rand)
        {
            var hunter = GetHunter();
            if (hunter == null) return;

            int currentRow = hunter.Row;
            int currentCol = hunter.Column;

            var directions = new List<(int r, int c)>
            {
                (currentRow - 1, currentCol),
                (currentRow + 1, currentCol),
                (currentRow, currentCol - 1),
                (currentRow, currentCol + 1)
            };

            var validDirs = directions.Where(d => d.r >= 0 && d.r <= 2 && d.c >= 0 && d.c <= 4).ToList();
            if (validDirs.Count == 0) return;

            var (newRow, newCol) = validDirs[rand.Next(validDirs.Count)];
            hunter.Row = newRow;
            hunter.Column = newCol;

            var target = ObjectList.OfType<IBird>().FirstOrDefault(b => b.Row == newRow && b.Column == newCol);
            if (target != null)
            {
                Score += target.Score;
                ObjectList.Remove(target);
            }
        }
        private Hunter GetHunter()
        {
            return ObjectList.OfType<Hunter>().FirstOrDefault();
        }

        private void CreateBird(Random rand)
        {
            var occupied = ObjectList.Select(x => (x.Row, x.Column)).ToList();
            var empty = new List<(int Row, int Col)>();
            IBird bird;

            for (int r = 0; r < 3; r++)
            {
                for (int c = 0; c < 5; c++)
                {
                    if (!occupied.Contains((r, c)))
                        empty.Add((r, c));
                }
            }

            if (empty.Count == 0) return;
            var pos = empty[rand.Next(empty.Count)];            

            switch (rand.Next(3))
            {
                case 0:
                    bird = new Cuckoo { Name = "뻐꾸기", Image = LoadImage("Images/뻐꾸기.jpg"), Score = 10, Row = pos.Row, Column = pos.Col };
                    break;
                case 1:
                    bird = new Pigeon { Name = "비둘기", Image = LoadImage("Images/비둘기.jpg"), Score = 20, Row = pos.Row, Column = pos.Col };
                    break;
                default:
                    bird = new Kestrel { Name = "황조롱이", Image = LoadImage("Images/황조롱이.jpg"), Score = 30, Row = pos.Row, Column = pos.Col };
                    break;
            }

            ObjectList.Add(bird);
        }                

        protected void OnPropertyChanged([CallerMemberName] string name = "") =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
