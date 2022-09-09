﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitConverter
{
    /// <summary>
    /// A observable wrapper for a generic collection. It does not copy the collection source. Instead, it keeps a reference to the collection source. In addition, the caller can explicitly invoke the CollectionChanged and PropertyChanged methods when anythin in the collection is updated.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ObservableWrapper<T> : INotifyCollectionChanged, INotifyPropertyChanged, ICollection<T>
    {
        public event NotifyCollectionChangedEventHandler CollectionChanged;
        public event PropertyChangedEventHandler PropertyChanged;

        private ICollection<T> collection;

        public int Count => collection.Count;

        public bool IsReadOnly => collection.IsReadOnly;

        public ObservableWrapper(ICollection<T> collectionSource)
        {
            collection = collectionSource;
        }

        public void RaiseCollectionChangedEvent() =>
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));


        public void RaisePropertyChangedEvent(string propertyName = "") =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        public void Add(T item)
        {
            collection.Add(item);
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add,item));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Count"));
        }

        public void Clear()
        {
            collection.Clear();
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Count"));
        }

        public bool Contains(T item) =>
            collection.Contains(item);

        public void CopyTo(T[] array, int arrayIndex) =>
            collection.CopyTo(array, arrayIndex);

        public bool Remove(T item)
        {
            bool success = collection.Remove(item);
            if (success)
            {
                CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Count"));
            }
            return success;
        }

        public IEnumerator<T> GetEnumerator() =>
            collection.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() =>
            collection.GetEnumerator();
    }
}