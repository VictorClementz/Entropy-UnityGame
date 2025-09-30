using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    [SerializeField] private BuildingPlacer buildingPlacer;

    //Insure if needed, if only menu/click based?
    void Update()
    {
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            int test = 0;
            buildingPlacer.PlaceBuilding(test);
        }
    }
}