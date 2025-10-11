using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace NotificationsVisualizerLibrary.Model
{
    internal partial class DataBindingValues : IEnumerable<KeyValuePair<string, string>>
    {
        private readonly Dictionary<string, string> _values;
        public uint Version { get; private set; }

        public DataBindingValues(PreviewNotificationData data)
        {
            _values = [];
            foreach (KeyValuePair<string, string> pair in data.Values)
            {
                _values[pair.Key] = pair.Value;
            }
            Version = data.Version;
        }

        public bool TryGetValue(string key, [NotNullWhen(true)] out string? value) => _values.TryGetValue(key, out value);

        public void Update(IEnumerable<KeyValuePair<string, string>> values)
        {
            foreach (KeyValuePair<string, string> pair in values)
            {
                _values[pair.Key] = pair.Value;
            }

            string[] newKeys = [.. values.Select(i => i.Key)];
            string[] oldKeys = [.. _values.Keys];
            foreach (string oldKey in oldKeys)
            {
                // If key no longer exists
                if (!newKeys.Contains(oldKey))
                {
                    // Remove the value
                    _values.Remove(oldKey);
                }
            }
        }

        public IEnumerator<KeyValuePair<string, string>> GetEnumerator() => _values.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
