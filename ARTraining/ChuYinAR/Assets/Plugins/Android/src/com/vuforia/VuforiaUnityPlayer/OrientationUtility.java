/*============================================================================
Copyright (c) 2016 PTC Inc. All Rights Reseverd
Copyright (c) 2010-2014 Qualcomm Connected Experiences, Inc.
============================================================================*/


package com.vuforia.VuforiaUnityPlayer;

import android.app.Activity;
import android.content.Context;
import android.content.pm.ActivityInfo;
import android.content.res.Configuration;
import android.os.Build;
import android.util.DisplayMetrics;
import android.view.Display;
import android.view.Surface;
import android.view.WindowManager;

/* On some devices specific orientations are not supported if "autorotation" is not enabled in the screen settings.
 * Unity will still report the unsupported orientation at runtime via Screen.orientation, which will lead to inconsitencies
 * in video background rendering. Querying the actual orientation from the Activity resolves the problem.
**/
public class OrientationUtility
{
    // The values here need to match those in Tracker.h
    static final int SCREEN_ORIENTATION_UNKNOWN = 0;
    static final int SCREEN_ORIENTATION_PORTRAIT = 1;
    static final int SCREEN_ORIENTATION_PORTRAITUPSIDEDOWN = 2;
    static final int SCREEN_ORIENTATION_LANDSCAPELEFT = 3;
    static final int SCREEN_ORIENTATION_LANDSCAPERIGHT = 4;

    public static int getSurfaceOrientation(Activity activity)
    {

        // Sanity check:
        if (activity == null)
        {
            return -1; // invalid value
        }

        Configuration config = activity.getResources().getConfiguration();
        Display display = ((WindowManager)activity.getSystemService(Context.WINDOW_SERVICE)).getDefaultDisplay();

        int displayRotation;
        if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.FROYO)
        {
            displayRotation = display.getRotation(); // only available from Froyo
        }
        else
        {
            displayRotation = display.getOrientation();
        }

        int activityOrientation = SCREEN_ORIENTATION_UNKNOWN;

        switch (config.orientation)
        {
            case Configuration.ORIENTATION_PORTRAIT:
            case Configuration.ORIENTATION_SQUARE:
                activityOrientation = ( (displayRotation == Surface.ROTATION_0 || displayRotation == Surface.ROTATION_270) ? SCREEN_ORIENTATION_PORTRAIT : SCREEN_ORIENTATION_PORTRAITUPSIDEDOWN );
                break;

            case Configuration.ORIENTATION_LANDSCAPE:
                activityOrientation = ( (displayRotation == Surface.ROTATION_0 || displayRotation == Surface.ROTATION_90) ? SCREEN_ORIENTATION_LANDSCAPELEFT : SCREEN_ORIENTATION_LANDSCAPERIGHT);
                break;

            case Configuration.ORIENTATION_UNDEFINED:
            default:
                break;
        }

        return activityOrientation;
    }
 
}
