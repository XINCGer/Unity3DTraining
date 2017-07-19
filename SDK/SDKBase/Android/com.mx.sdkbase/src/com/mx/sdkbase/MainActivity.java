package com.mx.sdkbase;

import android.app.Activity;
import android.os.Bundle;
import android.view.Menu;
import android.view.MenuItem;
import android.widget.Toast;

import com.unity3d.player.UnityPlayer;
import com.unity3d.player.UnityPlayerActivity;

public class MainActivity extends UnityPlayerActivity {

	private static MainActivity instance;

	@Override
	protected void onCreate(Bundle savedInstanceState) {
		super.onCreate(savedInstanceState);
		// setContentView(R.layout.activity_main);

		instance = this;
	}

	@Override
	public boolean onCreateOptionsMenu(Menu menu) {
		// Inflate the menu; this adds items to the action bar if it is present.
		getMenuInflater().inflate(R.menu.main, menu);
		return true;
	}

	@Override
	public boolean onOptionsItemSelected(MenuItem item) {
		// Handle action bar item clicks here. The action bar will
		// automatically handle clicks on the Home/Up button, so long
		// as you specify a parent activity in AndroidManifest.xml.
		int id = item.getItemId();
		if (id == R.id.action_settings) {
			return true;
		}
		return super.onOptionsItemSelected(item);
	}

	public int Sum(int x, int y) {
		return x + y;
	}

	public int Max(int x, int y) {
		return Math.max(x, y);
	}

	public void MakeToast(String str) {
		Toast.makeText(this, str, Toast.LENGTH_LONG).show();
	}

	public int AddOne(int x) {
		return x + 1;
	}

	public static MainActivity GetInstance() {
		return instance;
	}
	
	public void CallUnityFunc(String str){
		str=str+"Android Call Unity.";
		String ReceiveObject="MessageHandler";
		String ReceiverMethod="Receive";
		UnityPlayer.UnitySendMessage(ReceiveObject, ReceiverMethod, str);
	}
}
