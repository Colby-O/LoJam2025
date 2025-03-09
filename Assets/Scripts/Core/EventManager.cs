using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace LoJam.Core
{
    public class EventResponse {
        private UnityEvent<Component, object> _callback = new UnityEvent<Component, object>();

        public void AddListener(UnityAction<Component, object> func) {
            _callback.AddListener(func);
        }

        public void Invoke(Component sender, object data) {
            _callback.Invoke(sender, data);
        }
    }

    public class EventManager
    {
        private Dictionary<string, EventResponse> _events = new Dictionary<string, EventResponse>();


        public void AddEvent<TEvent>(UnityAction<Component, object> callback) {
            if (_events.TryGetValue(nameof(TEvent), out EventResponse res)) {
                res.AddListener(callback);
            }
            else {
                _events.Add(nameof(TEvent), new EventResponse());
                AddEvent<TEvent>(callback);
            }
        }

        public void InvokeEvent<TEvent>(Component sender, object data) {
            if (_events.TryGetValue(nameof(TEvent), out EventResponse res)) {
                res.Invoke(sender, data);
            }
        }
    }
}
