using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Paps.Levels
{
    public class LevelList : ScriptableObject, IReadOnlyList<Level>
    {
        [SerializeField] private List<Level> _levels;

        private Dictionary<string, Level> _levelsDictionary;

        public Level this[int index] => _levels[index];

        public int Count => _levels.Count;

        public IEnumerator<Level> GetEnumerator()
        {
            return _levels.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private void PrepareLevelsDictionary()
        {
            _levelsDictionary = new Dictionary<string, Level>(_levels.Count);

            for(int i = 0; i < _levels.Count; i++)
            {
                var current = _levels[i];
                _levelsDictionary[current.Id] = current;
            }
        }

        private void EnsureDictionaryInitialized()
        {
            if(_levelsDictionary == null)
            {
                PrepareLevelsDictionary();
            }
        }

        public Level GetById(string id)
        {
            EnsureDictionaryInitialized();

            return _levelsDictionary[id];
        }

        public bool Contains(string levelId)
        {
            EnsureDictionaryInitialized();

            return _levelsDictionary.ContainsKey(levelId);
        }

#if UNITY_EDITOR
        public void SetLevels(List<Level> levels)
        {
            _levels.Clear();

            _levels = levels;
            
            UnityEditor.EditorUtility.SetDirty(this);
        }
#endif
    }
}
