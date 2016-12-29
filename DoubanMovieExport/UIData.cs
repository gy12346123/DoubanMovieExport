using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoubanMovieExport
{
    public class UIData : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        int _TotalPage;

        public int TotalPage
        {
            get { return _TotalPage; }
            set
            {
                if (_TotalPage != value)
                {
                    _TotalPage = value;
                    if (PropertyChanged != null)
                    {
                        PropertyChanged(this, new PropertyChangedEventArgs("TotalPage"));
                    }
                }
            }
        }

        int _LoadPage;

        public int LoadPage
        {
            get { return _LoadPage; }
            set
            {
                if (_LoadPage != value)
                {
                    _LoadPage = value;
                    if (PropertyChanged != null)
                    {
                        PropertyChanged(this, new PropertyChangedEventArgs("LoadPage"));
                    }
                }
            }
        }

        int _SaveItem;

        public int SaveItem
        {
            get { return _SaveItem; }
            set
            {
                if (_SaveItem != value)
                {
                    _SaveItem = value;
                    if (PropertyChanged != null)
                    {
                        PropertyChanged(this, new PropertyChangedEventArgs("SaveItem"));
                    }
                }
            }
        }

        int _Exception;

        public int Exception
        {
            get { return _Exception; }
            set
            {
                if (_Exception != value)
                {
                    _Exception = value;
                    if (PropertyChanged != null)
                    {
                        PropertyChanged(this, new PropertyChangedEventArgs("Exception"));
                    }
                }
            }
        }
    }
}
