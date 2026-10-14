using UnityEngine;

public class InputManager : MonoBehaviour
{
    void Update()
    {
        float moveInput = Input.GetAxis("Horizontal");
        if(Mathf.Abs(moveInput) > 0.01f)
        {
            GameEvents.TriggerHorizontalMove(moveInput);
        }

        if(Input.GetKeyDown(KeyCode.Space))
        {
            GameEvents.TriggerConfirmInput();
        }
    }
}
