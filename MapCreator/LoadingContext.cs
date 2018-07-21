using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MapCreator
{
    class LoadingContext: INotifyPropertyChanged
    { 
        public int Progress
        {
            get { return progress; }
            set
            {
                progress = value;
                OnPropertyChanged("Progress");
            }
        }
        public Visibility IsVisible
        {
            get {
                if (isActive) return Visibility.Visible;
                else return Visibility.Collapsed;
                }
        }
        
        public bool IsActive
        {
            get
            {
                return isActive;
            }

            set
            {
                isActive = value;
                OnPropertyChanged("IsActive");
                OnPropertyChanged("IsVisible");
            }
        }

        public string Label
        {
            get
            {
                return label;
            }

            set
            {
                label = value;
                OnPropertyChanged("Label");
            }
        }

        public DelegateCommand OnCancel
        {
            get
            {
                return onCancel;
            }

            set
            {
                onCancel = value;
            }
        }



        public bool IsIndeterminate
        {
            get { return isIndeterminate; }
            private set
            {
                isIndeterminate = value;
                OnPropertyChanged("IsIndeterminate");
            }
        }

        bool isActive;
        string label;
        int totalWork;
        int progress=0;
        bool isIndeterminate = false;
        BackgroundWorker worker;
        void OnPropertyChanged(String prop)
        {
            PropertyChangedEventHandler handler = PropertyChanged;

            if (handler != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
        DelegateCommand onCancel;
        public LoadingContext(DoWorkEventHandler Load,int totalWork)
        {
            this.totalWork = totalWork;
            OnCancel = new DelegateCommand(x => OnCancelExecute());
            IsActive = true;
            worker = new BackgroundWorker();
            worker.DoWork += Load;
            worker.WorkerReportsProgress = true;
            worker.ProgressChanged += Worker_ProgressChanged;
            worker.WorkerSupportsCancellation = true;
            worker.RunWorkerAsync();
            worker.RunWorkerCompleted += Worker_RunWorkerCompleted;
        }

        private void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            OnCancelExecute();
        }

        private void Worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (e.ProgressPercentage < 0)
            {
                IsIndeterminate = true;
            }
            else
            {
                Progress = (100*e.ProgressPercentage)/totalWork;
                
            }
            Label = e.UserState as String;
        }
        private void OnCancelExecute()
        {
            IsActive = false;
            worker.CancelAsync();
        }


    }
}
