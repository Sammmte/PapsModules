using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Paps.Levels
{
    public class LevelList : ScriptableObject, IReadOnlyList<Level>
    {
        [SerializeField] internal List<Level> Levels;

        private Dictionary<string, Level> _levelsDictionary;

        public Level this[int index] => Levels[index];

        public int Count => Levels.Count;

        public IEnumerator<Level> GetEnumerator()
        {
            return Levels.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private void PrepareLevelsDictionary()
        {
            _levelsDictionary = new Dictionary<string, Level>(Levels.Count);

            for(int i = 0; i < Levels.Count; i++)
            {
                var current = Levels[i];
                _levelsDictionary[current.Id] = current;
            }
        }

        public Level GetById(string id)
        {
            if(_levelsDictionary == null)
            {
                PrepareLevelsDictionary();
            }

            return _levelsDictionary[id];
        }
    }
}
