using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore
{
    public class ViewsHistory
    {
        private readonly Queue<Tuple<string, int>> _queue;

        public ViewsHistory()
        {
            _queue = new Queue<Tuple<string, int>>(SettingsConstants.VIEWS_HISTORY_SIZE);
        }


        public bool Contains(string userId, int id) => Contains(new Tuple<string, int>(userId, id));

        public bool Contains(Tuple<string, int> entry)
        {
            var a = this.GetHashCode();
            return _queue.Contains(entry);
        }

        public void Add(string userId, int id) => Add(new Tuple<string, int>(userId, id));

        public void Add(Tuple<string, int> entry)
        {
            if(_queue.Count == SettingsConstants.VIEWS_HISTORY_SIZE)
            {
                _queue.Dequeue();
            }
            _queue.Enqueue(entry);
        }
    }
}
