using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.RenderGraphModule.Util;
using UnityEngine.Rendering.Universal;

namespace LoJam.Rendering
{
    public sealed class CRTRendererPass : ScriptableRenderPass
    {
        private Material _mat;
        private RTHandle _rtHandle;

        public CRTRendererPass(Material mat) => _mat = mat;

        public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
        {
            cameraTextureDescriptor.depthBufferBits = (int)DepthBits.None;
            RenderingUtils.ReAllocateIfNeeded(
                ref _rtHandle,
                cameraTextureDescriptor,
                wrapMode: TextureWrapMode.Clamp
            );
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            if (_mat == null || _rtHandle == null || _rtHandle.rt == null || renderingData.cameraData.renderer.cameraColorTargetHandle == null || renderingData.cameraData.renderer.cameraColorTargetHandle.rt == null) return;

            CommandBuffer cmd = CommandBufferPool.Get();

            _mat.SetFloat("m_time", Time.time);
            cmd.Blit(renderingData.cameraData.renderer.cameraColorTargetHandle, _rtHandle, _mat, 0);
            cmd.Blit(_rtHandle, renderingData.cameraData.renderer.cameraColorTargetHandle);

            context.ExecuteCommandBuffer(cmd);
            cmd.Clear();
            cmd.Release();
        }

        public override void FrameCleanup(CommandBuffer cmd)
        {
            base.FrameCleanup(cmd);
        }

        public void Destroy()
        {
            _rtHandle?.Release();
            _rtHandle = null;
        }
    }
}
