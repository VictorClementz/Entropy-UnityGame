using System.Collections;
using UnityEngine;

public class ResourceBuilding : Building
{
    public ResourceType producedResource;
    void Start()
    {
        buildingName = "Resource Collector";

        //Do on place
        producedResource = calculateResourceTile();
        StartCoroutine(ProduceResourceRoutine(producedResource, 1f));

    }

    private ResourceType calculateResourceTile() //Cheap solution might change
    {

        // Get the resource from GridManager
        Vector3 myPos = transform.position;
        int x = Mathf.RoundToInt(myPos.x);
        int y = Mathf.RoundToInt(myPos.z);
        
        return gridManager.GetTileResource(x, y);
    }

    IEnumerator ProduceResourceRoutine(ResourceType resource, float interval)
    {
        yield return new WaitForSeconds(1f); 

        while (true)
        {
            ProduceResource(resource);
            yield return new WaitForSeconds(interval);
        }
    }


    private void ProduceResource(ResourceType resourceType)
    {


        //Add resource
        ResourceManager.Instance.AddResource(resourceType, 1);

    }
}
