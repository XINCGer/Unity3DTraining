///[[Notice:This cs uiview file is auto generate by UIViewExporter，don't modify it manually! --]]

public partial class UILoginPanel : UIBase
{
	private UnityEngine.UI.Text m_Text;
	private UnityEngine.UI.Button m_Button;
	protected override void BindView()
	{
		var UICollection = this.transform.GetComponent<UICollection>();
		m_Text = UICollection.GetComponent<UnityEngine.UI.Text>(0);
		m_Button = UICollection.GetComponent<UnityEngine.UI.Button>(1);
	}
}
