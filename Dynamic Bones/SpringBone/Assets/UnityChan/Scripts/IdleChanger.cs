using UnityEngine;
using System.Collections;

//
// ↑↓キーでループアニメーションを切り替えるスクリプト（ランダム切り替え付き）Ver.3
// 2014/04/03 N.Kobayashi
//

// Require these components when using this script
[RequireComponent(typeof(Animator))]



public class IdleChanger : MonoBehaviour
{
	
	private Animator anim;						// Animatorへの参照
	private AnimatorStateInfo currentState;		// 現在のステート状態を保存する参照
	private AnimatorStateInfo previousState;	// ひとつ前のステート状態を保存する参照
	public bool _random = false;				// ランダム判定スタートスイッチ
	public float _threshold = 0.5f;				// ランダム判定の閾値
	public float _interval = 2f;				// ランダム判定のインターバル
	//private float _seed = 0.0f;					// ランダム判定用シード
	


	// Use this for initialization
	void Start ()
	{
		// 各参照の初期化
		anim = GetComponent<Animator> ();
		currentState = anim.GetCurrentAnimatorStateInfo (0);
		previousState = currentState;
		// ランダム判定用関数をスタートする
		StartCoroutine ("RandomChange");
	}
	
	// Update is called once per frame
	void  Update ()
	{
		// ↑キー/スペースが押されたら、ステートを次に送る処理
		if (Input.GetKeyDown ("up") || Input.GetButton ("Jump")) {
			// ブーリアンNextをtrueにする
			anim.SetBool ("Next", true);
		}
		
		// ↓キーが押されたら、ステートを前に戻す処理
				if (Input.GetKeyDown ("down")) {
			// ブーリアンBackをtrueにする
			anim.SetBool ("Back", true);
		}
		
		// "Next"フラグがtrueの時の処理
		if (anim.GetBool ("Next")) {
			// 現在のステートをチェックし、ステート名が違っていたらブーリアンをfalseに戻す
			currentState = anim.GetCurrentAnimatorStateInfo (0);
			if (previousState.nameHash != currentState.nameHash) {
				anim.SetBool ("Next", false);
				previousState = currentState;				
			}
		}
		
		// "Back"フラグがtrueの時の処理
		if (anim.GetBool ("Back")) {
			// 現在のステートをチェックし、ステート名が違っていたらブーリアンをfalseに戻す
			currentState = anim.GetCurrentAnimatorStateInfo (0);
			if (previousState.nameHash != currentState.nameHash) {
				anim.SetBool ("Back", false);
				previousState = currentState;
			}
		}
	}


	void OnGUI()
	{
				GUI.Box(new Rect(Screen.width - 110 , 10 ,100 ,90), "Change Motion");
				if(GUI.Button(new Rect(Screen.width - 100 , 40 ,80, 20), "Next"))
					anim.SetBool ("Next", true);
				if(GUI.Button(new Rect(Screen.width - 100 , 70 ,80, 20), "Back"))
					anim.SetBool ("Back", true);
	}


	// ランダム判定用関数
	IEnumerator RandomChange ()
	{
		// 無限ループ開始
		while (true) {
			//ランダム判定スイッチオンの場合
			if (_random) {
				// ランダムシードを取り出し、その大きさによってフラグ設定をする
				float _seed = Random.Range (-1f, 1f);
				if (_seed <= -_threshold) {
					anim.SetBool ("Back", true);
				} else if (_seed >= _threshold) {
					anim.SetBool ("Next", true);
				}
			}
			// 次の判定までインターバルを置く
			yield return new WaitForSeconds (_interval);
		}

	}

}
