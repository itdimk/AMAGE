using System;
using System.Collections.Generic;

namespace AMAGE.Services
{
    public interface IRepository<TValue>
    {
        event EventHandler<string> ItemAdded;
        event EventHandler<string> ItemChanged;
        event EventHandler<string> ItemRemoved;

        TValue this[string key] { get; set; }

        IReadOnlyList<string> Keys { get; }
        IReadOnlyList<TValue> Values { get; }

        void Add(string key, TValue value);
        void Remove(string key);

        void Snapshot(string key);
        bool CanUndo(string key);
        bool CanRedo(string key);

        TValue Undo(string key);
        TValue Redo(string key);

        bool ContainsKey(string key);
        bool ContainsValue(TValue value);

        bool TryGetValue(string key, out TValue value);

        void OnItemChanged(string key);
    }
}

