using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitConverter
{
    public class ObservableWrapper : INotifyCollectionChanged, INotifyPropertyChanged, IList
    {

        public event NotifyCollectionChangedEventHandler CollectionChanged;
        public event PropertyChangedEventHandler PropertyChanged;

        private IList collection;

        public int Count => collection.Count;

        public bool IsReadOnly => collection.IsReadOnly;

        public bool IsFixedSize => collection.IsFixedSize;

        public object SyncRoot => collection.SyncRoot;

        public bool IsSynchronized => collection.IsSynchronized;

        public object this[int index] { get => collection[index]; set => collection[index] = value; }

        public ObservableWrapper(IList collectionSource)
        {
            collection = collectionSource;
        }

        public void RaiseCollectionChangedEvent() =>
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));


        public void RaisePropertyChangedEvent(string propertyName = "") =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        public void Clear()
        {
            collection.Clear();
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Count"));
        }

        public bool Contains(object item) =>
            collection.Contains(item);

        public void CopyTo(Array array, int arrayIndex) =>
            collection.CopyTo(array, arrayIndex);


        int IList.Add(object value)
        {
            int result = collection.Add(value);
            if(result != -1)
            {
                CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, value));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Count"));
            }
            return result;
        }

        public int IndexOf(object value) =>
            collection.IndexOf(value);

        public void Insert(int index, object value)
        {
            collection.Insert(index, value);
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, value));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Count"));
        }

        void IList.Remove(object value)
        {
            collection.Remove(value);
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, value));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Count"));
        }

        public void RemoveAt(int index)
        {
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, collection[index]));
            collection.RemoveAt(index);            
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Count"));
        }

        public IEnumerator GetEnumerator() =>
            collection.GetEnumerator();
    }
}
