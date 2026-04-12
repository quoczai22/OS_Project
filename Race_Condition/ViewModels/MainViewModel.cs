using Race_Condition.Models;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Race_Condition.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        public ICommand RunWithoutLockCommand { get; set; }
        public ICommand RunWithLockCommand { get; set; }
        public ICommand ResetCommand { get; set; }

        private int _demTienTrinh = 10;
        public int DemTienTrinh
        {
            get => _demTienTrinh;
            set { _demTienTrinh = value; OnPropertyChanged(); }
        }

        private int _iterations = 100000;
        public int Iterations
        {
            get => _iterations;
            set { _iterations = value; OnPropertyChanged(); }
        }

        private int _giaTriDauTien = 0;
        public int GiaTriDauTien
        {
            get => _giaTriDauTien;
            set { _giaTriDauTien = value; OnPropertyChanged(); }
        }

        private int _giaTriKyVong;
        public int GiaTriKyVong { get => _giaTriKyVong; set { _giaTriKyVong = value; OnPropertyChanged(); } }

        private int _giaTriThuc;
        public int GiaTriThuc { get => _giaTriThuc; set { _giaTriThuc = value; OnPropertyChanged(); } }

        private double _thoiGianThucHien;
        public double ThoiGianThucHien { get => _thoiGianThucHien; set { _thoiGianThucHien = value; OnPropertyChanged(); } }

        private string _status = "Sẵn sàng";
        public string Status { get => _status; set { _status = value; OnPropertyChanged(); } }

        private ObservableCollection<string> _logs;
        public ObservableCollection<string> Logs { get => _logs; set { _logs = value; OnPropertyChanged(); } }

        private readonly object _khoaobj = new object();

        public MainViewModel()
        {
            Logs = new ObservableCollection<string>();

            // Luôn cho phép bấm nút bằng cách trả về true
            RunWithoutLockCommand = new RelayCommand<object>(p => true, async p => await ChayMoPhongAsync(false));
            RunWithLockCommand = new RelayCommand<object>(p => true, async p => await ChayMoPhongAsync(true));
            ResetCommand = new RelayCommand<object>(p => true, p => Reset());
        }

        private void Reset()
        {
            DemTienTrinh = 10;
            Iterations = 100000;
            GiaTriDauTien = 0;
            GiaTriKyVong = 0;
            GiaTriThuc = 0;
            ThoiGianThucHien = 0;
            Status = "Sẵn sàng";
            Logs.Clear();
            ThemLog("Hệ thống đã được đưa về trạng thái mặc định.");
        }

        private async Task ChayMoPhongAsync(bool Khoa)
        {
            Logs.Clear();
            Status = "Đang chạy mô phỏng...";

            GiaTriKyVong = GiaTriDauTien + (DemTienTrinh * Iterations);
            int bienChung = GiaTriDauTien;

            Stopwatch dongHo = Stopwatch.StartNew();
            Task[] danhSachTask = new Task[DemTienTrinh];

            await Task.Run(() => {
                for (int i = 0; i < DemTienTrinh; i++)
                {
                    int maTienTrinh = i + 1;
                    danhSachTask[i] = Task.Run(() => {
                        ThemLog($"Tiến trình {maTienTrinh} bắt đầu.");
                        for (int j = 0; j < Iterations; j++)
                        {
                            if (Khoa) { lock (_khoaobj) { bienChung++; } }
                            else { bienChung++; }
                        }
                        ThemLog($"Tiến trình {maTienTrinh} kết thúc.");
                    });
                }
                Task.WaitAll(danhSachTask);
            });

            dongHo.Stop();
            GiaTriThuc = bienChung;
            ThoiGianThucHien = dongHo.Elapsed.TotalMilliseconds;

            if (GiaTriThuc != GiaTriKyVong)
                Status = "PHÁT HIỆN RACE CONDITION!";
            else
                Status = "AN TOÀN - Dữ liệu chính xác.";

            ThemLog($"--- KẾT THÚC. Tổng thời gian: {ThoiGianThucHien:N2} ms ---");
        }

        private void ThemLog(string noiDung)
        {
            Application.Current.Dispatcher.Invoke(() => {
                Logs.Insert(0, noiDung);
            });
        }
    }
}