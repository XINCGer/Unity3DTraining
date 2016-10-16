Shader "DepthMask" {
   
    SubShader {
        // Render the mask after regular geometry, but before masked geometry and
        // transparent things.
       
        Tags {"Queue" = "Geometry-10" }
       
        // Turn off lighting, because it's expensive and the thing is supposed to be
        // invisible anyway.
       
        Lighting Off

        // Draw into the depth buffer in the usual way.  This is probably the default,
        // but it doesn't hurt to be explicit.

        ZTest LEqual
        ZWrite On

        // Don't draw anything into the RGBA channels. This is an undocumented
        // argument to ColorMask which lets us avoid writing to anything except
        // the depth buffer.

        ColorMask 0

        // Do nothing specific in the pass:

        Pass {}
    }
}
