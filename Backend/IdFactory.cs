using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend
{
    public static class IdFactory
    {
        private static int _nextId;

        public static void SetCurrentId(int id)
        {
            _nextId = id;
            //Properties.Settings.Default["lastId"] = id;
        }

        public static int GetCurrentId()
        {
            _nextId++;
            //Properties.Settings.Default["lastId"] = _nextId;
            return _nextId;
        }

        static IdFactory()
        {
            //_nextId = (int)Properties.Settings.Default["lastId"];
        }

    }
}
