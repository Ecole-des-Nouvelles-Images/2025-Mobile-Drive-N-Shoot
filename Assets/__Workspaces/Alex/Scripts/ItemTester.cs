using UnityEngine;

public class ItemTester : MonoBehaviour
{
    public Item itemToTest;
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            Debug.Log("testing item: " + itemToTest.name);
            itemToTest.Execute(gameObject);
        }
    }

    public void TestItem()
    {
        Debug.Log("Testing item: " + itemToTest.name);
        itemToTest.Execute(gameObject);
    }
}
