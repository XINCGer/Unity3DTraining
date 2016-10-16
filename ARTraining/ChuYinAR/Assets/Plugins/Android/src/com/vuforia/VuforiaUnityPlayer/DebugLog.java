/*============================================================================
Copyright (c) 2016 PTC Inc. All Rights Reseverd
Copyright (c) 2010-2011 Qualcomm Connected Experiences, Inc.
============================================================================*/

package com.vuforia.VuforiaUnityPlayer;

import android.util.Log;

/** DebugLog is a support class for the Vuforia samples applications.
 * 
 *  Exposes functionality for logging.
 *  
 * */
public class DebugLog
{
    private static final String LOGTAG = "Vuforia";

    /** Logging functions to generate ADB logcat messages. */

    public static final void LOGE(String nMessage)
    {
        Log.e(LOGTAG, nMessage);
    }
    
    public static final void LOGW(String nMessage)
    {
        Log.w(LOGTAG, nMessage);
    }
    
    public static final void LOGD(String nMessage)
    {
        Log.d(LOGTAG, nMessage);
    }
    
    public static final void LOGI(String nMessage)
    {
        Log.i(LOGTAG, nMessage);
    }
}
