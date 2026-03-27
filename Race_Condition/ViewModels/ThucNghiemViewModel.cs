using Race_Condition.Models;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Race_Condition.Views;

namespace Race_Condition.ViewModels
{
    public class ThucNghiemViewModel:BaseViewModel
    {
            public ICommand RunWithoutLockCommand { get; set; }
            public ICommand RunWithLockCommand { get; set; }

            private int _demTienTrinh = 10;
            public int DemTienTrinh
            {
                get { return _demTienTrinh; }
                set { _demTienTrinh = value; OnPropertyChanged(); }
            }
        private int _iterations = 100000;
            public int Iterations
            {
                get { return _iterations; }
                set { _iterations = value; OnPropertyChanged(); }
            }

            private int _giaTriDauTien = 0;
            public int GiaTriDauTien
            {
                get { return _giaTriDauTien; }
                set { _giaTriDauTien = value; OnPropertyChanged(); }
            }
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
            private double _thoiGianThucHien;
            public double ThoiGianThucHien
            {
                get => _thoiGianThucHien;
                set { _thoiGianThucHien = value; OnPropertyChanged(); }
            }
            private string _status;
            public string Status
            {
                get => _status;
                set { _status = value; OnPropertyChanged(); }
            }
            private ObservableCollection<string> _logs;
            public ObservableCollection<string> Logs
            {
                get => _logs;
                set { _logs = value; OnPropertyChanged(); }
            }

            private readonly object _khoaobj = new object();
            public ThucNghiemViewModel()
            {
                Logs = new ObservableCollection<string>();
                RunWithoutLockCommand = new RelayCommand<object>(
                                (p) => true,
                                async (p) => await ChayMoPhongAsync(false)
                            );
                RunWithLockCommand = new RelayCommand<object>(
                    (p) => true,
                    async (p) => await ChayMoPhongAsync(true)
                );
            }
            private async Task ChayMoPhongAsync(bool Khoa)
            {
                Logs.Clear();
                Status = "Đang chạy mô phỏng...";
                GiaTriKyVong = GiaTriDauTien + (DemTienTrinh * Iterations);
                GiaTriThuc = 0;
                ThoiGianThucHien = 0;
                CauHinhTieuTrinhModel config = new CauHinhTieuTrinhModel
                {
                    DemTieuTrinh = DemTienTrinh,
                    GiaTriBanDau = GiaTriDauTien,
                    Iterations = Iterations,
                    Khoa = Khoa

                };
                MoPhongKetQuaModel kq = new MoPhongKetQuaModel
                {
                    Mode = Khoa ? "Có Lock (An toàn)" : "Không Lock (Bị Race Condition)"
                };
                ThemLog($"--- BẮT ĐẦU CHẠY: {kq.Mode} ---");
                int bienChung = config.GiaTriBanDau;
                Stopwatch dongHo = Stopwatch.StartNew();

                Task[] danhSachTask = new Task[config.DemTieuTrinh];

                await Task.Run(() =>
                {
                    for (int i = 0; i < config.DemTieuTrinh; i++)
                    {
                        int maTienTrinh = i + 1;
                        danhSachTask[i] = Task.Run(() =>
                        {
                            ThemLog($"Tiến trình {maTienTrinh} bắt đầu.");
                            for (int j = 0; j < config.Iterations; j++)
                            {
                                if (config.Khoa)
                                {
                                    lock (_khoaobj)
                                    {
                                        bienChung++;
                                    }
                                }
                                else
                                {
                                    bienChung++;
                                }
                            }
                            ThemLog($"Tiến trình {maTienTrinh} kết thúc.");
                        });
                    }
                    Task.WaitAll(danhSachTask);
                });
                dongHo.Stop();
                kq.GiaTriKyVong = GiaTriKyVong;
                kq.GiaTriThuc = bienChung;
                kq.ThoiGianThucHien = dongHo.Elapsed.TotalMicroseconds;
                kq.IsRaceCondition = (kq.GiaTriThuc != kq.GiaTriKyVong);
                GiaTriThuc = kq.GiaTriThuc;
                ThoiGianThucHien = kq.ThoiGianThucHien;
                if (kq.IsRaceCondition)
                    Status = "Phát hiện Race Condition! Dữ liệu bị thất thoát.";
                else
                    Status = "An toàn. Dữ liệu chuẩn xác 100%.";

                ThemLog($"--- KẾT THÚC. Tốn {ThoiGianThucHien} ms ---");
            }
            private void ThemLog(string noiDung)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    Logs.Add(noiDung);
                });
            }
        }
}

