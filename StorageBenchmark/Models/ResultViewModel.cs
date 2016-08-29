using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StorageBenchmark.Models
{
    public class ResultViewModel : INotifyPropertyChanged
    {
        private double? _writeSpeed;
        private double? _readSpeed;
        private bool _isRunning;
        
        public double? ReadSpeed
        {
            get { return _readSpeed; }
            set
            {
                _readSpeed = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ReadSpeed"));
            }
        }


        public double? WriteSpeed
        {
            get { return _writeSpeed; }
            set
            {
                _writeSpeed = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("WriteSpeed"));
            }
        }

        public int Times { get; set; } = 3;


        public bool IsRunning
        {
            get { return _isRunning; }
            set
            {
                _isRunning = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IsRunning"));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IsStopped"));
            }
        }

        public bool IsStopped => !IsRunning;


        public event PropertyChangedEventHandler PropertyChanged;

    }
}
