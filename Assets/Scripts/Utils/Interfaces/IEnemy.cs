using UnityEngine;

namespace Utils.Interfaces
{
    public interface IEnemy
    {
        public Vector3 GetAimPosition { get; }
        public void SetupEnemy(Transform target);
    }
}
