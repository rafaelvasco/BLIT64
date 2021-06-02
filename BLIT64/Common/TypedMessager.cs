using System;
using System.Collections.Generic;

namespace BLIT64
{
    public static class TypedMessager<T> where T : struct
    {
        private static readonly Dictionary<int, List<Action<T>>> _with_args_event_handlers = new Dictionary<int, List<Action<T>>>();

        public static void On(int message_code, Action<T> action)
        {
            if (!_with_args_event_handlers.ContainsKey(message_code))
            {
                _with_args_event_handlers.Add(message_code, new List<Action<T>>());
            }

            _with_args_event_handlers[message_code].Add(action);
        }

        public static void Emit(int message_code, T value)
        {
            foreach(var handler in _with_args_event_handlers[message_code])
            {
                handler.Invoke(value);
            }
        }
    }
}
