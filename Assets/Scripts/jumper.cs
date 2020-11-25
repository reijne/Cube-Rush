using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class jumper : MonoBehaviour {
	public Button jumpButton;
  public PlayerMovement ps;

	void Start () {
		Button btn = jumpButton.GetComponent<Button>();
		btn.onClick.AddListener(TaskOnClick);
	}

	void TaskOnClick(){
		if (!ps.justJumped) {
      ps.rb.AddForce(0, ps.jumpForce, 0, ForceMode.Impulse);
      ps.justJumped = true;
    }
	}
}