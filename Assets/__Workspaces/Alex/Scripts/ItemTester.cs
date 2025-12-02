using UnityEngine;

public class ItemTester : MonoBehaviour
{
    public Item itemToTest;
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            itemToTest.Execute(gameObject);
            Debug.Log("testing item: " + itemToTest.name);
        }
    }
}
