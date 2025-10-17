using Unity.Properties;
using UnityEngine;

namespace Paps.DevelopmentTools.Editor
{
    internal class TransformWorldDataSource
    {
        private GameObject _gameObject;

        [CreateProperty]
        public Vector3 WorldPosition
        {
            get
            {
                try
                {
                    return _gameObject?.transform.position ?? Vector3.zero;
                }
                catch (MissingReferenceException e)
                {
                    return Vector3.zero;
                }
            }
            set
            {
                if(_gameObject == null)
                    return;

                _gameObject.transform.position = value;
            }
        }

        [CreateProperty]
        public Vector3 WorldRotationEuler
        {
            get
            {
                try
                {
                    return _gameObject?.transform.rotation.eulerAngles ?? Vector3.zero;
                }
                catch (MissingReferenceException e)
                {
                    return Vector3.zero;
                }
            }
            set
            {
                if (_gameObject == null)
                    return;

                _gameObject.transform.rotation = Quaternion.Euler(value);
            }
        }

        [CreateProperty]
        public Vector3 WorldScale
        {
            get
            {
                try
                {
                    return _gameObject?.transform.lossyScale ?? Vector3.zero;
                }
                catch (MissingReferenceException e)
                {
                    return Vector3.zero;
                }
            }
            set
            {
                
            }
        }
        
        public void Set(GameObject gameObject)
        {
            _gameObject = gameObject;
        }

        public void Clear()
        {
            _gameObject = null;
        }
    }
}