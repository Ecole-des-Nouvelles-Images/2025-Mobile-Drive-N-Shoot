using UnityEngine;
using UnityEngine.SceneManagement;

namespace __Workspaces.Hugoi.Scripts
{
    public class UIMethods : MonoBehaviour
    {
        public void LoadScene(int id)
        {
            SceneManager.LoadScene(id);
        }
    }
}
