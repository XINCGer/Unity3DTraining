/*============================================================================
Copyright (c) 2014 Qualcomm Connected Experiences, Inc.
All Rights Reserved.
============================================================================*/
 

#import "VuforiaRenderDelegate.h"

// Exported methods for setting surface recreated flag
extern "C" void setSurfaceRecreated();

@implementation VuforiaRenderDelegate

- (void)mainDisplayInited:(struct UnityRenderingSurface*)surface
{
}

- (void)onAfterMainDisplaySurfaceRecreate
{
	setSurfaceRecreated();
}
@end
