using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class jumper : MonoBehaviour {
	public Button jumpButton;
  public PlayerMovement pm;

	void Start () {
		Button btn = jumpButton.GetComponent<Button>();
		btn.onClick.AddListener(TaskOnClick);
	}

	void TaskOnClick(){
		if (pm.jumpTimer >= 69) {
      pm.rb.AddForce(0, pm.jumpForce, 0, ForceMode.Impulse);
      pm.jumpTimer = 0;
    }
	}
}