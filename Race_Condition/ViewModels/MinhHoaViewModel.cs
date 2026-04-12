using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Race_Condition.Views;

namespace Race_Condition.ViewModels
{
    public class MinhHoaViewModel : BaseViewModel
    {
        public ICommand DemoWithoutLockCommand { get; set; }
        public ICommand DemoWithLockCommand { get; set; }

        public ObservableCollection<string> Logs { get; set; } = new();

        private int _giaTriKyVong;
        public int GiaTriKyVong
        {
            get => _giaTriKyVong;
            set { _giaTriKyVong = value; OnPropertyChanged(); }
        }

        private int _giaTriThuc;
        public int GiaTriThuc
        {
            get => _giaTriThuc;
            set { _giaTriThuc = value; OnPropertyChanged(); }
        }

        private string _status;
        public string Status
        {
            get => _status;
            set { _status = value; OnPropertyChanged(); }
        }

        private readonly object _khoaObj = new();

        public MinhHoaViewModel()
        {
            DemoWithoutLockCommand = new RelayCommand<object>(p => true, async p => await MinhHoaAsync(false));
            DemoWithLockCommand = new RelayCommand<object>(p => true, async p => await MinhHoaAsync(true));
        }

        private async Task MinhHoaAsync(bool khoa)
        {
            Logs.Clear();

            int count = 10;
            GiaTriKyVong = 12;
            GiaTriThuc = 0;

            ThemLog($"Dữ liệu chia sẻ chung ban đầu: count = {count}");

            Task luongA = Task.Run(() =>
            {
                if (khoa)
                {
                    ThemLog("Luồng A yêu cầu khóa.");
                    lock (_khoaObj)
                    {
                        ThemLog("Luồng A đã vào vùng khóa.");
                        int docA = count;
                        ThemLog($"Luồng A đọc count = {docA}");
                        int tamA = docA + 1;
                        ThemLog($"Luồng A xử lý: {docA} + 1 = {tamA}");
                        Thread.Sleep(50);
                        count = tamA;
                        ThemLog($"Luồng A ghi count = {count}");
                        ThemLog("Luồng A rời vùng khóa.");
                    }
                }
                else
                {
                    int docA = count;
                    ThemLog($"Luồng A đọc count = {docA}");
                    int tamA = docA + 1;
                    ThemLog($"Luồng A xử lý: {docA} + 1 = {tamA}");
                    Thread.Sleep(50);
                    count = tamA;
                    ThemLog($"Luồng A ghi count = {count}");
                }
            });

            Task luongB = Task.Run(() =>
            {
                if (khoa)
                {
                    ThemLog("Luồng B yêu cầu khóa.");
                    lock (_khoaObj)
                    {
                        ThemLog("Luồng B đã vào vùng khóa.");
                        int docB = count;
                        ThemLog($"Luồng B đọc count = {docB}");
                        int tamB = docB + 1;
                        ThemLog($"Luồng B xử lý: {docB} + 1 = {tamB}");
                        Thread.Sleep(60);
                        count = tamB;
                        ThemLog($"Luồng B ghi count = {count}");
                        ThemLog("Luồng B rời vùng khóa.");
                    }
                }
                else
                {
                    int docB = count;
                    ThemLog($"Luồng B đọc count = {docB}");
                    int tamB = docB + 1;
                    ThemLog($"Luồng B xử lý: {docB} + 1 = {tamB}");
                    Thread.Sleep(60);
                    count = tamB;
                    ThemLog($"Luồng B ghi count = {count}");
                }
            });

            await Task.WhenAll(luongA, luongB);

            GiaTriThuc = count;

            Status = khoa
                ? "Có khóa: mỗi luồng phải hoàn tất read-modify-write rồi luồng còn lại mới được vào."
                : "Không có khóa: hai luồng có thể cùng đọc dữ liệu cũ và ghi đè kết quả của nhau.";

            ThemLog($"Kết quả cuối cùng: count = {GiaTriThuc}");
        }

        private void ThemLog(string noiDung)
        {
            Application.Current.Dispatcher.Invoke(() => Logs.Add(noiDung));
        }
    }
}