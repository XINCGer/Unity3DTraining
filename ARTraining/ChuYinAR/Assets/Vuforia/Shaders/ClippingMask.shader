Shader "ClippingMask" {
   
    SubShader {
        // Render the mask after regular geometry and transparent things but 
        // but before any other overlays
       
        Tags {"Queue" = "Overlay-10" }
       
        // Turn off lighting, because it's expensive and the thing is supposed to be
        // invisible anyway.
       
        Lighting Off

        // Draw into the depth buffer in the usual way.  This is probably the default,
        // but it doesn't hurt to be explicit.

        ZTest Always
        ZWrite On

        // Draw black background into the RGBA channel
		Color (0,0,0,0)
        ColorMask RGBA

		// compare stencil buffer		
		Stencil {
			Ref 0
			Comp Equal
		}

        // Do nothing specific in the pass:

        Pass {}
    }
}
