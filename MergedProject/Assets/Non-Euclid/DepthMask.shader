Shader "Masked/Mask" {
	SubShader {
		// Render the mask after regular geometry, but before masked geometry and
		// transparent things.
		
		Tags {"Queue" = "Geometry+990" }
		
		// Don't draw in the RGBA channels; just the depth buffer
		
		ColorMask 0
		ZWrite On
		
		//Edit - Compensate for masking objects on same plane
		Offset -1,-1
		
		// Do nothing specific in the pass:
		
		Pass {}
	}
}