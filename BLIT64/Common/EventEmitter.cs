using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BLIT64
{
    public abstract class EventEmitter
    {
        private readonly Dictionary<int, List<Action<object>>> _events;

        protected EventEmitter()
        {
            _events = new Dictionary<int, List<Action<object>>>();
        }

        public void On(int event_id, Action<object> method)
        {
            if (_events.TryGetValue(event_id, out var subscribed_methods))
            {
                subscribed_methods.Add(method);
            }
            else
            {
                _events.Add(event_id, new List<Action<object>> {method});    
            }
        }

        public void Emit(int event_id, object data = null)
        {
            if (_events.TryGetValue(event_id, out var subscribed_methods))
            {
                for (int i = 0; i < subscribed_methods.Count; ++i)
                {
                    subscribed_methods[i](data);
                }
            }
            else
            {
                throw new Exception($"Event [{event_id}] does not exist in the emitter.");
            }
        }

        public void EmitAsync(int event_id, object data = null)
        {
            if (_events.TryGetValue(event_id, out var subscribed_methods))
            {
                foreach (var subscribed_method in subscribed_methods)
                {
                    Task.Run(() => subscribed_method(data));
                }
            }
            else
            {
                throw new Exception($"Event [{event_id}] does not exist to have methods removed");
            }
        }

        public void RemoveListener(int event_id, Action<object> method)
        {
            if (_events.TryGetValue(event_id, out var subscribed_methods))
            {
                var method_exists = subscribed_methods.Exists(e => e == method);
                if (method_exists)
                {
                    subscribed_methods.Remove(method);
                }
                else
                {
                    throw new Exception($"Func [{method.Method}] does not exist to be removed.");
                }
            }
            else
            {
                throw new Exception($"Event [{event_id}] does not exist in the emitter.");
            }
        }

        public void RemoveAllListeners(int event_id)
        {
            if (_events.TryGetValue(event_id, out var subscribed_methods))
            {
                subscribed_methods.RemoveAll(x => x != null);
            }
            else
            {
                throw new Exception($"Event [{event_id}] does not exist to have methods removed");
            }
        }
    }
}
