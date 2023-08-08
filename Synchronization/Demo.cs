using UnityEditor.Callbacks;
using Synchronization;
using static Synchronization.Synchronizer;
using PSXView.Utils;

static class Demo
{
    [DidReloadScripts]
    private static void OnReloadScripts()
    {
        new Synchronizer("ShaderLibrary", "#define SYNC_", "psxViewSyncData") 
            .Synchronize("PSXSphericalLight.hlsl", String("SphericalLightsMaxCount", $"{TargetGraphics.SphericalLightsMaxCount}"))
            .Synchronize("PSXCubeLight.hlsl",      String("CubeLightsMaxCount",      $"{TargetGraphics.CubeLightsMaxCount}"),
                                                   String("SomeString1",             $"{SomeValue1}"),
                                                   String("SomeString2",             $"{SomeValue2}"));
    }
}