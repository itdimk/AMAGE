using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AMAGE.Services.Imaging
{
    public sealed class RepositoryService<TValue> : IRepository<TValue>
    {
        private readonly Action<TValue, Stream> saving;
        private readonly Func<Stream, TValue> loading;

        private readonly Dictionary<string, TValue> data
            = new Dictionary<string, TValue>();

        private readonly Dictionary<string, Stack<string>> snapshots
            = new Dictionary<string, Stack<string>>();

        public event EventHandler<string> ItemAdded;
        public event EventHandler<string> ItemChanged;
        public event EventHandler<string> ItemRemoved;

        public IReadOnlyList<string> Keys => data.Keys.ToArray();
        public IReadOnlyList<TValue> Values => data.Values.ToArray();

        public TValue this[string key]
        {
            get { return data[key]; }
            set { data[key] = value; }
        }

        public RepositoryService()
        {

        }

        public RepositoryService(Action<TValue, Stream> saving, Func<Stream, TValue> loading)
        {
            this.saving = saving;
            this.loading = loading;
        }

        ~RepositoryService()
        {
            foreach (Stack<string> files in snapshots.Values)
                foreach (string file in files)
                    File.Delete(file);
        }

        public void Add(string key, TValue value)
        {
            data.Add(key, value);
            ItemAdded?.Invoke(this, key);
        }

        public void Remove(string key)
        {
            if (snapshots.ContainsKey(key))
            {
                foreach (string fileName in snapshots[key])
                    File.Delete(fileName);

                snapshots.Remove(key);
            }

            if (data.ContainsKey(key))
            {
                data.Remove(key);
                ItemRemoved?.Invoke(this, key);
            }
        }

        public void Snapshot(string key)
        {
            string destination = Path.GetTempFileName();

            using (Stream output = File.Open(destination, FileMode.Open))
                saving.Invoke(data[key], output);

            if (!snapshots.ContainsKey(key))
                snapshots.Add(key, new Stack<string>());

            snapshots[key].Push(destination);
        }

        public bool CanUndo(string key)
        {
            return snapshots.ContainsKey(key) && snapshots[key].Count > 0;
        }

        public bool CanRedo(string key)
        {
            return false;
        }

        public TValue Undo(string key)
        {
            string filename = snapshots[key].Pop();
            using (Stream source = File.OpenRead(filename))
            {
                TValue result = loading.Invoke(source);

                File.Delete(filename);
                return result;
            }
        }

        public TValue Redo(string key)
        {
            throw new NotImplementedException();
        }

        public bool ContainsKey(string key)
        {
            return data.ContainsKey(key);
        }

        public bool ContainsValue(TValue value)
        {
            return data.ContainsValue(value);
        }

        public void OnItemChanged(string key)
        {
            ItemChanged?.Invoke(this, key);
        }

        public bool TryGetValue(string key, out TValue value)
        {
            return data.TryGetValue(key, out value);
        }
    }
}
