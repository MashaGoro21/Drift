using UnityEngine;
using UnityEngine.EventSystems;

public class MyButton : MonoBehaviour
{
    [SerializeField] private float sensitivity = 2f;
    
    private bool isPressed;
    private float dampenPress = 0f;

    private void Start()
    {
        SetUpButton();
    }

    private void Update()
    {
        if (isPressed) dampenPress += sensitivity * Time.deltaTime;
        else dampenPress -= sensitivity * Time.deltaTime;

        dampenPress = Mathf.Clamp01(dampenPress);
    }

    private void SetUpButton()
    {
        EventTrigger trigger = gameObject.AddComponent<EventTrigger>();

        var pointerDown = new EventTrigger.Entry();
        pointerDown.eventID = EventTriggerType.PointerDown;
        pointerDown.callback.AddListener((e) => OnClickDown());

        var pointerUp = new EventTrigger.Entry();
        pointerUp.eventID = EventTriggerType.PointerUp;
        pointerUp.callback.AddListener((e) => OnClickUp());

        trigger.triggers.Add(pointerDown);
        trigger.triggers.Add(pointerUp);
    }

    private void OnClickDown() => isPressed = true;
    private void OnClickUp() => isPressed = false;
    public bool GetIsPressed() => isPressed;
    public float GetDampenPress() => dampenPress;
}
