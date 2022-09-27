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
    public class ObservableListWrapper : INotifyCollectionChanged, INotifyPropertyChanged, IList
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

        public ObservableListWrapper(IList collectionSource)
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



    public class ObservableListWrapper<T> : INotifyCollectionChanged, INotifyPropertyChanged, IList<T>, IList 
    {
        List<T> list;

        public int Count => list.Count;

        public bool IsReadOnly => (list as IList).IsReadOnly;

        public bool IsFixedSize => (list as IList).IsFixedSize;

        public object SyncRoot => (list as IList).SyncRoot;

        public bool IsSynchronized => (list as IList).IsSynchronized;

        object IList.this[int index] { get => list[index]; set => list[index] = (T)value; }
        public T this[int index] { get => list[index]; set => list[index] = (T)value; }

        public event NotifyCollectionChangedEventHandler CollectionChanged;
        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableListWrapper(List<T> collectionSource)
        {
            list = collectionSource;
        }

        public void RaiseCollectionChangedEvent() =>
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));


        public void RaisePropertyChangedEvent(string propertyName = "") =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        public int IndexOf(T item) => list.IndexOf(item);

        public void Insert(int index, T value)
        {
            list.Insert(index, value);
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, value));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Count"));
        }

        public void RemoveAt(int index)
        {
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, list[index]));
            list.RemoveAt(index);
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Count"));
        }

        public void Add(T item)
        {
            list.Add(item);
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Count"));
        }

        public void Clear()
        {
            list.Clear();
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Count"));
        }

        public bool Contains(T item) => list.Contains(item);

        public void CopyTo(T[] array, int arrayIndex) => list.CopyTo(array, arrayIndex);

        public bool Remove(T item)
        {
            if (list.Remove(item))
            {
                CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Count"));
                return true;
            }
            else
                return false;
        }

        public IEnumerator<T> GetEnumerator() => list.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => list.GetEnumerator();

        public int Add(object value)
        {
            int result = (list as IList).Add(value);
            if (result != -1)
            {
                CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, value));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Count"));
            }
            return result;
        }

        public bool Contains(object value) => (list as IList).Contains(value);

        public int IndexOf(object value) => (list as IList).IndexOf(value);

        public void Insert(int index, object value)
        {
            (list as IList).Insert(index, value);
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, value));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Count"));
        }

        public void Remove(object value)
        {
            (list as IList).Remove(value);
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, value));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Count"));
        }

        public void CopyTo(Array array, int index) => (list as IList).CopyTo(array, index);
    }
}
