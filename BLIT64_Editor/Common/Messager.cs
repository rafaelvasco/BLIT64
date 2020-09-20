using System;
using System.Collections.Generic;

namespace BLIT64_Editor
{
    public static class Messager
    {
        private static readonly Dictionary<int, List<Action>> _no_args_event_handlers = new Dictionary<int, List<Action>>();

        public static void On(int message_code, Action action)
        {
            if (!_no_args_event_handlers.ContainsKey(message_code))
            {
                _no_args_event_handlers.Add(message_code, new List<Action>());
            }

            _no_args_event_handlers[message_code].Add(action);
        }

        public static void Emit(int message_code)
        {
            foreach(var handler in _no_args_event_handlers[message_code])
            {
                handler.Invoke();
            }
        }
    }
}
