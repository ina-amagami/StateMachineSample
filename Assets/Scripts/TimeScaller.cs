using UnityEngine;

public class TimeScaller : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
		{
            Time.timeScale = 3;
		}
		if (Input.GetKeyUp(KeyCode.Space))
		{
			Time.timeScale = 1;
		}
    }
}
